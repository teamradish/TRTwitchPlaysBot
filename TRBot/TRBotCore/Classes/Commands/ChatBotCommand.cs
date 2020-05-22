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
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class ChatBotCommand : BaseCommand
    {
        public ChatBotCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            
        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotSettings.UseChatBot == false)
            {
                BotProgram.QueueMessage("The streamer is not currently using a chatbot!");
                return;
            }
            
            string question = e.Command.ArgumentsAsString;
            
            //The user needs to send a prompt to the bot
            if (string.IsNullOrEmpty(question) == true)
            {
                BotProgram.QueueMessage("Usage: \"prompt/question\"");
                return;
            }

            if (Globals.SaveToTextFile(Globals.ChatBotPromptFilename, question) == false)
            {
                BotProgram.QueueMessage("Error saving question to prompt file.");
                return;
            }
        }
    }
}
