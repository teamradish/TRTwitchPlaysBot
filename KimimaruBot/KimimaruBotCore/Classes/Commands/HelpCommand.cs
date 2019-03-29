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
        private KeyValuePair<string, BaseCommand>[] CommandCache = null;
        private StringBuilder StrBuilder = new StringBuilder(1000);

        public HelpCommand()
        {
            
        }
        
        public override void Initialize(CommandHandler commandHandler)
        {
            //Cache command string
            CommandCache = commandHandler.CommandDict.ToArray();
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            StrBuilder.Clear();

            for (int i = 0; i < CommandCache.Length; i++)
            {
                ref KeyValuePair<string, BaseCommand> cmd = ref CommandCache[i];

                //Ignore if the command is hidden from the help menu
                if (cmd.Value.HiddenFromHelp == true) continue;

                //Show the command only if the user has access to it
                if (user.Level >= cmd.Value.AccessLevel)
                {
                    StrBuilder.Append(Globals.CommandIdentifier).Append(cmd.Key).Append(", ");
                }
            }

            int lastTwo = StrBuilder.Length - 2;

            //Remove trailing comma
            if (lastTwo >= 0)
            {
                StrBuilder.Remove(lastTwo, 2);
            }

            string helpStr = StrBuilder.ToString();

            BotProgram.QueueMessage($"Hi {e.Command.ChatMessage.DisplayName}! Here's a list of all commands: {helpStr}");
        }
    }
}
