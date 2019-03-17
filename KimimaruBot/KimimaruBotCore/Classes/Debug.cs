using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace KimimaruBot
{
    /// <summary>
    /// Debugging.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Global values regarding debugging.
        /// </summary>
        public static class DebugGlobals
        {
            /// <summary>
            /// Gets the path for crash log files.
            /// </summary>
            /// <returns>A string with the full name of the crash log file.</returns>
            public static string GetCrashLogPath()
            {
                string time = GetFileFriendlyTimeStamp();

                string path = Path.Combine(Environment.CurrentDirectory, $"{GetAssemblyName()} {GetBuildNumber()} Crash Log - {time}.txt");

                return path;
            }

            /// <summary>
            /// Returns a file friendly time stamp of the current time.
            /// </summary>
            /// <returns>A string representing current time.</returns>
            public static string GetFileFriendlyTimeStamp()
            {
                string time = DateTime.Now.ToUniversalTime().ToString();
                time = time.Replace(':', '-');
                time = time.Replace('/', '-');

                return time;
            }

            // <summary>
            /// Gets the name of the assembly.
            /// </summary>
            /// <returns>A string representing the name of the assembly.</returns>
            public static string GetAssemblyName()
            {
                //Get the name from the assembly information
                System.Reflection.Assembly assembly = typeof(Debug).Assembly;
                System.Reflection.AssemblyName asm = assembly.GetName();

                return asm.Name;
            }

            /// <summary>
            /// Gets the full build number as a string.
            /// </summary>
            /// <returns>A string representing the full build number.</returns>
            public static string GetBuildNumber()
            {
                //Get the build number from the assembly information
                System.Reflection.Assembly assembly = typeof(Debug).Assembly;
                System.Reflection.AssemblyName asm = assembly.GetName();

                return asm.Version.Major + "." + asm.Version.Minor + "." + asm.Version.Build + "." + asm.Version.Revision;
            }
        }
    }
}
