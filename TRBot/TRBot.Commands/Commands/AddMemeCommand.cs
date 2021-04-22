/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Logging;

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

        private string UsageMessage = "Usage: \"memename (enclose in \" quotes for multi-word)\" \"memevalue\"";

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

            string userName = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.ADD_MEME_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to add memes.");
                    return;
                }
            }

            string memeName = arguments[0];
            int memeNameLength = memeName.Length;

            int memeValArgStartIndex = 1;

            //Check if it's a multi-word meme
            if (memeName.StartsWith("\"") == true && arguments.Count > 2)
            {
                //Start with 1 to account for the space after the first argument
                int additionalLength = 1;

                //Look for arguments ending with quotes
                for (int i = 1; i < arguments.Count; i++)
                {
                    string arg = arguments[i];
                    
                    //Add 1 to account for the space
                    additionalLength += arg.Length + 1;

                    //TRBotLogger.Logger.Information($"Additional length is: {additionalLength}");

                    //We found the meme
                    if (arg.EndsWith("\"") == true)
                    {
                        //The next index is the start of the meme value
                        memeValArgStartIndex = i + 1;

                        //Invalid - all arguments are surrounded in quotes
                        if (memeValArgStartIndex >= arguments.Count)
                        {
                            QueueMessage("Please provide a value for the multi-word meme.");
                            return;
                        }

                        //Adjust meme length and name
                        //Account for spaces
                        memeNameLength += additionalLength - 1;

                        //Remove the start and end quotes from the meme's name
                        memeName = args.Command.ArgumentsAsString.Substring(0, memeNameLength);
                        memeName = memeName.Remove(0, 1);
                        memeName = memeName.Remove(memeName.Length - 1, 1);

                        //TRBotLogger.Logger.Information($"Meme name: \"{memeName}\"");
                        //TRBotLogger.Logger.Information($"Meme Length: \"{memeNameLength}\"");

                        break;
                    }
                }
            }
            
            //TRBotLogger.Logger.Information($"Meme value starts at index: {memeValArgStartIndex}");

            string memeValue = arguments[memeValArgStartIndex];

            if (memeName.ElementAt(0) == '/' || memeValue.ElementAt(0) == '/')
            {
                QueueMessage("Memes cannot start with Twitch chat commands!");
                return;
            }

            if (memeName.StartsWith(InputMacroPreparser.DEFAULT_MACRO_START) == true)
            {
                QueueMessage($"Memes cannot start with \"{InputMacroPreparser.DEFAULT_MACRO_START}\".");
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

                string actualMemeValue = args.Command.ArgumentsAsString.Remove(0, memeNameLength + 1);

                if (meme != null)
                {
                    meme.MemeValue = actualMemeValue;

                    QueueMessage($"Meme \"{memeToLower}\" overwritten!");
                }
                else
                {
                    Meme newMeme = new Meme(memeToLower, actualMemeValue);
                    context.Memes.Add(newMeme);

                    QueueMessage($"Added meme \"{memeToLower}\"!");
                }

                context.SaveChanges();
            }
        }
    }
}
