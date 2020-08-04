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
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class BingoCommand : BaseCommand
    {
        /// <summary>
        /// The timeout for connecting to the bingo server.
        /// </summary>
        private const int BingoServerTimeout = 1000;

        public BingoCommand()
        {
            
        }

        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.VIP;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            if (BotProgram.BotSettings.BingoSettings.UseBingo == false)
            {
                BotProgram.MsgHandler.QueueMessage("The streamer is not currently running a bingo.");
                return;
            }

            string bingoTile = e.Command.ArgumentsAsString;
            
            //The user needs to send a bingo tile to the bot
            if (string.IsNullOrEmpty(bingoTile) == true || bingoTile.Length != 2)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"Bingo tile to mark (Ex. \"A1\")\"");
                return;
            }

            try
            {
                string pipeName = BotProgram.BotSettings.BingoSettings.BingoPipeFilePath;

                //Set up the pipe stream
                using (NamedPipeClientStream bingoClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    //Connect to the pipe or wait until it's available, with a timeout
                    //Console.WriteLine("Attempting to connect to chatbot socket...");
                    bingoClient.Connect(BingoServerTimeout);

                    //Send the data to the bingo server
                    using (StreamWriter bingoWriter = new StreamWriter(bingoClient, Encoding.UTF8))
                    {
                        bingoWriter.WriteLine(bingoTile);
                    }
                }
            }
            catch (Exception exc)
            {
                BotProgram.MsgHandler.QueueMessage($"Error with sending bingo message: {exc.Message}");
            }
        }
    }
}
