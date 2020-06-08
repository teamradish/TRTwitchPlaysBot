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
using TwitchLib;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class RandNumCommand : BaseCommand
    {
        public Random Rand = new Random();

        public RandNumCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> argList = e.Command.ArgumentsAsList;

            if (argList.Count <= 1 || argList.Count >= 3)
            {
                BotProgram.MsgHandler.QueueMessage("I'm sorry; please enter a range of two whole numbers!");
            }
            else
            {
                if (int.TryParse(argList[0], out int num1) && int.TryParse(argList[1], out int num2))
                {
                    int min = Math.Min(num1, num2);
                    int max = Math.Max(num1, num2);

                    int randNum = Rand.Next(min, max);
                    BotProgram.MsgHandler.QueueMessage($"The random number between {min} and {max} is: {randNum}! Play again!");
                }
                else
                {
                    BotProgram.MsgHandler.QueueMessage("I'm sorry; please enter a range of two whole numbers!");
                }
            }
        }
    }
}
