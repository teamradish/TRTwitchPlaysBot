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
    /// <summary>
    /// Sets the level of a user.
    /// </summary>
    public sealed class SetLevelCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 2)
            {
                BotProgram.MsgHandler.QueueMessage($"{Globals.CommandIdentifier}setlevel usage: \"username\" \"level\"");
                return;
            }

            string levelUsername = args[0].ToLowerInvariant();
            string levelStr = args[1];

            string curUserName = e.Command.ChatMessage.Username.ToLowerInvariant();
            if (levelUsername == curUserName)
            {
                BotProgram.MsgHandler.QueueMessage("You cannot set your own level!");
                return;
            }

            User levelUser = BotProgram.GetUser(levelUsername, true);

            if (levelUser == null)
            {
                BotProgram.MsgHandler.QueueMessage($"User does not exist in database!");
                return;
            }

            User curUser = BotProgram.GetUser(e.Command.ChatMessage.Username, true);

            if (curUser == null)
            {
                BotProgram.MsgHandler.QueueMessage("Invalid user of this command; something went wrong?!");
                return;
            }

            if (levelUser.Level >= curUser.Level)
            {
                BotProgram.MsgHandler.QueueMessage("You can't set the level of a user with a level equal to or greater than yours!");
                return;
            }

            if (int.TryParse(levelStr, out int levelNum) == false)
            {
                BotProgram.MsgHandler.QueueMessage("Invalid level specified.");
                return;
            }

            AccessLevels.Levels[] levelArray = EnumUtility.GetValues<AccessLevels.Levels>.EnumValues;

            bool found = false;
            string lvlName = string.Empty;

            for (int i = 0; i < levelArray.Length; i++)
            {
                if (levelNum == (int)levelArray[i])
                {
                    found = true;
                    lvlName = levelArray[i].ToString();
                    break;
                }
            }

            if (found == false)
            {
                BotProgram.MsgHandler.QueueMessage("Invalid level specified.");
                return;
            }

            if (levelNum >= curUser.Level)
            {
                BotProgram.MsgHandler.QueueMessage("You cannot set a level greater than or equal to your own!");
                return;
            }

            levelUser.SetLevel(levelNum);

            BotProgram.SaveBotData();

            BotProgram.MsgHandler.QueueMessage($"Set {levelUsername}'s level to {levelNum}, {lvlName}!");
        }
    }
}
