using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class ChatBotCommand : BaseCommand
    {
        public ChatBotCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotSettings.UseChatBot == false)
            {
                BotProgram.QueueMessage("The streamer is not currently using a chatbot!");
                return;
            }
            
            string question = e.Command.ArgumentsAsString;
            
            if (Globals.SaveToTextFile(Globals.ChatBotPromptFilename, question) == false)
            {
                BotProgram.QueueMessage("Error saving question to prompt file.");
                return;
            }
        }
    }
}
