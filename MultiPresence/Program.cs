namespace MultiPresence
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using MainForm frm = new();
            frm.Visible = false;
            frm.EnableRefactoredRuntime();
            Application.Run();
        }
    }
}
