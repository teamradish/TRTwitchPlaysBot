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
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;

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

            using BotDBContext context = DatabaseManager.OpenContext();

            Settings defaultDurSetting = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.DEFAULT_INPUT_DURATION);

            int defaultInputDur = (int)defaultDurSetting.value_int;

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

            if (int.TryParse(arguments[0], out int newDefaultDur) == false)
            {
                QueueMessage("Please enter a valid number!");
                return;
            }

            if (newDefaultDur < 0)
            {
                QueueMessage("Cannot set a negative duration!");
                return;
            }

            if (newDefaultDur == defaultInputDur)
            {
                QueueMessage("The duration is already this value!");
                return;
            }

            defaultDurSetting.value_int = newDefaultDur;
            
            context.SaveChanges();

            QueueMessage($"Set the default input duration to {newDefaultDur} milliseconds!");
        }
    }
}
