/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.Runtime.InteropServices;

namespace TRBot.Utilities
{
    /// <summary>
    /// Operating systems.
    /// </summary>
    public static class TRBotOSPlatform
    {
        /// <summary>
        /// Operating systems supported by TRBot.
        /// </summary>
        public enum OS
        {
            Windows = 1,
            GNULinux = 2,
            FreeBSD = 3,
            macOS = 4,
            Other = 10
        }

        /// <summary>
        /// The type of operating system TRBot is currently running on.
        /// </summary>
        public static readonly OS CurrentOS = OS.Other;

        static TRBotOSPlatform()
        {
            //Retrieve the operating system type
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                CurrentOS = OS.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) == true)
            {
                CurrentOS = OS.GNULinux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) == true)
            {
                CurrentOS = OS.FreeBSD;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) == true)
            {
                CurrentOS = OS.macOS;
            }
            else
            {
                CurrentOS = OS.Other;
            }
        }
    }
}
