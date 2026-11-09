using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MultiPresence.Infrastructure
{
    internal sealed class ConfigurationEditorDocument
    {
        public static readonly string[] Fields =
        {
            "Details", "State", "LargeAsset", "SmallAsset", "LargeAssetText", "SmallAssetText",
            "Button1Text", "Button1URL", "Button2Text", "Button2URL"
        };
        private static readonly JsonDocumentOptions ReadOptions = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
        private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };
        private byte[] originalBytes;
        private string originalText;

        private ConfigurationEditorDocument(string path, byte[] bytes)
        {
            Path = System.IO.Path.GetFullPath(path);
            originalBytes = bytes;
            originalText = Decode(bytes);
            Text = originalText;
        }

        public string Path { get; }
        public string Text { get; set; }
        public bool IsDirty => !string.Equals(Text, originalText, StringComparison.Ordinal);

        public static ConfigurationEditorDocument Load(string path)
        {
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length > 4 * 1024 * 1024)
            {
                throw new InvalidDataException("This configuration is too large to open in the editor.");
            }
            return new ConfigurationEditorDocument(path, bytes);
        }

        public string[] GetGames()
        {
            return GetRoot()["Games"]!.AsObject().Where(item => item.Value is JsonObject).Select(item => item.Key).ToArray();
        }

        public string[] GetStates(string game)
        {
            return GetGame(game).Where(item => item.Value is JsonObject).Select(item => item.Key).ToArray();
        }

        public string GetField(string game, string state, string field)
        {
            var value = GetGame(game)[state]?[field];
            return value is JsonValue item && item.TryGetValue<string>(out var text) ? text : "";
        }

        public void SetField(string game, string state, string field, string value)
        {
            if (!Fields.Contains(field, StringComparer.Ordinal))
            {
                throw new ArgumentException("Unknown presence field.", nameof(field));
            }
            var root = GetRoot();
            var template = root["Games"]?[game]?[state] as JsonObject ?? throw new InvalidDataException("Select a game and state first.");
            if (template[field] is JsonValue current && current.TryGetValue<string>(out var currentText) && currentText == value)
            {
                return;
            }
            template[field] = value;
            Text = root.ToJsonString(WriteOptions) + Environment.NewLine;
        }

        public bool TryValidate(out string error)
        {
            try
            {
                var root = GetRoot();
                var games = root["Games"]!.AsObject();
                if (!games.Any(item => item.Value is JsonObject))
                {
                    throw new InvalidDataException("The Games object must contain at least one game.");
                }
                foreach (var game in games.Where(item => item.Value is JsonObject))
                {
                    foreach (var state in game.Value!.AsObject().Where(item => item.Value is JsonObject))
                    {
                        foreach (var field in Fields)
                        {
                            var value = state.Value![field];
                            if (value != null && (value is not JsonValue item || !item.TryGetValue<string>(out _)))
                            {
                                throw new InvalidDataException($"{game.Key} / {state.Key}: {field} must be text or null.");
                            }
                        }
                    }
                }
                error = "";
                return true;
            }
            catch (JsonException exception)
            {
                error = $"Invalid JSON at line {(exception.LineNumber ?? 0) + 1}, column {(exception.BytePositionInLine ?? 0) + 1}: {exception.Message}";
                return false;
            }
            catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
            {
                error = exception.Message;
                return false;
            }
        }

        public void Save()
        {
            if (!TryValidate(out var error))
            {
                throw new InvalidDataException(error);
            }
            if (!IsDirty)
            {
                return;
            }
            var bytes = new UTF8Encoding(false, true).GetBytes(Text);
            using var stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            if (stream.Length != originalBytes.Length)
            {
                throw new ConfigurationConflictException();
            }
            var current = new byte[originalBytes.Length];
            stream.ReadExactly(current);
            if (!current.AsSpan().SequenceEqual(originalBytes))
            {
                throw new ConfigurationConflictException();
            }
            try
            {
                stream.Position = 0;
                stream.Write(bytes);
                stream.SetLength(bytes.Length);
                stream.Flush(true);
            }
            catch
            {
                try
                {
                    stream.Position = 0;
                    stream.Write(originalBytes);
                    stream.SetLength(originalBytes.Length);
                    stream.Flush(true);
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                    AppLog.Warning("Could not restore a configuration after a failed save.", exception);
                }
                throw;
            }
            originalBytes = bytes;
            originalText = Text;
        }

        private JsonObject GetRoot()
        {
            var root = JsonNode.Parse(Text, documentOptions: ReadOptions) as JsonObject;
            if (root?["Games"] is not JsonObject)
            {
                throw new InvalidDataException("The configuration must contain a Games object.");
            }
            return root;
        }

        private JsonObject GetGame(string game)
        {
            return GetRoot()["Games"]?[game] as JsonObject ?? throw new InvalidDataException("The selected game is not present in this file.");
        }

        private static string Decode(byte[] bytes)
        {
            int offset = bytes.AsSpan().StartsWith(new byte[] { 0xEF, 0xBB, 0xBF }) ? 3 : 0;
            return new UTF8Encoding(false, true).GetString(bytes, offset, bytes.Length - offset);
        }
    }

    internal sealed class ConfigurationConflictException : IOException
    {
        public ConfigurationConflictException() : base("This file was changed outside the editor. Your changes have not been saved. Reload the file to review the current version, or copy your edits from the JSON tab first.")
        {
        }
    }
}
