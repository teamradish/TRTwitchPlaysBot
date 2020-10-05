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
    /// It will first look for the given from the database, and if not found, use the message as it is.
    /// </summary>
    public class MessageCommand : BaseCommand
    {
        public MessageCommand()
        {
            
        }

        public MessageCommand(string databaseMsgKey)
        {
            ValueStr = databaseMsgKey;
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string sentMessage = DataHelper.GetSettingString(ValueStr, ValueStr);

            //The message we want to send is null or empty
            if (string.IsNullOrEmpty(sentMessage) == true)
            {
                DataContainer.MessageHandler.QueueMessage("This command should say something, but the sent message is empty!");
                return;
            }

            List<string> textList = null;
            int charLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);
            
            Helpers.SplitStringWithinCharCount(sentMessage, charLimit, out textList);

            //If the text fits within the character limit, print it all out at once
            if (textList == null)
            {
                DataContainer.MessageHandler.QueueMessage(sentMessage);
            }
            else
            {
                //Otherwise, queue up the text in pieces
                for (int i = 0; i < textList.Count; i++)
                {
                    DataContainer.MessageHandler.QueueMessage(textList[i]);
                }
            }
        }
    }
}
