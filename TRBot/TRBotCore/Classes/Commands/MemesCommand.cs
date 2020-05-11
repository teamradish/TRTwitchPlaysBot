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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace TRBot
{
    public sealed class MemesCommand : BaseCommand
    {
        private static List<string> MemesCache = new List<string>(16);
        private const string InitMessage = "Here is the list of memes: ";

        public MemesCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            CacheMemesString();
        }

        public static void CacheMemesString()
        {
            MemesCache.Clear();

            string curString = string.Empty;

            //List all memes
            string[] memes = BotProgram.BotData.Memes.Keys.ToArray();

            for (int i = 0; i < memes.Length; i++)
            {
                int length = memes[i].Length + curString.Length;
                int maxLength = Globals.TwitchCharacterLimit;
                if (MemesCache.Count == 0)
                {
                    maxLength -= InitMessage.Length;
                }

                if (length >= maxLength)
                {
                    MemesCache.Add(curString);
                    curString = string.Empty;
                }

                curString += memes[i];

                if (i < (memes.Length - 1))
                {
                    curString += ", ";
                }
            }

            if (string.IsNullOrEmpty(curString) == false)
            {
                MemesCache.Add(curString);
            }
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (MemesCache.Count == 0)
            {
                BotProgram.QueueMessage("There are none!");
                return;
            }

            for (int i = 0; i < MemesCache.Count; i++)
            {
                string message = (i == 0) ? InitMessage : string.Empty;
                message += MemesCache[i];
                BotProgram.QueueMessage(message);
            }
        }
    }
}
