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
    /// Lists all permission abilities.
    /// </summary>
    public sealed class ListPermissionAbilitiesCommand : BaseCommand
    {
        public ListPermissionAbilitiesCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            StringBuilder strBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //No abilities
                if (context.PermAbilities.Count() == 0)
                {
                    QueueMessage("There are no permission abilities!");
                    return;
                }

                //Order alphabetically
                IOrderedQueryable<PermissionAbility> permAbilities = context.PermAbilities.OrderBy(p => p.Name);

                strBuilder = new StringBuilder(250);
                strBuilder.Append("Hi, ").Append(args.Command.ChatMessage.Username);
                strBuilder.Append(", here's a list of all permission abilities: ");

                foreach (PermissionAbility pAbility in permAbilities)
                {
                    strBuilder.Append(pAbility.Name);
                    strBuilder.Append(',').Append(' ');
                }
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            int maxCharCount = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
