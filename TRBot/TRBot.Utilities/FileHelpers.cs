/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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

namespace TRBot.Utilities
{
    /// <summary>
    /// Helpers for handling files.
    /// </summary>
    public static class FileHelpers
    {
        /// <summary>
        /// Validates if a path exists and creates it if not.
        /// </summary>
        /// <param name="path">A valid path name to check or create.</param>
        /// <returns>true if the path exists or is created, false if the path doesn't exist and cannot be created.</returns>
        public static bool ValidatePath(string path)
        {
            if (Directory.Exists(path) == false)
            {
                //Console.WriteLine($"Folder at path \"{path}\" does not exist; creating");

                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Unable to create folder at \"{path}\": {exception.Message}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates if a path leading up to a file exists and creates it if not.
        /// </summary>
        /// <param name="path">A valid path, including the file name, to check or create.</param>
        /// <returns>true if the path for the file exists or is created, false if the path for the file doesn't exist and cannot be created.</returns>
        public static bool ValidatePathForFile(string path)
        {
            //If the path name is empty, exit early
            if (string.IsNullOrEmpty(path) == true)
            {
                return false;
            }

            string dirName = Path.GetDirectoryName(path);

            return ValidatePath(dirName);
        }

        /// <summary>
        /// Attempts to read text from a file.
        /// </summary>
        /// <param name="fullPath">The full path including the file name.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string containing the contents of the file. If the file doesn't exist, <see cref="string.Empty"/>.</returns>
        public static string ReadFromTextFile(string fullPath)
        {
            if (File.Exists(fullPath) == false)
            {
                return string.Empty;
            }

            //Try to read the file
            try
            {
                return File.ReadAllText(fullPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to read from file \"{fullPath}\": {exception.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to read text from a file.
        /// </summary>
        /// <param name="path">The base path the file resides in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string containing the contents of the file. If the file doesn't exist, <see cref="string.Empty"/>.</returns>
        public static string ReadFromTextFile(string path, string fileName)
        {
            if (Directory.Exists(path) == false)
            {
                return string.Empty;
            }

            string fullPath = Path.Combine(path, fileName);

            if (File.Exists(fullPath) == false)
            {
                return string.Empty;
            }

            //Try to read the file
            try
            {
                return File.ReadAllText(fullPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to read from file \"{fileName}\": {exception.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to read text from a file. If the file doesn't exist, it is created.
        /// </summary>
        /// <param name="fullPath">The full path including the file name.</param>
        /// <returns>A string containing the contents of the file.</returns>
        public static string ReadFromTextFileOrCreate(string fullPath)
        {
            if (ValidatePathForFile(fullPath) == false)
            {
                return string.Empty;
            }
            
            if (File.Exists(fullPath) == false)
            {
                //Create the file if it doesn't exist
                try
                {
                    File.WriteAllText(fullPath, string.Empty);
                    return string.Empty;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Unable to create file \"{fullPath}\": {exception.Message}");
                    return string.Empty;
                }
            }

            //Try to read the file
            try
            {
                return File.ReadAllText(fullPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to read from file \"{fullPath}\": {exception.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to read text from a file. If the file doesn't exist, it is created.
        /// </summary>
        /// <param name="path">The base path the file resides in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string containing the contents of the file.</returns>
        public static string ReadFromTextFileOrCreate(string path, string fileName)
        {
            if (ValidatePath(path) == false)
            {
                return string.Empty;
            }

            string fullPath = Path.Combine(path, fileName);
            
            if (File.Exists(fullPath) == false)
            {
                //Create the file if it doesn't exist
                try
                {
                    File.WriteAllText(fullPath, string.Empty);
                    return string.Empty;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Unable to create file \"{fileName}\": {exception.Message}");
                    return string.Empty;
                }
            }

            //Try to read the file
            try
            {
                return File.ReadAllText(fullPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to read from file \"{fileName}\": {exception.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to save text to a file.
        /// </summary>
        /// <param name="path">The base path the file resides in.</param>
        /// <param name="fileName">The name of the file to save to.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <returns>true if the text was successfully saved, otherwise false.</returns>
        public static bool SaveToTextFile(string path, string fileName, string contents)
        {
            if (ValidatePath(path) == false)
            {
                return false;
            }

            string fullPath = Path.Combine(path, fileName);

            try
            {
                File.WriteAllText(fullPath, contents);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to save to file \"{fileName}\": {exception.Message}");
                return false;
            }
        }

        /// <summary>
        /// Attempts to save text to a file.
        /// </summary>
        /// <param name="fullPath">The full path including the file name.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <returns>true if the text was successfully saved, otherwise false.</returns>
        public static bool SaveToTextFile(string fullPath, string contents)
        {
            if (ValidatePathForFile(fullPath) == false)
            {
                return false;
            }

            try
            {
                File.WriteAllText(fullPath, contents);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to save to file \"{fullPath}\": {exception.Message}");
                return false;
            }
        }
    }
}
