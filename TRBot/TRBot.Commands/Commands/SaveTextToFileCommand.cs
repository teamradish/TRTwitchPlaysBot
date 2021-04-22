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
using System.Text;
using System.IO;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// Saves text to a given file, which can be displayed on stream.
    /// </summary>
    public class SaveTextToFileCommand : BaseCommand
    {
        /// <summary>
        /// Tells if the path to the file is relative.
        /// </summary>
        protected virtual bool PathIsRelative => false;

        /// <summary>
        /// The permission ability required for a user to execute this command. Leave empty for no permissions.
        /// </summary>
        protected virtual string PermissionAbilityRequired => string.Empty;

        /// <summary>
        /// The message displayed when the user doesn't have sufficient privileges to use this command.
        /// </summary>
        protected virtual string PermissionDeniedMessage => "You don't have permission to use this command!";

        public SaveTextToFileCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            //Validate the ValueStr path
            if (string.IsNullOrEmpty(ValueStr) == true)
            {
                QueueMessage($"This command's {nameof(ValueStr)} is empty. The streamer should set this in the database to a valid file path. If this is {nameof(SetGameMessageCommand)} and you just upgraded TRBot, set {nameof(ValueStr)} to \"{SettingsConstants.GAME_MESSAGE_PATH}\" and run the reload command to apply the changes on the spot.");
                return;
            }

            //Check permission ability if not empty
            string permAbility = PermissionAbilityRequired;

            if (string.IsNullOrEmpty(permAbility) == false)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Check if the user has the ability to save to the file
                    User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                    if (user != null && user.HasEnabledAbility(permAbility) == false)
                    {
                        QueueMessage(PermissionDeniedMessage);
                        return;
                    }
                }
            }

            string textToSave = args.Command.ArgumentsAsString;

            //Allow setting a null message to clear the file
            if (textToSave == null)
            {
                textToSave = string.Empty;
            }

            //Get the path - first check if it's a database setting
            string fileLocation = DataHelper.GetSettingString(ValueStr, string.Empty);

            //Fallback to the ValueStr if this isn't a database setting
            if (string.IsNullOrEmpty(fileLocation) == true)
            {
                fileLocation = ValueStr;
            }

            //Get relative path if we should
            if (PathIsRelative == true)
            {
                fileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileLocation);
            }

            //Save the message to the file
            if (FileHelpers.SaveToTextFile(fileLocation, textToSave) == false)
            {
                QueueMessage("Unable to validate path for game message.");
            }
        }
    }
}
