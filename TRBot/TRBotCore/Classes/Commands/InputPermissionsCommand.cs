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
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class InputPermissionsCommand : BaseCommand
    {
        private StringBuilder StrBuilder = null;

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            //See the permissions
            if (args.Count == 0)
            {
                BotProgram.QueueMessage($"Inputs are allowed for {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above. To set the permissions, add one as an argument: {GetValidPermsStr()}");
                return;
            }

            string permsStr = args[0];

            if (Enum.TryParse<AccessLevels.Levels>(permsStr, true, out AccessLevels.Levels perm) == false)
            {
                BotProgram.QueueMessage($"Please enter a valid permission: {GetValidPermsStr()}");
                return;
            }

            int permissionLvl = (int)perm;

            if (permissionLvl == BotProgram.BotData.InputPermissions)
            {
                BotProgram.QueueMessage($"The permissions are already {(AccessLevels.Levels)BotProgram.BotData.InputPermissions}!");
                return;
            }

            if (permissionLvl < (int)AccessLevels.Levels.User || permissionLvl > (int)AccessLevels.Levels.Admin)
            {
                BotProgram.QueueMessage("Invalid permission level!");
                return;
            }

            BotProgram.BotData.InputPermissions = permissionLvl;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Set input permissions to {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above!");
        }

        private string GetValidPermsStr()
        {
            string[] names = EnumUtility.GetNames<AccessLevels.Levels>.EnumNames;

            if (StrBuilder == null)
            {
                StrBuilder = new StringBuilder(500);
            }

            StrBuilder.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                StrBuilder.Append(names[i]).Append(',').Append(' ');
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            return StrBuilder.ToString();
        }
    }
}
