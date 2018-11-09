using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using NLog.Targets;

namespace ClientsDbExplorer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConfigureLog();
            Application.Run(new MainView());
        }

        private static void ConfigureLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            
            //var logconsole = new ColoredConsoleTarget("logconsole");
            var logdebug = new DebuggerTarget("logdebug");

            //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logdebug);

            LogManager.Configuration = config;
        }
    }
}
