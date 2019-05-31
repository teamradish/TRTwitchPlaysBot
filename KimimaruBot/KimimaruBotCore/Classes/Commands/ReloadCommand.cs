using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Reloads settings and bot data.
    /// </summary>
    public sealed class ReloadCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            //Reload settings and bot data
            BotProgram.LoadSettingsAndBotData();

            BotProgram.QueueMessage("Reloaded bot settings and data!");
        }
    }
}
