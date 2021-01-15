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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TRBot.Utilities;

namespace TRBot.Misc
{
    /// <summary>
    /// Contains utilities for debugging.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// The name of the crash log folder.
        /// </summary>
        public const string CRASH_LOG_FOLDER = "CrashLogs";

        /// <summary>
        /// The path to the crash log folder.
        /// </summary>
        public static readonly string CrashLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CRASH_LOG_FOLDER);
        
        /// <summary>
        /// Gets the path for crash log files.
        /// </summary>
        /// <returns>A string with the full name of the crash log file.</returns>
        public static string GetCrashLogPath()
        {
            string time = GetFileFriendlyTimeStamp();
            string path = Path.Combine(CrashLogPath, $"{GetAssemblyName()} {Application.VERSION_NUMBER} Crash Log - {time}.txt");
            return path;
        }
        
        /// <summary>
        /// Returns a file friendly time stamp of the current time.
        /// </summary>
        /// <returns>A string representing current time.</returns>
        public static string GetFileFriendlyTimeStamp()
        {
            return GetFileFriendlyTimeStamp(DateTime.Now);
        }
        
        /// <summary>
        /// Returns a file friendly time stamp of a given time.
        /// </summary>
        /// <param name="dateTime">The time stamp.</param>
        /// <returns>A string representing current time.</returns>
        public static string GetFileFriendlyTimeStamp(DateTime dateTime)
        {
            string time = dateTime.ToUniversalTime().ToString();
            time = time.Replace(':', '-');
            time = time.Replace('/', '-');
            return time;
        }
        
        /// <summary>
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
    }
}
