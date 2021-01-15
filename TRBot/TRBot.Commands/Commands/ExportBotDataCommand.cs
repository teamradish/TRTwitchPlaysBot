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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Exports a copy of all the bot data.
    /// <para>It's highly recommended to have this accessible ONLY to the streamer.</para>  
    /// </summary>
    public sealed class ExportBotDataCommand : BaseCommand
    {
        private const string CONFIRMATION_ARG = "confirm";

        public ExportBotDataCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 1)
            {
                QueueMessage($"Enter \"{CONFIRMATION_ARG}\" as an argument to confirm exporting a copy of the bot data.");
                return;
            }

            string arg = arguments[0];

            if (arg != CONFIRMATION_ARG)
            {
                QueueMessage($"Enter \"{CONFIRMATION_ARG}\" as an argument to confirm exporting a copy of the bot data.");
                return;
            }

            try
            {
                //Check if the database file exists
                if (File.Exists(DatabaseManager.DatabasePath) == false)
                {
                    QueueMessage("Huh?! The database file doesn't exist! This is a serious problem!", Serilog.Events.LogEventLevel.Error);
                    return;
                }

                string timeStamp = Debug.GetFileFriendlyTimeStamp();
                
                //Use the time stamp in the folder name
                string dirPath = Path.Combine(DataConstants.DataFolderPath, $"TRBot Data Export ({timeStamp})");
                
                //Double check if the directory exists - don't overwrite anything if it does
                if (Directory.Exists(dirPath) == true)
                {
                    QueueMessage("ERROR - Path for bot data export already exists, not overwriting.", Serilog.Events.LogEventLevel.Warning);
                    return;
                }

                string destinationPath = Path.Combine(dirPath, DataConstants.DATABASE_FILE_NAME);

                //Create directory
                Directory.CreateDirectory(dirPath);

                //Copy the database file to the new folder
                File.Copy(DatabaseManager.DatabasePath, destinationPath, false);
            }
            catch (Exception exc)
            {
                QueueMessage($"ERROR - Failed to export bot data: {exc.Message}", Serilog.Events.LogEventLevel.Error);
                return;
            }

            QueueMessage("Successfully backed up bot data!");
        }
    }
}
