/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Logging;

namespace TRBot.Misc
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
            {
                AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
            }
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
                //Create the directory to the crash log path
                if (Directory.Exists(Debug.CrashLogPath) == false)
                {
                    Directory.CreateDirectory(Debug.CrashLogPath);
                }

                //Dump the message, stack trace, and logs to a file
                using (StreamWriter writer = File.CreateText(Debug.GetCrashLogPath()))
                {
                    string message = $"Message: {exc.Message}\n\nStack Trace:\n";
                    string trace = $"{exc.StackTrace}\n\n";

                    writer.Write(message);
                    writer.Write(trace);

                    writer.Flush();

                    TRBotLogger.Logger.Fatal($"CRASH: {exc.Message}");
                    TRBotLogger.Logger.Fatal(exc.StackTrace);
                }
            }
        }
    }
}
