/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that obtains a pseudo-random number in a range.
    /// </summary>
    public class RandomNumberCommand : BaseCommand
    {
        private Random Rand = new Random();
        private string UsageMessage = "Usage: \"min number (int - inclusive)\" \"max number (int - exclusive)\"";

        public RandomNumberCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Parse the numbers
            if (int.TryParse(arguments[0], out int num1) == true && int.TryParse(arguments[1], out int num2) == true)
            {
                //Get the true min and max values
                int min = Math.Min(num1, num2);
                int max = Math.Max(num1, num2);
                
                int randNum = Rand.Next(min, max);

                QueueMessage($"The random number between {min} and {max} is: {randNum}! Play again!");
            }
            else
            {
                QueueMessage(UsageMessage);
            }
        }
    }
}
