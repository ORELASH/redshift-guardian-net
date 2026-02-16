using System;
using System.Windows.Forms;
using RedshiftGuardianNET.DataAccess;
using RedshiftGuardianNET.Forms;
using RedshiftGuardianNET.CLI;

namespace RedshiftGuardianNET
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Supports both GUI mode (no arguments) and CLI mode (with arguments).
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Initialize database
                DatabaseContext.InitializeDatabase();

                // Check if CLI mode (arguments provided)
                if (args != null && args.Length > 0)
                {
                    // CLI Mode
                    CommandLineParser parser = new CommandLineParser();
                    if (parser.Parse(args))
                    {
                        CommandExecutor executor = new CommandExecutor();
                        int exitCode = executor.Execute(parser);
                        Environment.Exit(exitCode);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid arguments. Use 'help' command for usage information.");
                        Environment.Exit(1);
                        return;
                    }
                }

                // GUI Mode
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DashboardForm());
            }
            catch (Exception ex)
            {
                if (args != null && args.Length > 0)
                {
                    // CLI mode - write to console
                    Console.WriteLine("ERROR: " + ex.Message);
                    Environment.Exit(1);
                }
                else
                {
                    // GUI mode - show message box
                    MessageBox.Show(
                        "Failed to start application:\n\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
    }
}
