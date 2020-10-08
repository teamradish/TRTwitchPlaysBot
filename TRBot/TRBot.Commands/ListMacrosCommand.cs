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
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.ParserData;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Lists all available input macros.
    /// </summary>
    public sealed class ListMacrosCommand : BaseCommand
    {
        public ListMacrosCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            long lastConsole = DataHelper.GetSettingIntNoOpen(SettingsConstants.LAST_CONSOLE, context, 1L);
            int maxCharCount = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.BOT_MSG_CHAR_LIMIT, context, 500L);

            int macroCount = context.Macros.Count();

            if (macroCount == 0)
            {
                QueueMessage("There are no macros!");
                return;
            }

            //The capacity is the estimated average number of characters for each macro multiplied by the number of macros
            StringBuilder strBuilder = new StringBuilder(macroCount * 15);

            //Sort alphabetically
            IOrderedQueryable<InputMacro> macros = context.Macros.OrderBy(m => m.MacroName);

            foreach (InputMacro macro in macros)
            {
                strBuilder.Append(macro.MacroName).Append(',').Append(' ');
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
