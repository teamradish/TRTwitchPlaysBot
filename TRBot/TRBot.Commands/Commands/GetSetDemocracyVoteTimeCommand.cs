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
using System.Text;
using TRBot.Connection;
using TRBot.VirtualControllers;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Misc;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Displays or changes the current vote time in the Democracy input mode.
    /// </summary>
    public sealed class GetSetDemocracyVoteTimeCommand : BaseCommand
    {
        private const long MIN_VOTING_TIME = 1000L;
        private const long MAX_VOTING_TIME_WARNING = 120000L;
        private string UsageMessage = "Usage: \"voting time (int) - in milliseconds\"";
        
        public GetSetDemocracyVoteTimeCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long curVoteTime = DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_VOTE_TIME, 10000L);

            //See the virtual controller
            if (arguments.Count == 0)
            {
                QueueMessage($"The current Democracy voting time is {curVoteTime}. To set the vote time, add it as an argument, in milliseconds.");
                return;
            }

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user has the ability to set the vote time
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_DEMOCRACY_VOTE_TIME_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to set the Democracy voting time!");
                    return;
                }
            }

            string numStr = arguments[0];

            //Parse
            if (long.TryParse(numStr, out long parsedTime) == false)
            {
                QueueMessage("Invalid number!");
                return;
            }

            //Same time
            if (curVoteTime == parsedTime)
            {
                QueueMessage($"The current voting time is already {curVoteTime}!");
                return;
            }

            //Check min value
            if (parsedTime < MIN_VOTING_TIME)
            {
                QueueMessage($"{parsedTime} is a very low voting time and may not be useful in the long run! Please set it to at least {MIN_VOTING_TIME} milliseconds.");
                return;
            }

            if (parsedTime > MAX_VOTING_TIME_WARNING)
            {
                QueueMessage($"{parsedTime} milliseconds is a long voting time that may slow down the stream. Consider setting the time lower than {MAX_VOTING_TIME_WARNING} milliseconds.");
            }
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Set the value and save
                Settings resModeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.DEMOCRACY_VOTE_TIME, context);
                resModeSetting.ValueInt = parsedTime;

                context.SaveChanges();
            }
            
            QueueMessage($"Changed the Democracy voting time from {curVoteTime} to {parsedTime}!");
        }
    }
}
