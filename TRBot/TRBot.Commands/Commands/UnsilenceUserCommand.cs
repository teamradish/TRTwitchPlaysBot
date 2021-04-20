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
    /// A command that unsilences a user.
    /// </summary>
    public sealed class UnsilenceUserCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username (string)\"";

        public UnsilenceUserCommand()
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
            string otherUserName = arguments[0].ToLowerInvariant();

            PermissionAbility silencedPermAbility = DataHelper.GetPermissionAbility(PermissionConstants.SILENCED_ABILITY);

            if (silencedPermAbility == null)
            {
                QueueMessage($"There is no {PermissionConstants.SILENCED_ABILITY} in the database!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(userName, context);
                User otherUser = DataHelper.GetUserNoOpen(otherUserName, context);

                if (otherUser == null)
                {
                    QueueMessage($"No user named {otherUserName} can be found in the database!");
                    return;
                }

                if (thisUser.ID == otherUser.ID)
                {
                    QueueMessage("You can't unsilence yourself!");
                    return;
                }

                if (thisUser.CanAddAbilityToUser(silencedPermAbility, otherUser) == false)
                {
                    QueueMessage($"You cannot unsilence {otherUser.Name}, likely because you don't have permissions or a higher-leveled user adjusted their silence status.");
                    return;
                }

                if (otherUser.HasEnabledAbility(silencedPermAbility.Name) == false)
                {
                    QueueMessage($"{otherUserName} isn't silenced!");
                    return;
                }

                //Use the level of the command as the override so others with equal access to it can silence
                otherUser.AddAbility(silencedPermAbility, false, string.Empty, 0, Level, null);

                context.SaveChanges();
            }

            QueueMessage($"{otherUserName} has been unsilenced and can once again perform inputs!");
        }
    }
}
