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
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Gets or sets the global minimum level required to perform inputs.
    /// </summary>
    public class GlobalInputPermissionsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"level (string/int)\"";
        private string CachedPermsStr = string.Empty;

        public GlobalInputPermissionsCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            string[] names = EnumUtility.GetNames<PermissionLevels>.EnumNames;

            for (int i = 0; i < names.Length; i++)
            {
                CachedPermsStr += names[i];

                if (i < (names.Length - 1))
                {
                    CachedPermsStr += ", ";
                }
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            using BotDBContext context = DatabaseManager.OpenContext();

            Settings inputPermsSetting = DataHelper.GetSettingNoOpen(SettingsConstants.GLOBAL_INPUT_LEVEL, context);

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //See the permissions
            if (arguments.Count == 0)
            {
                QueueMessage($"Inputs are allowed for {(PermissionLevels)inputPermsSetting.value_int} and above. To set the permissions, add one as an argument: {CachedPermsStr}");
                return;
            }

            User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

            //Check if this user has the ability to set this level
            if (user != null && user.HasAbility(PermissionConstants.SET_GLOBAL_INPUT_LEVEL_ABILITY) == false)
            {
                QueueMessage("You do not have the ability to set the global input level!");
                return;
            }

            string permsStr = arguments[0];

            //Try to parse the permission level
            if (PermissionHelpers.TryParsePermissionLevel(permsStr, out PermissionLevels permLevel) == false)
            {
                QueueMessage($"Please enter a valid permission: {CachedPermsStr}");
                return;
            }

            //The permissions are already this value
            if (inputPermsSetting.value_int == (long)permLevel)
            {
                QueueMessage($"The permissions are already {permLevel}!");
                return;
            }

            //Check if this user is lower than this level and deny it if so
            if (user.Level < (long)permLevel)
            {
                QueueMessage("You cannot set the global input level greater than your own level!");
                return;
            }

            //Set new value and save
            inputPermsSetting.value_int = (long)permLevel;

            context.SaveChanges();

            QueueMessage($"Set input permissions to {(PermissionLevels)inputPermsSetting.value_int} and above!");
        }
    }
}
