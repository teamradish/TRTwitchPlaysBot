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
    /// Updates a user's level.
    /// This will also adjust the user's abilities.
    /// </summary>
    public sealed class SetUserLevelCommand : BaseCommand
    {
        public SetUserLevelCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 2)
            {
                QueueMessage("Usage: \"username\" \"level\"");
                return;
            }

            using BotDBContext context = DatabaseManager.OpenContext();

            string levelUsername = arguments[0].ToLowerInvariant();
            string levelStr = arguments[1].ToLowerInvariant();

            string curUserName = args.Command.ChatMessage.Username.ToLowerInvariant();
            if (levelUsername == curUserName)
            {
                QueueMessage("You cannot set your own level!");
                return;
            }

            User levelUser = DataHelper.GetUserNoOpen(levelUsername, context);

            if (levelUser == null)
            {
                QueueMessage("User does not exist in database!");
                return;
            }

            User curUser = DataHelper.GetUserNoOpen(curUserName, context);

            if (curUser == null)
            {
                QueueMessage("Invalid user of this command; something went wrong?!");
                return;
            }

            if (levelUser.Level >= curUser.Level)
            {
                QueueMessage("You can't set the level of a user whose level is equal to or greater than yours!");
                return;
            }

            //Parse the level
            if (PermissionHelpers.TryParsePermissionLevel(levelStr, out PermissionLevels permLvl) == false)
            {
                QueueMessage("Invalid level specified.");
                return;
            }

            long levelNum = (long)permLvl;

            //Invalid - attempting to set level higher than or equal to own
            if (levelNum >= curUser.Level)
            {
                QueueMessage("You cannot set a level greater than or equal to your own!");
                return;
            }

            //Set level, adjust abilities, and save
            DataHelper.AdjustUserAbilitiesOnLevel(levelUser, levelNum, context);
            
            levelUser.Level = levelNum;

            context.SaveChanges();

            QueueMessage($"Set {levelUsername}'s level to {levelNum}, {permLvl}!");
        }
    }
}
