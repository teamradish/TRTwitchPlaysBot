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
    /// Updates all of a user's abilities, enabling the ones they should have at their current access level,
    /// and disabling the ones they shouldn't have at their access level.
    /// For use if changing a user's level outside the application, such as directly through the database.
    /// </summary>
    public sealed class UpdateAllUserAbilitiesCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username\"";

        public UpdateAllUserAbilitiesCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //This supports updating another user's abilities if provided as an argument, but only one user at a time
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string usedName = string.Empty;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Get the user calling this
                string thisUserName = args.Command.ChatMessage.Username.ToLowerInvariant();
                usedName = thisUserName;

                User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);

                //Check to update another user's abilities
                if (arguments.Count == 1)
                {
                    //Check if this user has permission to do this
                    if (thisUser.HasEnabledAbility(PermissionConstants.UPDATE_OTHER_USER_ABILITES) == false)
                    {
                        QueueMessage("You do not have permission to update another user's abilities!");
                        return;
                    }

                    string otherUserName = arguments[0].ToLowerInvariant();

                    User changedUser = DataHelper.GetUserNoOpen(otherUserName, context);

                    if (changedUser == null)
                    {
                       QueueMessage("A user with this name does not exist in the database!");
                       return;
                    }

                    //Prohibit updating abilities for higher levels
                    if (thisUser.Level < changedUser.Level)
                    {
                       QueueMessage("You cannot update the abilities for someone higher in level than you!");
                        return;
                    }

                    usedName = otherUserName;
                }
            }

            //Fully update the abilities
            DataHelper.UpdateUserAutoGrantAbilities(usedName);

            QueueMessage($"Updated {usedName}'s abilities!");
        }
    }
}
