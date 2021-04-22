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

namespace TRBot.Commands
{
    /// <summary>
    /// Lists all available input macros.
    /// </summary>
    public sealed class ListMacrosCommand : BaseCommand
    {
        private const string NORMAL_ARG = "normal";
        private const string DYNAMIC_ARG = "dynamic";

        private string UsageMessage = "Usage: no arguments (all macros), \"normal\" (only normal macros), \"dynamic\" (only dynamic macros)";

        public ListMacrosCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string arg = string.Empty;
            
            if (arguments.Count > 0)
            {
                arg = arguments[0].ToLowerInvariant();
                
                //Validate argument
                if (arg != NORMAL_ARG && arg != DYNAMIC_ARG)
                {
                    QueueMessage(UsageMessage);
                    return;
                }
            }

            long lastConsole = DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);
            int maxCharCount = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            StringBuilder strBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                int macroCount = context.Macros.Count();

                if (macroCount == 0)
                {
                    QueueMessage("There are no input macros!");
                    return;
                }

                //The capacity is the estimated average number of characters for each macro multiplied by the number of macros
                strBuilder = new StringBuilder(macroCount * 15);

                //Sort alphabetically
                IOrderedQueryable<InputMacro> macros = context.Macros.OrderBy(m => m.MacroName);
            
                foreach (InputMacro macro in macros)
                {
                    //Skip macros according to the argument
                    switch (arg)
                    {
                        case NORMAL_ARG:
                            if (macro.MacroName.Contains('(') == true)
                            {
                                continue;
                            }
                        break;
                        case DYNAMIC_ARG:
                            if (macro.MacroName.Contains('(') == false)
                            {
                                continue;
                            }
                        break;
                    }

                    //Append the macro
                    strBuilder.Append(macro.MacroName).Append(',').Append(' ');
                }
            }

            //If no macros are available, mention it
            if (strBuilder.Length == 0)
            {
                QueueMessage("No input macros can be found with your argument!");
                return;
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
