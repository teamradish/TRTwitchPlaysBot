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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot
{
    /// <summary>
    /// Global values.
    /// </summary>
    public static class Globals
    {
        //Kimimaru: We should link the data folder so that it's referenced from the executable dir, but .NET Core projects don't yet support it from the IDE
        //Do it manually via editing the .csproj file
        public static string RootDir => Environment.CurrentDirectory;
        public static readonly string DataPath = $"{Path.Combine(RootDir, $"Data{Path.DirectorySeparatorChar}")}";
        public const char CommandIdentifier = '!';
        public const char MacroIdentifier = '#';

        public const string LoginInfoFilename = "LoginInfo.txt";
        public const string SettingsFilename = "Settings.txt";
        public const string BotDataFilename = "BotData.txt";

        /// <summary>
        /// Kimimaru: The bot can only output 495 characters in a message.
        /// If it contains more, it might trim the end automatically (Ex. 'say' command) or not, for currently unknown reasons.
        /// </summary>
        public const int BotCharacterLimit = 495;

        public const int MinSleepTime = 1;
        public const int MaxSleepTime = 10000;

        public const string ChatBotPromptFilename = "ChatBotPrompt.txt";
        public const string ChatBotResponseFilename = "ChatBotResponse.txt";

        public static string GetDataFilePath(in string fileName)
        {
            return $"{DataPath}{fileName}";
        }

        /// <summary>
        /// Validates if the data path exists and creates it if not.
        /// </summary>
        /// <returns>true if the data path exists, false if the data path doesn't exist and cannot be created.</returns>
        public static bool ValidateDataPath()
        {
            if (Directory.Exists(DataPath) == false)
            {
                Console.WriteLine($"Data folder does not exist; creating");

                try
                {
                    Directory.CreateDirectory(DataPath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Unable to create data folder at \"{DataPath}\": {exception.Message}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to read text from a file in the data folder.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string containing the contents of the file. If the file doesn't exist, <see cref="string.Empty"/>.</returns>
        public static string ReadFromTextFile(in string fileName)
        {
            if (ValidateDataPath() == false)
            {
                return string.Empty;
            }

            string path = GetDataFilePath(fileName);

            //Try to read the file
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to read from file \"{fileName}\": {exception.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to read text from a file in the data folder. If the file doesn't exist, it is created.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string containing the contents of the file.</returns>
        public static string ReadFromTextFileOrCreate(in string fileName)
        {
            if (ValidateDataPath() == false)
            {
                return string.Empty;
            }

            string path = GetDataFilePath(fileName);
            if (File.Exists(path) == false)
            {
                //Create the file if it doesn't exist
                try
                {
                    File.WriteAllText(path, string.Empty);
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
                return File.ReadAllText(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to read from file \"{fileName}\": {exception.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Attempts to save text to a file in the data folder.
        /// </summary>
        /// <param name="fileName">The name of the file to save to.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <returns>true if the text was successfully saved, otherwise false.</returns>
        public static bool SaveToTextFile(in string fileName, string contents)
        {
            if (ValidateDataPath() == false)
            {
                return false;
            }

            string path = GetDataFilePath(fileName);
            try
            {
                File.WriteAllText(path, contents);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unable to save to file \"{fileName}\": {exception.Message}");
                return false;
            }
        }
    }
}
