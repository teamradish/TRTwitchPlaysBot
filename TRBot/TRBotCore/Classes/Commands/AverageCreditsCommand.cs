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
    public sealed class AverageCreditsCommand : BaseCommand
    {
        public AverageCreditsCommand()
        {

        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            User[] users = BotProgram.BotData.Users.Values.ToArray();

            long average = 0L;

            for (int i = 0; i < users.Length; i++)
            {
                //Don't include opted out users
                if (users[i].OptedOut == false)
                {
                    average += users[i].Credits;
                }
            }

            average /= users.Length;

            BotProgram.QueueMessage($"The average number of credits in the database is {average}!");
        }
    }
}
