/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
    /// Sets the maximum input duration.
    /// </summary>
    public sealed class MaxInputDurCommand : BaseCommand
    {
        private string UsageMessage = "Usage: no arguments (get value). \"duration (int)\" (set value)";

        public MaxInputDurCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int maxInputDur = (int)DataHelper.GetSettingInt(SettingsConstants.MAX_INPUT_DURATION, 60000L);

            if (arguments.Count == 0)
            {
                QueueMessage($"The maximum duration of an input sequence is {maxInputDur} milliseconds!");
                return;
            }

            if (arguments.Count > 1)
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

                if (user.HasEnabledAbility(PermissionConstants.SET_MAX_INPUT_DUR_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the global max input duration!");
                    return;
                }
            }

            if (int.TryParse(arguments[0], out int newMaxDur) == false)
            {
                QueueMessage("Please enter a valid number!");
                return;
            }

            if (newMaxDur < 0)
            {
                QueueMessage("Cannot set a negative duration!");
                return;
            }

            if (newMaxDur == maxInputDur)
            {
                QueueMessage("The duration is already this value!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings maxDurSetting = DataHelper.GetSettingNoOpen(SettingsConstants.MAX_INPUT_DURATION, context);
                maxDurSetting.ValueInt = newMaxDur;

                context.SaveChanges();
            }

            QueueMessage($"Set the maximum input sequence duration to {newMaxDur} milliseconds!");
        }
    }
}
