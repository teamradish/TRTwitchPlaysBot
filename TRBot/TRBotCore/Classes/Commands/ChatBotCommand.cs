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
    public sealed class ChatBotCommand : BaseCommand
    {
        /// <summary>
        /// The response timeout for the chatbot.
        /// </summary>
        private const int ResponseTimeout = 1000;

        public ChatBotCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            if (BotProgram.BotSettings.UseChatBot == false)
            {
                BotProgram.MsgHandler.QueueMessage("The streamer is not currently using a chatbot!");
                return;
            }
            
            string question = e.Command.ArgumentsAsString;
            
            //The user needs to send a prompt to the bot
            if (string.IsNullOrEmpty(question) == true)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"prompt/question\"");
                return;
            }

            try
            {
                string pipeName = Globals.GetDataFilePath(BotProgram.BotSettings.ChatBotSocketFilename);

                //Set up the pipe stream
                using (NamedPipeClientStream chatterBotClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    //Connect to the pipe or wait until it's available, with a timeout
                    //Console.WriteLine("Attempting to connect to chatbot socket...");
                    chatterBotClient.Connect(ResponseTimeout);

                    //Send the input to ChatterBot
                    using (BinaryWriter promptWriter = new BinaryWriter(chatterBotClient))
                    {
                        using (BinaryReader responseReader = new BinaryReader(chatterBotClient))
                        {
                            //Get a byte array
                            byte[] byteBuffer = System.Text.Encoding.ASCII.GetBytes(question);
                            
                            //Send the data to the socket
                            promptWriter.Write((uint)byteBuffer.Length);
                            promptWriter.Write(byteBuffer);
                            
                            //Get the data back from the socket
                            uint responseLength = responseReader.ReadUInt32();
                            
                            //Console.WriteLine($"Response length: responseLength");
                            
                            string response = new string(responseReader.ReadChars((int)responseLength));
                            
                            //Console.WriteLine($"Received response: {response}");
                            //Output the response
                            BotProgram.MsgHandler.QueueMessage(response);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                BotProgram.MsgHandler.QueueMessage($"Error with sending chatbot reply: {exc.Message}");
            }
        }
    }
}
