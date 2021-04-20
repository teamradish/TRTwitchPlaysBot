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
    /// A command that lists all silenced users.
    /// </summary>
    public sealed class ListSilencedUsersCommand : BaseCommand
    {
        public ListSilencedUsersCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            PermissionAbility silencedPermAbility = DataHelper.GetPermissionAbility(PermissionConstants.SILENCED_ABILITY);

            if (silencedPermAbility == null)
            {
                QueueMessage($"There is no {PermissionConstants.SILENCED_ABILITY} in the database!");
                return;
            }

            StringBuilder strBuilder = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                DateTime now = DateTime.UtcNow;

                //This SQL query is manually performing what User.HasEnabledAbility would be performing
                //We do this to keep the query on the server side for performance reasons
                //Client-side performance here is significantly worse even with only 1000 users in the database 
                IEnumerable<User> silencedUsers = context.Users.Where(u =>
                    u.UserAbilities.FirstOrDefault(ab => ab.PermabilityID == silencedPermAbility.ID &&
                        //Enabled and either no expiration date or it isn't expired
                        ((ab.Enabled >= 1 && (ab.Expiration.HasValue == false || ab.Expiration.Value < now))
                        //Disabled and expired
                        || (ab.Enabled <= 0 && ab.Expiration.HasValue == true && ab.Expiration.Value >= now))) != null);

                int silencedUserCount = silencedUsers.Count();

                if (silencedUserCount == 0)
                {
                    QueueMessage("No one is silenced - hurray!");
                    return;
                }

                //Average estimation of username length multiplied by the number of users
                strBuilder = new StringBuilder(silencedUserCount * 8);
                
                strBuilder.Append("Silenced users: ");

                foreach (User user in silencedUsers)
                {
                    strBuilder.Append(user.Name).Append(',').Append(' ');
                }
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            string message = strBuilder.ToString();

            int botCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(message, botCharLimit, ", ");
        }
    }
}
