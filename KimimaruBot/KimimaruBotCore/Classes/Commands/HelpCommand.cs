using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class HelpCommand : BaseCommand
    {
        private string CachedCommandStr = null;

        public HelpCommand()
        {
            
        }
        
        public override void Initialize(CommandHandler commandHandler)
        {
            //Cache command string
            KeyValuePair<string, BaseCommand>[] commands = commandHandler.CommandDict.ToArray();

            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i].Value.HiddenFromHelp == true) continue;

                CachedCommandStr += Globals.CommandIdentifier + commands[i].Key + ", ";
            }

            CachedCommandStr = CachedCommandStr.TrimEnd(',', ' ');
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage($"Hi {e.Command.ChatMessage.DisplayName}! Here's a list of all commands: {CachedCommandStr}");
        }
    }
}
