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
using TRBot.Connection;
using TRBot.Misc;
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
                QueueMessage("This command should say something, but the sent message is empty!");
                return;
            }

            //Replace any first parameters with the user's name
            sentMessage = sentMessage.Replace("{0}", args.Command.ChatMessage.Username);

            int charLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);
            
            QueueMessageSplit(sentMessage, charLimit, string.Empty);
        }
    }
}
