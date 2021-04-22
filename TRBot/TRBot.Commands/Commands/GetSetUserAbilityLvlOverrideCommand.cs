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
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Obtains or sets the level override for a given user ability.
    /// </summary>
    public sealed class GetSetUserAbilityLvlOverrideCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username (string)\", \"ability name (string)\", \"level override (string/int) (-1 for no override)\" (optional)";

        public GetSetUserAbilityLvlOverrideCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count < 2 || arguments.Count > 3)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = args.Command.ChatMessage.Username;
            string otherUserName = arguments[0];
            string abilityName = arguments[1];

            long newLevelOverride = 0L;

            if (arguments.Count == 3)
            {
                string levelArgStr = arguments[2];

                if (PermissionHelpers.TryParsePermissionLevel(levelArgStr, out PermissionLevels permLevel) == false)
                {
                    //Check if the value is -1, which is the only other valid value
                    if (long.TryParse(levelArgStr, out newLevelOverride) == false || newLevelOverride != -1L)
                    {
                        QueueMessage("Please enter a valid permission level for the level override, or -1 for no override!");
                        return;
                    }
                }
                else
                {
                    newLevelOverride = (long)permLevel;
                }
            }

            PermissionAbility permAbility = DataHelper.GetPermissionAbility(abilityName);

            if (permAbility == null)
            {
                QueueMessage($"No ability named {abilityName} exists in the database.");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(userName, context);
                User changedUser = DataHelper.GetUserNoOpen(otherUserName, context);

                if (changedUser == null)
                {
                    QueueMessage($"No user named {otherUserName} exists in the database.");
                    return;
                }

                if (changedUser.TryGetAbility(permAbility.ID, out UserAbility userAbility) == false)
                {
                    QueueMessage($"{otherUserName} doesn't have an ability named {abilityName}.");
                    return;
                }

                //Get the value
                long curLevelGrant = userAbility.GrantedByLevel;

                //Simply print the value if 2 arguments are given
                if (arguments.Count == 2)
                {
                    if (curLevelGrant < 0)
                    {
                        QueueMessage($"The \"{abilityName}\" ability for {otherUserName} doesn't have a level override.");
                    }
                    else
                    {
                        QueueMessage($"The level override of the \"{abilityName}\" ability for {otherUserName} is {curLevelGrant}, {(PermissionLevels)curLevelGrant}. Users at or above this level can modify this ability on this user."); 
                    }

                    return;
                }

                //Set the value if you can
                if (thisUser.CanAddAbilityToUser(permAbility, changedUser) == false)
                {
                    QueueMessage($"Either you don't have permission to grant the \"{abilityName}\" ability, or the level override is higher than your own level.");
                    return;
                }

                userAbility.GrantedByLevel = newLevelOverride;

                context.SaveChanges();
            }

            string message = $"Set the level override of the {abilityName} ability for {otherUserName} to {newLevelOverride}";

            if (newLevelOverride >= 0)
            {
                message += $", {(PermissionLevels)newLevelOverride}";
            }

            message += "!";

            QueueMessage(message);
        }
    }
}
