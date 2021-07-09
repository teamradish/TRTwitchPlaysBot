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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// Allows users to interact with an external chatbot.
    /// </summary>
    public sealed class ChatbotCommand : BaseCommand
    {
        /// <summary>
        /// The response timeout for the chatbot.
        /// </summary>
        private const int RESPONSE_TIMEOUT = 1000;

        private string UsageMessage = "Usage: \"prompt or question (string)\"";

        public ChatbotCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            //Check if the chatbot is enabled
            long chatbotEnabled = DataHelper.GetSettingInt(SettingsConstants.CHATBOT_ENABLED, 0L);

            if (chatbotEnabled != 1)
            {
                QueueMessage("The streamer is not currently using a chatbot!");
                return;
            }
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user has the ability to chat with the chatbot
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.CHATBOT_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to chat with the chatbot.");
                    return;
                }
            }

            string question = args.Command.ArgumentsAsString;
            
            //The user needs to send a prompt to the bot
            if (string.IsNullOrEmpty(question) == true)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string chatbotHostName = DataHelper.GetSettingString(SettingsConstants.CHATBOT_SOCKET_HOSTNAME, "127.0.0.1");
            int chatbotPort = (int)DataHelper.GetSettingInt(SettingsConstants.CHATBOT_SOCKET_PORT, 7444);
            
            try
            {
                TRBotLogger.Logger.Debug($"Full chatbot address: {chatbotHostName}:{chatbotPort}");

                //Set up the stream by connecting over TCP
                using (TcpClient client = new TcpClient(chatbotHostName, chatbotPort))
                {
                    using (NetworkStream nStream = client.GetStream())
                    {
                        //Send the input to ChatterBot
                        using (BinaryWriter promptWriter = new BinaryWriter(nStream ))
                        {
                            using (BinaryReader responseReader = new BinaryReader(nStream))
                            {
                                //Get a byte array
                                byte[] byteBuffer = System.Text.Encoding.ASCII.GetBytes(question);

                                //Send the data to the socket
                                promptWriter.Write((uint)byteBuffer.Length);
                                promptWriter.Write(byteBuffer);

                                //Get the data back from the socket
                                uint responseLength = responseReader.ReadUInt32();

                                TRBotLogger.Logger.Debug($"Response length: {responseLength}");

                                string response = new string(responseReader.ReadChars((int)responseLength));

                                TRBotLogger.Logger.Debug($"Received response: {response}");

                                //Output the response
                                QueueMessage(response);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                QueueMessage($"Error with sending chatbot reply. {exc.Message} - Please check the \"{SettingsConstants.CHATBOT_SOCKET_HOSTNAME}\" and \"{SettingsConstants.CHATBOT_SOCKET_PORT}\" settings in the database. Also ensure your ChatterBot instance is running!", Serilog.Events.LogEventLevel.Warning);
            }
        }
    }
}
