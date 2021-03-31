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
    /// Sets the default input duration.
    /// </summary>
    public sealed class DefaultInputDurCommand : BaseCommand
    {
        private string UsageMessage = "Usage: no arguments (get value). \"duration (int)\" (set value)";

        public DefaultInputDurCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int defaultInputDur = (int)DataHelper.GetSettingInt(SettingsConstants.DEFAULT_INPUT_DURATION, 200L);

            if (arguments.Count == 0)
            {
                QueueMessage($"The default duration of an input is {defaultInputDur} milliseconds!");
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

                if (user.HasEnabledAbility(PermissionConstants.SET_DEFAULT_INPUT_DUR_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the global default input duration!");
                    return;
                }
            }

            if (int.TryParse(arguments[0], out int newDefaultDur) == false)
            {
                QueueMessage("Please enter a valid number!");
                return;
            }

            if (newDefaultDur <= 0)
            {
                QueueMessage("Cannot set a duration less than or equal to 0!");
                return;
            }

            if (newDefaultDur == defaultInputDur)
            {
                QueueMessage("The duration is already this value!");
                return;
            }

            //Change the setting
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings defaultDurSetting = DataHelper.GetSettingNoOpen(SettingsConstants.DEFAULT_INPUT_DURATION, context);

                defaultDurSetting.ValueInt = newDefaultDur;

                context.SaveChanges();
            }

            QueueMessage($"Set the default input duration to {newDefaultDur} milliseconds!");
        }
    }
}
