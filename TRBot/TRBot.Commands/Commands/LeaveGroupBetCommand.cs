﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Routines;
using TRBot.Utilities;

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
            RoutineHandler.RemoveRoutine(RoutineConstants.GROUP_BET_ROUTINE_NAME);

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string username = args.Command.ChatMessage.Username.ToLowerInvariant();

            GroupBetRoutine groupBetRoutine = RoutineHandler.FindRoutine(RoutineConstants.GROUP_BET_ROUTINE_NAME) as GroupBetRoutine;

            if (groupBetRoutine == null 
                || groupBetRoutine.TryGetParticipant(username, out GroupBetRoutine.ParticipantData participantData) == false)
            {
                QueueMessage("You're not in the group bet!");
                return;
            }
            
            groupBetRoutine.RemoveParticipant(username);

            string creditsName = DataHelper.GetCreditsName();

            QueueMessage($"{username} has backed out of the group bet and retained their {participantData.ParticipantBet} {creditsName.Pluralize(participantData.ParticipantBet)}!");
            
            int participantCount = groupBetRoutine.ParticipantCount;
            int minParticipants = groupBetRoutine.MinParticipants;

            //Check for ending the group bet if there are no longer enough participants
            if (participantCount < minParticipants)
            {
                //If no one is in the group bet, end it entirely
                if (participantCount == 0)
                {
                    RoutineHandler.RemoveRoutine(RoutineConstants.GROUP_BET_ROUTINE_NAME);
                }

                QueueMessage($"Oh no! The group bet has ended since there are no longer enough participants. {minParticipants - participantCount} more is/are required to start it up again!");
            }
        }
    }
}
