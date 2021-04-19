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
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Gets a user's default input duration.
    /// </summary>
    public sealed class UserDefaultInputDurCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username (string)\" - pass no arguments for your own value";

        public UserDefaultInputDurCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = args.Command.ChatMessage.Username;

            if (arguments.Count > 0)
            {
                userName = arguments[0];
            }

            long value = DataHelper.GetUserOrGlobalDefaultInputDur(userName);
            QueueMessage($"{userName}'s default input duration is {value} milliseconds!");
        }
    }
}
