using MultiPresence.Properties;

namespace MultiPresence
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var updatesDisabled = Settings.Default.autoupdate;
            Settings.Default.autoupdate = true;

            using MainForm frm = new();

            Settings.Default.autoupdate = updatesDisabled;
            frm.Visible = false;
            frm.EnableRefactoredRuntime(updatesDisabled);
            Application.Run();
        }
    }
}
