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
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that changes the periodic input interval.
    /// </summary>
    public sealed class GetSetPeriodicInputTimeCommand : BaseCommand
    {
        private const int MIN_INTERVAL_VAL = 1;
        private const int RECOMMENDED_MIN_VAL = 30000;

        private string UsageMessage = $"Usage - no arguments (get value) or \"interval in milliseconds (int)\"";

        public GetSetPeriodicInputTimeCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Ignore with too few arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            if (arguments.Count == 0)
            {
                long periodicInputTime = DataHelper.GetSettingInt(SettingsConstants.PERIODIC_INPUT_TIME, 0L);

                QueueMessage($"The interval for periodic inputs is {periodicInputTime} milliseconds. To change the periodic input interval, a value, in milliseconds, as an argument."); 
                return;
            }

            //Check for sufficient permissions
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_PERIODIC_INPUT_TIME_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the default periodic input interval!");
                    return;
                }
            }

            string intervalValStr = arguments[0].ToLowerInvariant();

            if (int.TryParse(intervalValStr, out int newInterval) == false)
            {
                QueueMessage("That's not a valid number!");
                return;
            }

            if (newInterval < MIN_INTERVAL_VAL)
            {
                string timePluralized = "milliseconds";
                timePluralized = timePluralized.Pluralize(false, MIN_INTERVAL_VAL);

                QueueMessage($"The interval cannot be below {MIN_INTERVAL_VAL} {timePluralized}!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings periodicInputTime = DataHelper.GetSettingNoOpen(SettingsConstants.PERIODIC_INPUT_TIME, context);
                if (periodicInputTime == null)
                {
                    periodicInputTime = new Settings(SettingsConstants.PERIODIC_INPUT_TIME, string.Empty, 0L);
                    context.SettingCollection.Add(periodicInputTime);
                }

                periodicInputTime.ValueInt = newInterval;

                context.SaveChanges();
            }

            string intervalPluralized = "milliseconds";
            intervalPluralized = intervalPluralized.Pluralize(false, newInterval);

            QueueMessage($"Set the periodic input interval to {newInterval} {intervalPluralized}!");
            
            //If the interval is less than a given amount, recommend a higher value so the message doesn't spam chat and interfere with inputs
            if (newInterval < RECOMMENDED_MIN_VAL)
            {
                QueueMessage($"The set interval is lower than {RECOMMENDED_MIN_VAL}. Keep in mind that it may spam chat and interfere with player inputs, so consider setting it higher."); 
            }
        }
    }
}
