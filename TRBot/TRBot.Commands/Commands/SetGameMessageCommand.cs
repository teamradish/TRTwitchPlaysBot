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
using System.IO;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Sets a game message that can be displayed on the stream.
    /// </summary>
    public sealed class SetGameMessageCommand : BaseCommand
    {
        public SetGameMessageCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            //Check if the user has the ability to set the message
            User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

            if (user != null && user.HasAbility(PermissionConstants.SET_GAME_MESSAGE_ABILITY) == false)
            {
                QueueMessage("You don't have the ability to set the game message!");
                return;
            }

            string gameMsgText = args.Command.ArgumentsAsString;

            //Allow setting a null message to clear the file
            if (gameMsgText == null)
            {
                gameMsgText = string.Empty;
            }

            //Save the new message into our data
            Settings gameMsgSetting = DataHelper.GetSettingNoOpen(SettingsConstants.GAME_MESSAGE, context);
            gameMsgSetting.value_str = gameMsgText;

            context.SaveChanges();

            //Get settings for the name and location of the game message
            long msgPathRelative = DataHelper.GetSettingIntNoOpen(SettingsConstants.GAME_MESSAGE_PATH_IS_RELATIVE, context, 1L);

            string msgFileName = DataHelper.GetSettingStringNoOpen(SettingsConstants.GAME_MESSAGE_PATH, context, string.Empty);

            string fullMsgPath = msgFileName;

            //Get relative path if we should
            if (msgPathRelative == 1)
            {
                fullMsgPath = Path.Combine(DataConstants.DataFolderPath, msgFileName);
            }

            //Save the message to the file so it updates on OBS
            /*For reading from this file on OBS:
              1. Create a Text object
              2. Check the box labeled "Read from file"
              3. Browse and select the file
             */
            if (FileHelpers.SaveToTextFile(fullMsgPath, gameMsgText) == false)
            {
                QueueMessage("Unable to validate path for game message.");
            }
        }
    }
}
