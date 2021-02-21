/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.Globalization;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that lists all inputs in the invalid input combo for a console.
    /// </summary>
    public sealed class ListInvalidInputComboCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name (string)\"";

        public ListInvalidInputComboCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleStr = arguments[0].ToLowerInvariant();
            StringBuilder strBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);

                if (console == null)
                {
                    QueueMessage($"No console named \"{consoleStr}\" found.");
                    return;
                }

                List<InvalidCombo> invalidComboList = console.InvalidCombos;
                if (invalidComboList.Count == 0)
                {
                    QueueMessage($"Console \"{consoleStr}\" does not have an invalid input combo.");
                    return;
                }

                strBuilder = new StringBuilder(128);
                strBuilder.Append("The invalid input combo for console \"").Append(consoleStr).Append("\" is (");
                for (int i = 0; i < invalidComboList.Count; i++)
                {
                    strBuilder.Append('"').Append(invalidComboList[i].Input.Name).Append('"');
                    if (i < (invalidComboList.Count - 1))
                    {
                        strBuilder.Append(',').Append(' ');
                    }
                }

                strBuilder.Append("). These inputs are not allowed to be pressed at once on the same controller port.");
            }

            int botCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(strBuilder.ToString(), botCharLimit, ", ");
        }
    }
}
