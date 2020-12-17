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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that gets or sets the max number of recent inputs stored per user.
    /// </summary>
    public sealed class GetSetMaxUserRecentInputsCommand : BaseCommand
    {
        private const int MIN_VALUE = 0;

        private string UsageMessage = $"Usage - no arguments (get value) or \"max # of inputs to store (int)\"";

        public GetSetMaxUserRecentInputsCommand()
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
                long maxRecentInputCount = DataHelper.GetSettingInt(SettingsConstants.MAX_USER_RECENT_INPUTS, 0L);

                QueueMessage($"The max number of recent inputs stored per user is {maxRecentInputCount}. To change this number, pass an integer as an argument."); 
                return;
            }

            //Check for sufficient permissions
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_MAX_USER_RECENT_INPUTS_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the number of recent inputs stored per user!");
                    return;
                }
            }

            string recentInputCountStr = arguments[0].ToLowerInvariant();

            if (int.TryParse(recentInputCountStr, out int newRecentInputCount) == false)
            {
                QueueMessage("That's not a valid number!");
                return;
            }

            if (newRecentInputCount < MIN_VALUE)
            {
                QueueMessage($"Value cannot be lower than {MIN_VALUE}!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings userMaxRecentInputs = DataHelper.GetSettingNoOpen(SettingsConstants.MAX_USER_RECENT_INPUTS, context);
                if (userMaxRecentInputs == null)
                {
                    userMaxRecentInputs = new Settings(SettingsConstants.MAX_USER_RECENT_INPUTS, string.Empty, 0L);
                    context.SettingCollection.Add(userMaxRecentInputs);
                }

                userMaxRecentInputs.ValueInt = newRecentInputCount;

                context.SaveChanges();
            }

            QueueMessage($"Set the max number of recent inputs stored per user to {newRecentInputCount}!"); 
        }
    }
}
