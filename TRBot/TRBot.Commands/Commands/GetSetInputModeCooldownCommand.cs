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
    /// Displays or changes the cooldown for voting after the input mode was changed.
    /// </summary>
    public sealed class GetSetInputModeCooldownCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"cooldown (int) - in milliseconds\"";
        
        public GetSetInputModeCooldownCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long curCooldown = DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE_CHANGE_COOLDOWN, 1000L * 60L * 15L);

            //See the time
            if (arguments.Count == 0)
            {
                QueueMessage($"The current post-voting cooldown for changing the input mode is {curCooldown}. To set the cooldown, add it as an argument, in milliseconds.");
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
                //Check if the user has the ability to set the cooldown
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_INPUT_MODE_CHANGE_COOLDOWN_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to set the input mode post-voting cooldown!");
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
            if (curCooldown == parsedTime)
            {
                QueueMessage($"The current cooldown is already {curCooldown}!");
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
                Settings resModeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.INPUT_MODE_CHANGE_COOLDOWN, context);
                resModeSetting.ValueInt = parsedTime;

                context.SaveChanges();
            }
            
            QueueMessage($"Changed the post-voting cooldown time from {curCooldown} to {parsedTime}!");
        }
    }
}
