using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
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
                BotProgram.QueueMessage("!addmeme usage: memename memevalue");
                return;
            }

            if (args[0].ElementAt(0) == Globals.CommandIdentifier)
            {
                BotProgram.QueueMessage($"Memes cannot start with \'{Globals.CommandIdentifier}\'");
                return;
            }

            if (args[0].Length >= MAX_MEME_LENGTH)
            {
                BotProgram.QueueMessage($"The max meme length is {MAX_MEME_LENGTH} characters!");
                return;
            }

            string memeToLower = args[0].ToLower();
            bool sendOverwritten = false;

            if (MemesCommand.Memes.ContainsKey(memeToLower) == true)
            {
                MemesCommand.Memes.Remove(memeToLower);
                sendOverwritten = true;
            }

            string actualMeme = e.Command.ArgumentsAsString.Remove(0, args[0].Length + 1);

            MemesCommand.Memes.Add(memeToLower, actualMeme);

            MemesCommand.SaveMemesDict();

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
