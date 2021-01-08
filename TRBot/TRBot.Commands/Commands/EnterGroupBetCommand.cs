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
using TRBot.Routines;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Allows a user to enter a group bet for a chance to win credits.
    /// </summary>
    public class EnterGroupBetCommand : BaseCommand
    {
        public EnterGroupBetCommand()
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
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 1)
            {
                QueueMessage("Please specify a bet amount!");
                return;
            }

            string creditsName = DataHelper.GetCreditsName();
            string userName = args.Command.ChatMessage.Username;

            //Check if we can parse the bet amount
            long betAmount = -1L;
            bool success = long.TryParse(arguments[0], out betAmount);
            if (success == false || betAmount <= 0)
            {
                QueueMessage($"Please specify a positive whole number of {creditsName.Pluralize(false, 0)} greater than 0!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user exists
                User user = DataHelper.GetUserNoOpen(userName, context);
                if (user == null)
                {
                    QueueMessage("The user calling this does not exist in the database!");
                    return;
                }

                //No ability to enter group bets
                if (user.HasEnabledAbility(PermissionConstants.GROUP_BET_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to participate in a group bet!");
                    return;
                }

                //Validate credit amount
                if (user.Stats.Credits < betAmount)
                {
                    QueueMessage($"You don't have enough {creditsName.Pluralize(false, 0)} to bet this much!");
                    return;
                }

                if (user.IsOptedOut == true)
                {
                    QueueMessage("You can't participate in the group bet since you opted out of bot stats.");
                    return;
                }
            }

            //Get total time and minimum participants required
            long groupBetTime = DataHelper.GetSettingInt(SettingsConstants.GROUP_BET_TOTAL_TIME, 120000L);
            int groupBetMinUsers = (int)DataHelper.GetSettingInt(SettingsConstants.GROUP_BET_MIN_PARTICIPANTS, 3L);

            //Get the routine
            GroupBetRoutine groupBetRoutine = RoutineHandler.FindRoutine<GroupBetRoutine>();

            //We haven't started the group bet, so start it up
            if (groupBetRoutine == null)
            {
                groupBetRoutine = new GroupBetRoutine(groupBetTime, groupBetMinUsers);
                RoutineHandler.AddRoutine(groupBetRoutine);
            }

            //Note: From hereon, use the routine's total time and min participant values
            //The database values could have changed in between calls of this command
            //and thus wouldn't be applicable to the group bet created for the first participant

            //See if the user is already in the group bet
            bool prevParticipant = groupBetRoutine.TryGetParticipant(userName,
                out GroupBetRoutine.ParticipantData participantData);

            groupBetRoutine.AddOrUpdateParticipant(userName, betAmount);

            string message = string.Empty;

            //Newly added since they were not previously there
            if (prevParticipant == false)
            {
                message = $"{userName} entered the group bet with {betAmount} {creditsName.Pluralize(false, betAmount)}!";
                
                int participantCount = groupBetRoutine.ParticipantCount;
                
                if (participantCount < groupBetRoutine.MinParticipants)
                {
                    int diff = groupBetRoutine.MinParticipants - groupBetRoutine.ParticipantCount;
                    message += $" {diff} more user(s) are required to start the group bet!";
                }

                QueueMessage(message);

                //Check if we have enough participants now
                if (participantCount == groupBetRoutine.MinParticipants)
                {
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(groupBetRoutine.MillisecondsForBet);

                    QueueMessage($"The group bet has enough participants and will start in {timeSpan.Minutes} minute(s) and {timeSpan.Seconds} second(s), so join before then if you want in!");
                }
            }
            else
            {
                QueueMessage($"{userName} adjusted their group bet from {participantData.ParticipantBet} to {betAmount} {creditsName.Pluralize(false, betAmount)}!");
            }
        }
    }
}
