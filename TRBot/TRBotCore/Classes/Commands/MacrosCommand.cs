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
using System.Text;
using System.Linq;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class MacrosCommand : BaseCommand
    {
        private StringBuilder StrBuilder = new StringBuilder(500);
        private List<string> MultiMessageCache = new List<string>(16);
        private const string InitMessage = "Macros: ";

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotData.Macros.Count == 0)
            {
                BotProgram.QueueMessage("There are no macros!");
                return;
            }

            List<string> macros = BotProgram.BotData.Macros.Keys.ToList();

            MultiMessageCache.Clear();
            StrBuilder.Clear();

            for (int i = 0; i < macros.Count; i++)
            {
                string macroName = macros[i];

                int newLength = StrBuilder.Length + macroName.Length + 3;
                int maxLength = Globals.TwitchCharacterLimit;
                if (MultiMessageCache.Count == 0)
                {
                    maxLength -= InitMessage.Length;
                }

                //Send in multiple messages if it exceeds the length
                if (newLength >= maxLength)
                {
                    MultiMessageCache.Add(StrBuilder.ToString());
                    StrBuilder.Clear();
                }

                StrBuilder.Append(macroName).Append(", ");
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            MultiMessageCache.Add(StrBuilder.ToString());

            for (int i = 0; i < MultiMessageCache.Count; i++)
            {
                if (i == 0)
                {
                    BotProgram.QueueMessage($"{InitMessage}{MultiMessageCache[i]}");
                }
                else
                {
                    BotProgram.QueueMessage(MultiMessageCache[i]);
                }
            }
        }
    }
}
