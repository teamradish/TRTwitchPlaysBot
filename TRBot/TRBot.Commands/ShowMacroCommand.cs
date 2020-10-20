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
    /// Shows the value of an input macro.
    /// </summary>
    public sealed class ShowMacroCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"#macroname\"";

        public ShowMacroCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string macroName = arguments[0].ToLowerInvariant();

            using BotDBContext context = DatabaseManager.OpenContext();

            InputMacro macro = context.Macros.FirstOrDefault(m => m.MacroName == macroName);

            string message = string.Empty;

            if (macro == null)
            {
                message = $"Macro \"{macroName}\" not found. For dynamic macros, use the generic form (Ex. \"#test(*)\"";
            }
            else
            {
                message = $"{macro.MacroName} = {macro.MacroValue}";
            }

            QueueMessage(message);
        }
    }
}
