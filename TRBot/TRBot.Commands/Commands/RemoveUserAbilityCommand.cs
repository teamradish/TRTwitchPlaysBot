/* This file is part of TRBot.
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
    /// Removes a user ability.
    /// </summary>
    public sealed class RemoveUserAbilityCommand : BaseCommand
    {
        private const string NULL_EXPIRATION_ARG = "null";
        private string UsageMessage = "Usage: \"username\", \"ability name\"";

        public RemoveUserAbilityCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using BotDBContext context = DatabaseManager.OpenContext();

            //Get the user calling this
            string thisUserName = args.Command.ChatMessage.Username.ToLowerInvariant();

            User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);

            string username = arguments[0];
            string abilityName = arguments[1].ToLowerInvariant();

            User abilityUser = DataHelper.GetUserNoOpen(username, context);

            if (abilityUser == null)
            {
                QueueMessage("This user doesn't exist in the database!");
                return;
            }

            PermissionAbility permAbility = context.PermAbilities.FirstOrDefault(p => p.Name == abilityName);

            if (permAbility == null)
            {
                QueueMessage($"There is no permission ability named \"{abilityName}\".");
                return;
            }

            if (thisUser != abilityUser && thisUser.Level <= abilityUser.Level)
            {
                QueueMessage("You cannot modify abilities for other users with levels greater than or equal to yours!");
                return;
            }

            if ((long)permAbility.AutoGrantOnLevel > thisUser.Level)
            {
                QueueMessage("This ability is granted to those above your level, so you cannot remove it!");
                return;
            }

            if ((long)permAbility.MinLevelToGrant > thisUser.Level)
            {
                QueueMessage($"You need to be at least level {(long)permAbility.MinLevelToGrant}, {permAbility.MinLevelToGrant}, to remove this ability.");
                return;
            }

            if (abilityUser.TryGetAbility(abilityName, out UserAbility userAbility) == false)
            {
                QueueMessage($"User does not have the \"{abilityName}\" ability!");
                return;
            }

            if (userAbility.GrantedByLevel > thisUser.Level)
            {
                QueueMessage("This ability was granted by someone with a higher level than you, so you can't remove it.");
                return;
            }

            //Remove ability and save
            abilityUser.UserAbilities.Remove(userAbility);

            context.SaveChanges();

            QueueMessage($"Removed the \"{abilityName}\" ability from {abilityUser.Name}!");
        }
    }
}
