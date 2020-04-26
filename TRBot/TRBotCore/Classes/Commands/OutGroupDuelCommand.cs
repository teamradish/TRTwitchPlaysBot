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
    public sealed class OutGroupDuelCommand : BaseCommand
    {
        public OutGroupDuelCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string displayLower = e.Command.ChatMessage.DisplayName.ToLower();

            if (GroupDuelCommand.UsersInDuel.ContainsKey(displayLower) == false)
            {
                BotProgram.QueueMessage("You're not in the group duel!");
                return;
            }
            else
            {
                long betAmt = GroupDuelCommand.UsersInDuel[displayLower];
                GroupDuelCommand.UsersInDuel.Remove(displayLower);

                BotProgram.QueueMessage($"{e.Command.ChatMessage.DisplayName} has backed out of the group duel and retained their {betAmt} credit(s)!");

                //Check for ending the group duel if there are no longer enough participants
                if (GroupDuelCommand.DuelStarted == true && GroupDuelCommand.UsersInDuel.Count < GroupDuelCommand.MinUsersForDuel)
                {
                    GroupDuelCommand.StopGroupDuel();

                    int count = GroupDuelCommand.UsersInDuel.Count;

                    BotProgram.QueueMessage($"Oh no! The group duel has ended since there no longer enough participants. {GroupDuelCommand.MinUsersForDuel - count} more is/are required to start it up again!");
                }
            }
        }
    }
}
