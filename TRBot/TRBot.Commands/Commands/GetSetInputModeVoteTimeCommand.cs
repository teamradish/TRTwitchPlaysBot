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
    /// Displays or changes the current vote time when changing the input mode.
    /// </summary>
    public sealed class GetSetInputModeVoteTimeCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"voting time (int) - in milliseconds\"";
        
        public GetSetInputModeVoteTimeCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long curVoteTime = DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE_VOTE_TIME, 60000L);

            //See the time
            if (arguments.Count == 0)
            {
                QueueMessage($"The current voting time for changing the input mode {curVoteTime}. To set the vote time, add it as an argument, in milliseconds.");
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

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_INPUT_MODE_VOTE_TIME_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to set the input mode voting time!");
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
            if (parsedTime <= 0)
            {
                QueueMessage($"{parsedTime} is less than or equal to 0! Consider setting it higher.");
                return;
            }
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Set the value and save
                Settings resModeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.INPUT_MODE_VOTE_TIME, context);
                resModeSetting.ValueInt = parsedTime;

                context.SaveChanges();
            }
            
            QueueMessage($"Changed the input mode voting time from {curVoteTime} to {parsedTime}!");
        }
    }
}
