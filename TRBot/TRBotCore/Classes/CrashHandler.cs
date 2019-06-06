using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot
{
    /// <summary>
    /// Handles crashes through unhandled exceptions.
    /// </summary>
    public class CrashHandler
    {
        public CrashHandler()
        {
            AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
            AppDomain.CurrentDomain.UnhandledException += HandleCrash;
        }

        ~CrashHandler()
        {
            if (AppDomain.CurrentDomain != null)
                AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
        }

        /// <summary>
        /// A crash handler for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception event.</param>
        /// <param name="e">An UnhandledExceptionEventArgs that contains the event data.</param>
        private void HandleCrash(object sender, UnhandledExceptionEventArgs e)
        {
            //Get the exception object
            Exception exc = e.ExceptionObject as Exception;
            if (exc != null)
            {
                //Dump the message, stack trace, and logs to a file
                using (StreamWriter writer = File.CreateText(Debug.DebugGlobals.GetCrashLogPath()))
                {
                    writer.Write($"Message: {exc.Message}\n\nStack Trace:\n");
                    writer.Write($"{exc.StackTrace}\n\n");

                    writer.Flush();
                }
            }
        }
    }
}
