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
    public sealed class InspirationCommand : BaseCommand
    {
        private readonly string[] RandomInspiration = new string[]
        {
            "You can do it {name}!",
            "You've got this, {name}!",
            "I'm rooting for you, {name}!",
            "{name}, awesome job; keep it up!",
            "Keep going, {name}!",
            "{name}, you've got this in the bag!",
            "{name}, you're such a great team player!",
            "You're the best, {name}!",
            "You're my best friend, {name}!",
            "Don't give up, {name}!"
        };

        public Random Rand = new Random();

        public InspirationCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string userName = e.Command.ChatMessage.DisplayName;

            //Allow inspiring another user
            List<string> args = e.Command.ArgumentsAsList;
            if (args.Count == 1)
            {
                //Check if the name is in the database
                User user = BotProgram.GetUser(args[0], false);
                if (user != null)
                {
                    userName = user.Name;
                }
            }

            int randinspiration = Rand.Next(0, RandomInspiration.Length);

            string message = RandomInspiration[randinspiration].Replace("{name}", userName);

            BotProgram.QueueMessage(message);
        }
    }
}
