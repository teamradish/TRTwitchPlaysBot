using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Unsilences a user, allowing them to perform inputs again.
    /// </summary>
    public sealed class UnsilenceCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage("Usage: \"username\"");
                return;
            }

            string unsilencedName = args[0].ToLowerInvariant();
            string selfName = e.Command.ChatMessage.Username.ToLowerInvariant();
            if (unsilencedName == selfName)
            {
                BotProgram.QueueMessage("Nice try.");
                return;
            }

            User user = BotProgram.GetUser(unsilencedName);

            if (user == null)
            {
                BotProgram.QueueMessage($"User does not exist in database!");
                return;
            }

            if (user.Silenced == false)
            {
                BotProgram.QueueMessage($"User {unsilencedName} is not silenced!");
                return;
            }

            //Make sure the user you're silencing is a lower level than you
            User selfUser = BotProgram.GetUser(selfName);
            if (selfUser.Level <= user.Level)
            {
                BotProgram.QueueMessage($"Cannot unsilence a user at or above your access level!");
                return;
            }

            user.Silenced = false;
            BotProgram.BotData.SilencedUsers.Remove(unsilencedName);
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"User {unsilencedName} has been unsilenced and can perform inputs once again.");
        }
    }
}
