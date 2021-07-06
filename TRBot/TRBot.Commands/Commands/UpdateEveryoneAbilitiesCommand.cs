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
using Microsoft.EntityFrameworkCore;

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

            int userCount = 0;

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

                //Get count
                userCount = context.Users.Count();
            }

            QueueMessage($"Starting update of {userCount} users' abilities...");

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            //Update everyone's abilities
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Use eager loading to include all user abilities beforehand for a slight performance boost
                var allUsers = context.Users.Include(e => e.UserAbilities);

                foreach (User user in allUsers)
                {
                    long originalLevel = user.Level;
                    PermissionLevels origPermLvl = (PermissionLevels)originalLevel;

                    //First, disable all auto grant abilities the user has
                    //Don't disable abilities that were given by a higher level
                    //This prevents users from removing constraints imposed by moderators and such
                    IEnumerable<UserAbility> abilities = user.UserAbilities.Where(p => p.PermAbility.AutoGrantOnLevel >= PermissionLevels.User
                            && p.GrantedByLevel <= originalLevel);

                    foreach (UserAbility ability in abilities)
                    {
                        ability.SetEnabledState(false);
                        ability.Expiration = null;
                        ability.GrantedByLevel = -1;
                    }

                    //Get all auto grant abilities up to the user's level
                    IEnumerable<PermissionAbility> permAbilities =
                        context.PermAbilities.Where(p => p.AutoGrantOnLevel >= PermissionLevels.User
                            && p.AutoGrantOnLevel <= origPermLvl);

                    //Enable all of those abilities
                    foreach (PermissionAbility permAbility in permAbilities)
                    {
                        user.EnableAbility(permAbility);
                    }
                }

                context.SaveChanges();
            }

            sw.Stop();

            QueueMessage($"Finished updating {userCount} users' abilities in {sw.ElapsedMilliseconds}ms!");
        }
    }
}
