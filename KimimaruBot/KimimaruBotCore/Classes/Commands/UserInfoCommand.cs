using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Tells information about a user.
    /// </summary>
    public sealed class UserInfoCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count > 1)
            {
                BotProgram.QueueMessage("Usage: \"username\"");
                return;
            }

            string username = string.Empty;

            //If no arguments are specified, use the name of the user who performed the command
            if (args.Count == 0)
            {
                username = e.Command.ChatMessage.Username;
            }
            else
            {
                username = args[0];
            }

            User user = BotProgram.GetUser(username, false);

            if (user == null)
            {
                BotProgram.QueueMessage($"User does not exist in database!");
                return;
            }

            //Print the user's information
            BotProgram.QueueMessage($"User: {user.Name} | Level: {user.Level} | Total Inputs: {user.ValidInputs} | Total Messages: {user.TotalMessages}");
        }
    }
}
