using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class RemoveMemeCommand : BaseCommand
    {
        public RemoveMemeCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}removememe usage: memename");
                return;
            }

            string meme = args[0].ToLower();
            if (BotProgram.BotData.Memes.ContainsKey(meme) == true)
            {
                BotProgram.BotData.Memes.Remove(meme);
                BotProgram.SaveBotData();
                MemesCommand.CacheMemesString();
            }
        }
    }
}
