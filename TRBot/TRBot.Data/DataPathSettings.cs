/* This file is part of TRBot.
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
using System.Text;

namespace TRBot.Data
{
    /// <summary>
    /// Settings for configuring data paths.
    /// </summary>
    public sealed class DataPathSettings
    {
        /// <summary>
        /// Whether to use a path relative to the directory the application is in.
        /// </summary>
        public bool UseRelativePath = true;

        /// <summary>
        /// The base path for the data folder.
        /// If <see cref="UseRelativePath"/> is true, this will be relative to the directory the application is in,
        /// otherwise, it will be an absolute path.
        /// </summary>
        public string BaseDataPath = "Data";

        /// <summary>
        /// Returns the path to the data folder.
        /// </summary>
        /// <returns>The path to the data folder.</returns>
        public string GetBaseDataPath()
        {
            if (UseRelativePath == true)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BaseDataPath);
            }

            return BaseDataPath;
        }

        /// <summary>
        /// Returns the full path to another file or folder from the base data folder.
        /// </summary>
        /// <param name="newPath">The file or folder from the data folder.</param>
        public string GetFullPathToData(string newPath)
        {
            return Path.Combine(GetBaseDataPath(), newPath);
        }
    }
}
