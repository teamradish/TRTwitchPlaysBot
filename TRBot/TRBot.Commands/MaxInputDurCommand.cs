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
using TRBot.ParserData;
using TRBot.Parsing;
using TRBot.Data;

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

            using BotDBContext context = DatabaseManager.OpenContext();

            Settings maxDurSetting = context.SettingCollection.FirstOrDefault(set => set.key == SettingsConstants.MAX_INPUT_DURATION);

            int maxInputDur = (int)maxDurSetting.value_int;

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

            maxDurSetting.value_int = newMaxDur;
            
            context.SaveChanges();

            QueueMessage($"Set the maximum input sequence duration to {newMaxDur} milliseconds!");
        }
    }
}
