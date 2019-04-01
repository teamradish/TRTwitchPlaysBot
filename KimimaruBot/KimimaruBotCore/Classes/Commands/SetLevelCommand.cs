using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
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

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 2)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}setlevel usage: \"username\" \"level\"");
                return;
            }

            string levelUsername = args[0].ToLowerInvariant();
            string levelStr = args[1];

            string curUserName = e.Command.ChatMessage.Username.ToLowerInvariant();
            if (levelUsername == curUserName)
            {
                BotProgram.QueueMessage("You cannot set your own level!");
                return;
            }

            User levelUser = BotProgram.GetUser(levelUsername, true);

            if (levelUser == null)
            {
                BotProgram.QueueMessage($"User {levelUsername} does not exist in database!");
                return;
            }

            User curUser = BotProgram.GetUser(e.Command.ChatMessage.Username, true);

            if (curUser == null)
            {
                BotProgram.QueueMessage("Invalid user of this command; something went wrong?!");
                return;
            }

            if (levelUser.Level >= curUser.Level)
            {
                BotProgram.QueueMessage("You can't set the level of a user with a level equal to or greater than yours!");
                return;
            }

            if (int.TryParse(levelStr, out int levelNum) == false)
            {
                BotProgram.QueueMessage("Invalid level specified.");
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
                BotProgram.QueueMessage("Invalid level specified.");
                return;
            }

            if (levelNum > curUser.Level)
            {
                BotProgram.QueueMessage("You cannot set a level greater than your own!");
                return;
            }

            levelUser.Level = levelNum;

            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Set {levelUsername}'s level to {levelNum}, {lvlName}!");
        }
    }
}
