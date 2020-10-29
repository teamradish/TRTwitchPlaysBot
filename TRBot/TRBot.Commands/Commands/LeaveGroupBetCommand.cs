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
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Routines;

namespace TRBot.Commands
{
    /// <summary>
    /// Allows a user to leave a group bet.
    /// </summary>
    public class LeaveGroupBetCommand : BaseCommand
    {
        public LeaveGroupBetCommand()
        {

        }

        public override void CleanUp()
        {
            //Remove the group bet routine if it's active
            RoutineHandler.RemoveRoutine(RoutineConstants.GROUP_BET_ROUTINE_ID);

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string username = args.Command.ChatMessage.Username.ToLowerInvariant();

            GroupBetRoutine groupBetRoutine = RoutineHandler.FindRoutine<GroupBetRoutine>();

            if (groupBetRoutine == null 
                || groupBetRoutine.TryGetParticipant(username, out GroupBetRoutine.ParticipantData participantData) == false)
            {
                QueueMessage("You're not in the group bet!");
                return;
            }
            
            groupBetRoutine.RemoveParticipant(username);

            string creditsName = DataHelper.GetCreditsName();

            QueueMessage($"{username} has backed out of the group bet and retained their {participantData.ParticipantBet} {creditsName}!");
            
            int participantCount = groupBetRoutine.ParticipantCount;
            int minParticipants = groupBetRoutine.MinParticipants;

            //Check for ending the group bet if there are no longer enough participants
            if (participantCount < minParticipants)
            {
                //If no one is in the group bet, end it entirely
                if (participantCount == 0)
                {
                    RoutineHandler.RemoveRoutine(groupBetRoutine);
                }

                QueueMessage($"Oh no! The group bet has ended since there are no longer enough participants. {minParticipants - participantCount} more is/are required to start it up again!");
            }
        }
    }
}
