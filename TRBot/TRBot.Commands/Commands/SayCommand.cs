/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
    /// Makes the bot repeat the message given to it.
    /// </summary>
    public class SayCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"text to say\"";

        public SayCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string message = args.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(message) == true)
            {
                QueueMessage(UsageMessage);
                return;
            }

            ClientServiceTypes clientServiceType = DataHelper.GetClientServiceType();

            //Ignore any output starting with a "/" to avoid exploiting Twitch chat commands
            if (clientServiceType == ClientServiceTypes.Twitch && message.StartsWith('/') == true)
            {
                QueueMessage("I can't say any Twitch chat commands for you - no hard feelings!");
                return;
            }

            QueueMessage(message);
        }
    }
}
