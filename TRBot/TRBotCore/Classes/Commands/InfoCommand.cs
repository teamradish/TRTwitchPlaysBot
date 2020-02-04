using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Gets the channel's info message.
    /// </summary>
    public sealed class InfoCommand : BaseCommand
    {
        private const string EmptyMessage = "No info message set.";

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.User;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (string.IsNullOrEmpty(BotProgram.BotData.InfoMessage) == true)
            {
                BotProgram.QueueMessage(EmptyMessage);
                return;
            }

            BotProgram.QueueMessage(BotProgram.BotData.InfoMessage);
        }
    }
}
