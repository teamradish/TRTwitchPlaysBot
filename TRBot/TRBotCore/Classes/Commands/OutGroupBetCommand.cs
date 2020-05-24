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
    public sealed class OutGroupBetCommand : BaseCommand
    {
        public OutGroupBetCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            string displayLower = e.Command.ChatMessage.DisplayName.ToLower();

            if (GroupBetCommand.UsersInBet.ContainsKey(displayLower) == false)
            {
                BotProgram.QueueMessage("You're not in the group bet!");
                return;
            }
            else
            {
                long betAmt = GroupBetCommand.UsersInBet[displayLower];
                GroupBetCommand.UsersInBet.Remove(displayLower);

                BotProgram.QueueMessage($"{e.Command.ChatMessage.DisplayName} has backed out of the group bet and retained their {betAmt} credit(s)!");

                //Check for ending the group bet if there are no longer enough participants
                if (GroupBetCommand.BetStarted == true && GroupBetCommand.UsersInBet.Count < GroupBetCommand.MinUsersForBet)
                {
                    GroupBetCommand.StopGroupBet();

                    int count = GroupBetCommand.UsersInBet.Count;

                    BotProgram.QueueMessage($"Oh no! The group bet has ended since there are no longer enough participants. {GroupBetCommand.MinUsersForBet - count} more is/are required to start it up again!");
                }
            }
        }
    }
}
