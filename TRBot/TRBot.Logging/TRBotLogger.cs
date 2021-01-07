using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TRBot.Logging
{
    /// <summary>
    /// Handles logging.
    /// </summary>
    public static class TRBotLogger
    {
        /// <summary>
        /// The logger instance.
        /// </summary>
        public static ILogger Logger => Log.Logger;

        /// <summary>
        /// The log level switch for the minimum logging level.
        /// </summary>
        public static LoggingLevelSwitch LogLevelSwitch { get; private set; } = new LoggingLevelSwitch();

        /// <summary>
        /// Sets up the logger with given information.
        /// </summary>
        /// <param name="filePath">The file path to write the logs to.</param>
        /// <param name="logLevelSwitch">The minimum log level when logging to the file.</param>
        /// <param name="fileRollingInterval">The interval to roll over to a new file.</param>
        /// <param name="logFileSizeLimit">The file size limit before rolling over to a new log file.</param>
        /// <param name="fileWriteInterval">The interval in which to write all logs to the file.</param>
        public static void SetupLogger(string filePath, in LogEventLevel logLevel,
            in RollingInterval fileRollingInterval, in long logFileSizeLimit, TimeSpan? fileWriteInterval)
        {
            LogLevelSwitch.MinimumLevel = logLevel;

            Logger logger = new LoggerConfiguration().WriteTo.Console(levelSwitch: LogLevelSwitch)
                                .WriteTo.File(filePath, levelSwitch: LogLevelSwitch,
                                    rollingInterval: fileRollingInterval, rollOnFileSizeLimit: true,
                                    fileSizeLimitBytes: logFileSizeLimit,
                                    flushToDiskInterval: fileWriteInterval)
                                .CreateLogger();
            Log.Logger = logger;
        }

        /// <summary>
        /// Closes and disposes the logger.
        /// </summary>
        public static void DisposeLogger()
        {
            Log.CloseAndFlush();
        }
    }
}
