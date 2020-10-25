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
using System.Globalization;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that sets the permission level for an input on a console.
    /// </summary>
    public sealed class SetInputLevelCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - \"console name\", \"input name\", \"level (string/int)\"";

        public SetInputLevelCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not enough arguments
            if (argCount != 3)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleStr = arguments[0].ToLowerInvariant();

            using BotDBContext context = DatabaseManager.OpenContext();

            GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleStr);
            if (console == null)
            {
                QueueMessage($"No console named \"{consoleStr}\" found.");
                return;
            }

            string inputName = arguments[1].ToLowerInvariant();
            
            //Check if the input exists
            InputData inputData = console.InputList.FirstOrDefault((inpData) => inpData.Name == inputName);

            if (inputData == null)
            {
                QueueMessage($"Input \"{inputName}\" does not exist in console \"{consoleStr}\".");
                return;
            }

            string levelStr = arguments[2].ToLowerInvariant();

            //Parse the permission level
            if (PermissionHelpers.TryParsePermissionLevel(levelStr, out PermissionLevels permLevel) == false)
            {
                QueueMessage("Invalid level specified.");
                return;
            }

            //Compare this user's level with the input's current level
            User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username.ToLowerInvariant(), context);
            
            long newLvlNum = (long)permLevel;

            if (user != null)
            {
                long curInputLvl = inputData.level;

                //Your level is less than the current input's level - invalid
                if (user.Level < curInputLvl)
                {
                    QueueMessage("Cannot change this input's access level because your level is lower than it.");
                    return;
                }
                //The new level number is higher than your level - invalid
                else if (user.Level < newLvlNum)
                {
                    QueueMessage("Cannot change this input's access level because the new level would be higher than yours.");
                    return;
                }
            }

            inputData.level = newLvlNum;

            context.SaveChanges();

            QueueMessage($"Set the level of input \"{inputName}\" on \"{consoleStr}\" to {newLvlNum}, {permLevel}!");
        }

        private bool GetInt(string value, out int num)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out num);
        }
    }
}
