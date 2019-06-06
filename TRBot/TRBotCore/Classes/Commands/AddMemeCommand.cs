using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class AddMemeCommand : BaseCommand
    {
        public const int MAX_MEME_LENGTH = 50;

        public AddMemeCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 2)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}addmeme usage: memename memevalue");
                return;
            }

            if (args[0].ElementAt(0) == Globals.CommandIdentifier)
            {
                BotProgram.QueueMessage($"Memes cannot start with \'{Globals.CommandIdentifier}\'");
                return;
            }

            if (args[0].ElementAt(0) == Globals.MacroIdentifier)
            {
                BotProgram.QueueMessage($"Memes cannot start with \'{Globals.MacroIdentifier}\'");
                return;
            }

            if (args[0].Length > MAX_MEME_LENGTH)
            {
                BotProgram.QueueMessage($"The max meme length is {MAX_MEME_LENGTH} characters!");
                return;
            }

            string memeToLower = args[0].ToLower();
            bool sendOverwritten = false;

            if (BotProgram.BotData.Memes.ContainsKey(memeToLower) == true)
            {
                BotProgram.BotData.Memes.Remove(memeToLower);
                sendOverwritten = true;
            }

            string actualMeme = e.Command.ArgumentsAsString.Remove(0, args[0].Length + 1);

            BotProgram.BotData.Memes.Add(memeToLower, actualMeme);

            BotProgram.SaveBotData();

            if (sendOverwritten == true)
            {
                BotProgram.QueueMessage("Meme overwritten!");
            }
            else
            {
                MemesCommand.CacheMemesString();
            }
        }
    }
}
