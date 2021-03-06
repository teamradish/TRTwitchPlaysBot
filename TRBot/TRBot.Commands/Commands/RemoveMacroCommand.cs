﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Removes an input macro.
    /// </summary>
    public sealed class RemoveMacroCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"#macroname\"";

        public RemoveMacroCommand()
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

            string userName = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.REMOVE_INPUT_MACRO_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to remove input macros.");
                    return;
                }
            }

            string macroName = arguments[0].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                InputMacro macro = context.Macros.FirstOrDefault(m => m.MacroName == macroName);

                if (macro == null)
                {
                    QueueMessage($"Input macro \"{macroName}\" could not be found.");
                    return;
                }

                context.Macros.Remove(macro);
                context.SaveChanges();
            }

            QueueMessage($"Removed input macro \"{macroName}\".");
        }
    }
}
