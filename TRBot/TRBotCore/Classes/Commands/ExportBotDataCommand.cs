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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Exports a copy of all the bot data.
    /// <para>Keep this command restricted to admins since it puts the data on the computer the bot is being run on.</para>  
    /// </summary>
    public sealed class ExportBotDataCommand : BaseCommand
    {
        private const string ConfirmationArg = "confirm";

        public ExportBotDataCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"Enter \"{ConfirmationArg}\" as an argument to confirm exporting a copy of the bot data.");
                return;
            }

            string arg = args[0];

            if (arg != ConfirmationArg)
            {
                BotProgram.QueueMessage($"Enter \"{ConfirmationArg}\" as an argument to confirm exporting a copy of the bot data.");
                return;
            }

            //Handle lock on the global bot data object to avoid changing any data while this is going on
            lock (Globals.BotDataLockObj)
            {
                try
                {
                    string timeStamp = Debug.DebugGlobals.GetFileFriendlyTimeStamp();

                    //Use the time stamp in the folder name
                    string dirPath = Path.Combine(Globals.DataPath, $"TRBot Data Export ({timeStamp})");

                    //Double check if the directory exists - don't overwrite anything if it does
                    if (Directory.Exists(dirPath) == true)
                    {
                        BotProgram.QueueMessage("ERROR - Path for bot data export already exists, not overwriting.");
                        return;
                    }

                    //Create directory
                    Directory.CreateDirectory(dirPath);

                    //Copy each file's data and save to new files in the new folder
                    GetAndWriteFile(dirPath, Globals.BotDataFilename);
                    GetAndWriteFile(dirPath, Globals.SettingsFilename);
                    GetAndWriteFile(dirPath, Globals.LoginInfoFilename);
                    GetAndWriteFile(dirPath, Globals.InputCallbacksFileName);
                    GetAndWriteFile(dirPath, Globals.AchievementsFilename);
                    GetAndWriteFile(dirPath, Globals.GameMessageFilename);
                }
                catch (Exception exc)
                {
                    BotProgram.QueueMessage($"ERROR - Failed to export bot data: {exc.Message}");
                    return;
                }
            }
        }

        private void GetAndWriteFile(in string dirPath, in string fileName)
        {
            string dataText = Globals.ReadFromTextFile(fileName);

            if (string.IsNullOrEmpty(dataText) == false)
                File.WriteAllText(Path.Combine(dirPath, fileName), dataText);
        }
    }
}
