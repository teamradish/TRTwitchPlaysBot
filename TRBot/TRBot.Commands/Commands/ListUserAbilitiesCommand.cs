/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
    /// Lists a user's abilities.
    /// </summary>
    public sealed class ListUserAbilitiesCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username\"";

        public ListUserAbilitiesCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //This supports listing another user's abilities if provided as an argument, but only one user at a time
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Get the user calling this
            string thisUserName = args.Command.ChatMessage.Username.ToLowerInvariant();
            string listedName = thisUserName;

            //Check to list another user's abilities
            if (arguments.Count == 1)
            {
                listedName = arguments[0].ToLowerInvariant();
            }

            StringBuilder strBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(listedName, context);

                if (user == null)
                {
                    QueueMessage("A user with this name does not exist in the database!");
                    return;
                }

                //No abilities
                if (user.UserAbilities.Count == 0)
                {
                    QueueMessage($"{user.Name} has no abilities!");
                    return;
                }

                DateTime now = DateTime.UtcNow;

                strBuilder = new StringBuilder(250);

                strBuilder.Append(user.Name).Append("'s abilities: ");

                for (int i = 0; i < user.UserAbilities.Count; i++)
                {
                    UserAbility ability = user.UserAbilities[i];

                    //if (ability.IsEnabled == false && ability.HasExpiration == false)
                    //{
                    //    continue;
                    //}

                    strBuilder.Append(ability.PermAbility.Name);

                    if (ability.IsEnabled == false)
                    {
                        strBuilder.Append(" (disabled)");
                    }

                    if (ability.HasExpired == false && ability.HasExpiration == true)
                    {
                        strBuilder.Append(" (exp: ").Append(ability.Expiration.Value.ToString()).Append(" UTC)");
                    }

                    strBuilder.Append(',').Append(' ');
                }
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            int maxCharCount = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
