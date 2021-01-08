using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TRBot.Logging
{
    /// <summary>
    /// Constants regarding logging.
    /// </summary>
    public static class LoggingConstants
    {
        /// <summary>
        /// The name of the logs folder.
        /// </summary>
        public const string LOGS_FOLDER_NAME = "Logs";

        /// <summary>
        /// The file name for the logs.
        /// </summary>
        public const string LOG_FILE_NAME = "TRBotLog.txt";

        /// <summary>
        /// The path to the logs folder.
        /// </summary>
        public static string LogFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOGS_FOLDER_NAME);

        
    }
}
