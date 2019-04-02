using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Sets a message that can be displayed on the stream.
    /// </summary>
    public sealed class SetMessageCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Whitelisted
                ;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string msg = e.Command.ArgumentsAsString;

            if (msg == null)
            {
                msg = string.Empty;
            }

            BotProgram.BotData.GameMessage = msg;
            BotProgram.SaveBotData();
        }
    }
}
