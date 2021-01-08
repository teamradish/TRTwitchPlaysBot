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
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Reverses a message.
    /// </summary>
    public class ReverseCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"text to reverse\"";

        public ReverseCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count == 0)
            {
                QueueMessage(UsageMessage);
                return;
            }

            char[] msgArr = args.Command.ArgumentsAsString.ToCharArray();
            Array.Reverse(msgArr);
            
            string msg = new string(msgArr);
            
            ClientServiceTypes clientServiceType = DataHelper.GetClientServiceType();

            //Ignore any output starting with a "/" to avoid exploiting Twitch chat commands
            if (clientServiceType == ClientServiceTypes.Twitch && msg.StartsWith('/') == true)
            {
                QueueMessage("!uoy rof sdnammoc tahc hctiwT oN");
                return;
            }

            QueueMessage(msg);
        }
    }
}
