using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Sets an info message that is output by the bot.
    /// </summary>
    public sealed class SetInfoCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string msg = e.Command.ArgumentsAsString;

            if (msg == null)
            {
                msg = string.Empty;
            }

            BotProgram.BotData.InfoMessage = msg;

            BotProgram.SaveBotData();
        }
    }
}
