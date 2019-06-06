using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Silences a user, preventing them from performing inputs.
    /// </summary>
    public sealed class SilenceCommand : BaseCommand
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

            string silencedName = args[0].ToLowerInvariant();
            string selfName = e.Command.ChatMessage.Username.ToLowerInvariant();
            if (silencedName == selfName)
            {
                BotProgram.QueueMessage("No use in silencing yourself, silly!");
                return;
            }

            User user = BotProgram.GetUser(silencedName);

            if (user == null)
            {
                BotProgram.QueueMessage($"User does not exist in database!");
                return;
            }

            if (user.Silenced == true)
            {
                BotProgram.QueueMessage($"User {silencedName} is already silenced!");
                return;
            }

            //Make sure the user you're silencing is a lower level than you
            User selfUser = BotProgram.GetUser(selfName);
            if (selfUser.Level <= user.Level)
            {
                BotProgram.QueueMessage($"Cannot silence a user at or above your access level!");
                return;
            }

            user.Silenced = true;
            BotProgram.BotData.SilencedUsers.Add(silencedName);
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"User {silencedName} has been silenced and thus prevented from performing inputs.");
        }
    }
}
