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
    /// Updates the user abilities for every single user in the database.
    /// This should be used only sparingly, such as after upgrading to a version with new permission abilities,
    /// since it can take a while based on how many users there are in the database.
    /// </summary>
    public sealed class UpdateEveryoneAbilitiesCommand : BaseCommand
    {
        public UpdateEveryoneAbilitiesCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 0)
            {
                QueueMessage("This command doesn't take any arguments.");
                return;
            }

            List<string> userNames = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Get the user calling this
                string thisUserName = args.Command.ChatMessage.Username.ToLowerInvariant();

                User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);

                //Check if this user has permission to do this
                if (thisUser.HasEnabledAbility(PermissionConstants.UPDATE_OTHER_USER_ABILITES) == false)
                {
                    QueueMessage("You do not have permission to update other users' abilities!");
                    return;
                }

                //Initialize list
                userNames = new List<string>(context.Users.Count());

                foreach (User user in context.Users)
                {
                    //Prohibit updating abilities for higher levels
                    if (thisUser.Level < user.Level)
                    {
                        continue;
                    }

                    //Add their name to the list
                    userNames.Add(user.Name);
                }
            }

            QueueMessage($"Starting update of {userNames.Count} users' abilities...");

            //Fully update everyone's abilities
            for (int i = 0; i < userNames.Count; i++)
            {
                DataHelper.UpdateUserAutoGrantAbilities(userNames[i]);
            }

            QueueMessage($"Finished updating {userNames.Count} users' abilities!");
        }
    }
}
