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
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Gets or sets the global mid input delay's enabled state and duration.
    /// </summary>
    public sealed class MidInputDelayCommand : BaseCommand
    {
        private const long MIN_MID_INPUT_DELAY_DURATION = 1L;

        private string UsageMessage = "Usage: no arguments (get values), (\"enabled state (bool)\" and/or \"duration (int)\") (set values)";

        public MidInputDelayCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long midDelayEnabled = DataHelper.GetSettingInt(SettingsConstants.GLOBAL_MID_INPUT_DELAY_ENABLED, 0L);
            long midDelayTime = DataHelper.GetSettingInt(SettingsConstants.GLOBAL_MID_INPUT_DELAY_TIME, 34L);

            if (arguments.Count == 0)
            {
                string enabledStateStr = (midDelayEnabled > 0L) ? "enabled" : "disabled";

                QueueMessage($"The global mid input delay is {enabledStateStr} with a duration of {midDelayTime} milliseconds!");
                return;
            }

            if (arguments.Count > 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user has this ability
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user == null)
                {
                    QueueMessage("Somehow, the user calling this is not in the database.");
                    return;
                }

                if (user.HasEnabledAbility(PermissionConstants.SET_MID_INPUT_DELAY_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the global mid input delay!");
                    return;
                }
            }

            string enabledStr = arguments[0];
            string durationStr = arguments[0];

            bool parsedEnabled = false;
            bool parsedDuration = false;

            //Set the duration string to the second argument if we have more arguments
            if (arguments.Count == 2)
            {
                durationStr = arguments[1];
            }

            //Parse enabled state
            parsedEnabled = bool.TryParse(enabledStr, out bool enabledState);
            
            //Parse duration
            parsedDuration = long.TryParse(durationStr, out long newMidInputDelay);

            //Failed to parse enabled state
            if (parsedEnabled == false)
            {
                //Return if we're expecting two valid arguments or one valid argument and the duration parse failed 
                if (arguments.Count == 2 || (arguments.Count == 1 && parsedDuration == false))
                {
                    QueueMessage("Please enter \"true\" or \"false\" for the enabled state!");
                    return;
                }
            }

            if (parsedDuration == false)
            {
                //Return if we're expecting two valid arguments or one valid argument and the enabled parse failed 
                if (arguments.Count == 2 || (arguments.Count == 1 && parsedEnabled == false))
                {
                    QueueMessage("Please enter a valid number greater than 0!");
                    return;
                }
            }

            if (parsedDuration == true && newMidInputDelay < MIN_MID_INPUT_DELAY_DURATION)
            {
                QueueMessage($"The global mid input delay cannot be less than {MIN_MID_INPUT_DELAY_DURATION}!");
                return;
            }

            string message = "Set the global mid input delay";

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings midDelayEnabledSetting = DataHelper.GetSettingNoOpen(SettingsConstants.GLOBAL_MID_INPUT_DELAY_ENABLED, context);
                Settings midDelayTimeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.GLOBAL_MID_INPUT_DELAY_TIME, context);

                //Modify the message based on what was successfully parsed
                if (parsedEnabled == true)
                {
                    midDelayEnabledSetting.ValueInt = (enabledState == true) ? 1L : 0L;

                    message += " enabled state to " + enabledState;
                }

                if (parsedDuration == true)
                {
                    midDelayTimeSetting.ValueInt = newMidInputDelay;

                    //The enabled state was also parsed
                    if (parsedEnabled == true)
                    {
                        message += " and duration to " + newMidInputDelay;
                    }
                    else
                    {
                        message += " duration to " + newMidInputDelay + " milliseconds";
                    }
                }

                context.SaveChanges();
            }

            QueueMessage($"{message}!");
        }
    }
}
