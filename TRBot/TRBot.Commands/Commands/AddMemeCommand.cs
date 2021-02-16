/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Adds a meme.
    /// </summary>
    public sealed class AddMemeCommand : BaseCommand
    {
        /// <summary>
        /// The max length for memes.
        /// </summary>
        public const int MAX_MEME_NAME_LENGTH = 50;

        private string UsageMessage = "Usage: \"memename\" \"memevalue\"";

        public AddMemeCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count < 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string memeName = arguments[0];
            string memeValue = arguments[1];

            if (memeName.ElementAt(0) == '/' || memeValue.ElementAt(0) == '/')
            {
                QueueMessage("Memes cannot start with Twitch chat commands!");
                return;
            }

            if (memeName.StartsWith(Parser.DEFAULT_PARSER_REGEX_MACRO_INPUT) == true)
            {
                QueueMessage($"Memes cannot start with \"{Parser.DEFAULT_PARSER_REGEX_MACRO_INPUT}\".");
                return;
            }

            if (memeName.ElementAt(0) == DataConstants.COMMAND_IDENTIFIER)
            {
                QueueMessage($"Memes cannot start with \'{DataConstants.COMMAND_IDENTIFIER}\'.");
                return;
            }

            if (memeName.Length > MAX_MEME_NAME_LENGTH)
            {
                QueueMessage($"Memes may have up to a max of {MAX_MEME_NAME_LENGTH} characters in their name!");
                return;
            }

            string memeToLower = memeName.ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Meme meme = context.Memes.FirstOrDefault(m => m.MemeName == memeToLower);

                string actualMemeValue = args.Command.ArgumentsAsString.Remove(0, memeName.Length + 1);

                if (meme != null)
                {
                    meme.MemeValue = actualMemeValue;

                    QueueMessage("Meme overwritten!");
                }
                else
                {
                    Meme newMeme = new Meme(memeToLower, actualMemeValue);
                    context.Memes.Add(newMeme);
                }

                context.SaveChanges();
            }
        }
    }
}
