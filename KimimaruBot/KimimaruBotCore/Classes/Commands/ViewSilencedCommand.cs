using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Views the silenced status of a particular user or views all silenced users.
    /// </summary>
    public sealed class ViewSilencedCommand : BaseCommand
    {
        private StringBuilder StrBuilder = new StringBuilder(1000);

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            //Kimimaru: I feel anyone should be able to use this to see if their peers can help out with the game or not
            AccessLevel = (int)AccessLevels.Levels.User;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            //If a username was specified, view that user's silenced state
            if (args.Count == 1)
            {
                string username = args[0].ToLowerInvariant();
                User user = BotProgram.GetUser(username);

                if (user == null)
                {
                    BotProgram.QueueMessage($"User {username} does not exist in database!");
                }
                else
                {
                    if (user.Silenced == false)
                    {
                        BotProgram.QueueMessage($"User {username} is not currently silenced and can perform inputs.");
                    }
                    else
                    {
                        BotProgram.QueueMessage($"User {username} is currently silenced and cannot perform inputs.");
                    }
                }

                return;
            }

            //No arguments, so print all users who are silenced
            if (BotProgram.BotData.SilencedUsers.Count == 0)
            {
                BotProgram.QueueMessage("No users are silenced. Hurray!");
                return;
            }

            //Copy for safety in case this gets modified during iteration
            string[] silencedNames = new string[BotProgram.BotData.SilencedUsers.Count];
            BotProgram.BotData.SilencedUsers.CopyTo(silencedNames);

            StrBuilder.Clear();

            StrBuilder.Append("Currently silenced users: ");

            for (int i = 0; i < silencedNames.Length; i++)
            {
                StrBuilder.Append(silencedNames[i]).Append(", ");
            }

            //Remove trailing comma
            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            string silencedStr = StrBuilder.ToString();

            BotProgram.QueueMessage(silencedStr);
        }
    }
}
