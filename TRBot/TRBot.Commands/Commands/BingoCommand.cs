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
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Allows users to interact with an external bingo application.
    /// </summary>
    public class BingoCommand : BaseCommand
    {
        /// <summary>
        /// The timeout for connecting to the bingo server.
        /// </summary>
        private const int BINGO_SERVER_TIMEOUT = 1000;

        private string UsageMessage = "Usage: \"Bingo tile to mark (Ex. \"A1\")\"";

        public BingoCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            //Check if bingo is enabled
            long bingoEnabled = DataHelper.GetSettingIntNoOpen(SettingsConstants.BINGO_ENABLED, context, 0L);

            if (bingoEnabled != 1)
            {
                QueueMessage("The streamer is not currently running a bingo!");
                return;
            }

            //Check if the user has the ability to play bingo
            User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

            if (user != null && user.HasAbility(PermissionConstants.BINGO_ABILITY) == false)
            {
                QueueMessage("You do not have the ability to play bingo.");
                return;
            }

            string bingoTile = args.Command.ArgumentsAsString;
            
            //The user needs to send a bingo tile to the bot
            if (string.IsNullOrEmpty(bingoTile) == true || bingoTile.Length != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            long bingoPipePathIsRelative = DataHelper.GetSettingIntNoOpen(SettingsConstants.BINGO_PIPE_PATH_IS_RELATIVE, context, 1L);

            string fileName = DataHelper.GetSettingStringNoOpen(SettingsConstants.BINGO_PIPE_PATH, context, string.Empty);

            try
            {
                string bingoPipePath = fileName;

                //Get relative path if we should
                if (bingoPipePathIsRelative == 1)
                {
                    bingoPipePath = Path.Combine(DataConstants.DataFolderPath, fileName);
                }

                //Set up the pipe stream
                using (NamedPipeClientStream bingoClient = new NamedPipeClientStream(".", bingoPipePath, PipeDirection.Out))
                {
                    //Connect to the pipe or wait until it's available, with a timeout
                    //Console.WriteLine("Attempting to connect to bingo pipe...");
                    bingoClient.Connect(BINGO_SERVER_TIMEOUT);

                    //Send the data to the bingo server
                    using (StreamWriter bingoWriter = new StreamWriter(bingoClient, Encoding.UTF8))
                    {
                        bingoWriter.WriteLine(bingoTile);
                    }
                }
            }
            catch (Exception exc)
            {
                QueueMessage($"Error with sending bingo message: {exc.Message} - Please check the \"{SettingsConstants.BINGO_PIPE_PATH}\" and \"{SettingsConstants.BINGO_PIPE_PATH_IS_RELATIVE}\" settings in the database.");
            }
        }
    }
}
