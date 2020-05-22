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
    public sealed class DenyCommand : BaseCommand
    {
        public DenyCommand()
        {

        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            string name = e.Command.ChatMessage.DisplayName;
            string nameToLower = name.ToLower();

            if (DuelCommand.DuelRequests.ContainsKey(nameToLower) == true)
            {
                DuelCommand.DuelData data = DuelCommand.DuelRequests[nameToLower];
                DuelCommand.DuelRequests.Remove(nameToLower);

                TimeSpan diff = DateTime.Now - data.CurDuelTime;

                if (diff.TotalMinutes >= DuelCommand.DUEL_MINUTES)
                {
                    BotProgram.QueueMessage("You are not in a duel or your duel has expired!");
                    return;
                }

                BotProgram.QueueMessage($"{name} has denied to duel with {data.UserDueling} and miss out on a potential {data.BetAmount} credit(s)!");
            }
            else
            {
                BotProgram.QueueMessage("You are not in a duel or your duel has expired!");
            }
        }
    }
}
