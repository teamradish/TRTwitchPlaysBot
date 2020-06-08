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
    public sealed class MedianCreditsCommand : BaseCommand
    {
        private Comparison<(string, long)> CreditsCompare = null;

        public MedianCreditsCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            //Set here to reduce garbage since it normally creates a new delegate instance each time you pass in the method name directly
            CreditsCompare = CompareKeyVal;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<(string, long)> allCredits = new List<(string, long)>(BotProgram.BotData.Users.Count);

            foreach (var kvPair in BotProgram.BotData.Users)
            {
                //Don't include opted out users
                if (kvPair.Value.OptedOut == true)
                {
                    continue;
                }

                allCredits.Add((kvPair.Key, kvPair.Value.Credits));
            }

            allCredits.Sort(CreditsCompare);

            int medianIndex = allCredits.Count / 2;

            if (medianIndex >= allCredits.Count)
            {
                BotProgram.MsgHandler.QueueMessage("Sorry, there's not enough data in the credits database for that!");
            }
            else
            {
                long median = allCredits[medianIndex].Item2;

                BotProgram.MsgHandler.QueueMessage($"The median number of credits in the database is {median}!");
            }
        }

        private int CompareKeyVal((string, long) val1, (string, long) val2)
        {
            return val1.Item2.CompareTo(val2.Item2);
        }
    }
}
