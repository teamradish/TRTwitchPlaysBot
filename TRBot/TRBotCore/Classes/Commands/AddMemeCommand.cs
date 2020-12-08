/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

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

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 2)
            {
                BotProgram.MsgHandler.QueueMessage($"{Globals.CommandIdentifier}addmeme usage: memename memevalue");
                return;
            }

            if (args[0].ElementAt(0) == '/' || args[1].ElementAt(0) == '/')
            {
                BotProgram.MsgHandler.QueueMessage("Memes cannot start with Twitch chat commands!");
                return;
            }

            if (args[0].ElementAt(0) == Globals.CommandIdentifier)
            {
                BotProgram.MsgHandler.QueueMessage($"Memes cannot start with \'{Globals.CommandIdentifier}\'");
                return;
            }

            if (args[0].ElementAt(0) == Globals.MacroIdentifier)
            {
                BotProgram.MsgHandler.QueueMessage($"Memes cannot start with \'{Globals.MacroIdentifier}\'");
                return;
            }

            if (args[0].Length > MAX_MEME_LENGTH)
            {
                BotProgram.MsgHandler.QueueMessage($"The max meme length is {MAX_MEME_LENGTH} characters!");
                return;
            }

            string memeToLower = args[0].ToLower();
            bool sendOverwritten = false;

            if (BotProgram.BotData.Memes.ContainsKey(memeToLower) == true)
            {
                BotProgram.BotData.Memes.TryRemove(memeToLower, out string meme);
                sendOverwritten = true;
            }

            string actualMeme = e.Command.ArgumentsAsString.Remove(0, args[0].Length + 1);

            BotProgram.BotData.Memes.TryAdd(memeToLower, actualMeme);

            BotProgram.SaveBotData();

            if (sendOverwritten == true)
            {
                BotProgram.MsgHandler.QueueMessage("Meme overwritten!");
            }
            else
            {
                ListMemesCommand.CacheMemesString();
            }
        }
    }
}
