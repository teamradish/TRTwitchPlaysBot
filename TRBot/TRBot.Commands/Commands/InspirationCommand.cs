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
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Give another user some inspiration.
    /// </summary>
    public class InspirationCommand : BaseCommand
    {
        private const string NAME_REPLACE = "{name}";
        private const char SPLIT_CHAR = '|';

        private readonly string[] DefaultInspiration = new string[]
        {
            $"You can do it {NAME_REPLACE}!",
            $"You've got this, {NAME_REPLACE}!",
            $"I'm rooting for you, {NAME_REPLACE}!",
            $"{NAME_REPLACE}, awesome job; keep it up!",
            $"Keep going, {NAME_REPLACE}!",
            $"{NAME_REPLACE}, you've got this in the bag!",
            $"{NAME_REPLACE}, you're such a great team player!",
            $"You're the best, {NAME_REPLACE}!",
            $"You're my best friend, {NAME_REPLACE}!",
            $"Don't give up, {NAME_REPLACE}!"
        };

        private Random Rand = new Random();

        public InspirationCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            //Values are separated by the pipe character, '|'
            //Replace "{name}" with the user's name

            string userName = args.Command.ChatMessage.Username;

            //Allow inspiring another user
            List<string> arguments = args.Command.ArgumentsAsList;
            if (arguments.Count == 1)
            {
                //Check if the name is in the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User user = DataHelper.GetUserNoOpen(arguments[0], context);
                    if (user != null)
                    {
                        userName = user.Name;
                    }
                }
            }

            string[] inspirationArray = DefaultInspiration;

            if (string.IsNullOrEmpty(ValueStr) == false)
            {
                string[] newInspirationArray = ValueStr.Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);

                //Use the array from the values if there are elements
                if (newInspirationArray.Length > 0)
                {
                    inspirationArray = newInspirationArray;
                }
            }

            int randInspiration = Rand.Next(0, inspirationArray.Length);
            string inspirationMsg = inspirationArray[randInspiration];

            string message = inspirationMsg.Replace(NAME_REPLACE, userName);

            QueueMessage(message);
        }
    }
}
