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
    /// <summary>
    /// Has the user opt in or out of any sort of bot stats, such as dueling, credits, and more.
    /// For simplicity, this currently doesn't factor in existing processes, such as ongoing duels or group bets.
    /// </summary>
    public sealed class OptStatsCommand : BaseCommand
    {
        public OptStatsCommand()
        {

        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            string name = e.Command.ChatMessage.DisplayName;
            string nameToLower = name.ToLower();

            User user = BotProgram.GetOrAddUser(nameToLower);
            
            if (user == null)
            {
                return;
            }

            if (user.OptedOut == false)
            {
                user.SetOptOut(true);

                BotProgram.SaveBotData();
                BotProgram.QueueMessage("Opted out of bot stats!");
                return;
            }
            else if (user.OptedOut == true)
            {
                user.SetOptOut(false);

                BotProgram.SaveBotData();
                BotProgram.QueueMessage("Opted back into bot stats!");
                return;
            }
        }
    }
}
