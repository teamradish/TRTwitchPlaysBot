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
    public sealed class UptimeCommand : BaseCommand
    {
        public UptimeCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            TimeSpan timeSpan = BotProgram.TotalUptime;

            string message = $"The bot has been up for {timeSpan.Days} days, {timeSpan.Hours} hours, {timeSpan.Minutes} minutes, and {timeSpan.Seconds} seconds!";
            
            BotProgram.MsgHandler.QueueMessage(message);
        }
    }
}
