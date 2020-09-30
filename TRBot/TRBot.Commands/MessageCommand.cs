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
using TRBot.Connection;
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// A simple command that sends a message.
    /// It can also use a message from the database.
    /// </summary>
    public class MessageCommand : BaseCommand
    {
        public string DatabaseMessageKey = string.Empty;
        public string FallbackMessage = string.Empty;
        protected BotMessageHandler MessageHandler = null;

        public MessageCommand()
        {
            
        }

        public MessageCommand(string databaseMsgKey, string fallbackMessage)
        {
            DatabaseMessageKey = databaseMsgKey;
            FallbackMessage = fallbackMessage;
        }

        public override void Initialize(BotMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }

        public override void CleanUp()
        {
            MessageHandler = null;
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string sentMessage = FallbackMessage;

            //If the database message key is defined, fetch the message from the database
            if (string.IsNullOrEmpty(DatabaseMessageKey) == false)
            {
                sentMessage = DataHelper.GetSettingString(DatabaseMessageKey, FallbackMessage);
            }

            //The message we want to send is null or empty
            if (string.IsNullOrEmpty(sentMessage) == true)
            {
                MessageHandler.QueueMessage("This command should say something, but the sent message is empty!");
                return;
            }

            List<string> textList = null;
            int charLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);
            
            Helpers.SplitStringWithinCharCount(sentMessage, charLimit, out textList);

            //If the text fits within the character limit, print it all out at once
            if (textList == null)
            {
                MessageHandler.QueueMessage(sentMessage);
            }
            else
            {
                //Otherwise, queue up the text in pieces
                for (int i = 0; i < textList.Count; i++)
                {
                    MessageHandler.QueueMessage(textList[i]);
                }
            }
        }
    }
}
