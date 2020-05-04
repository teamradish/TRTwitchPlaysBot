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
    public sealed class CreditsCommand : BaseCommand
    {
        public CreditsCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> arguments = e.Command.ArgumentsAsList;

            if (arguments == null || (arguments.Count <= 1 || arguments.Count > 2))
            {
                bool sameName = true;
                string userName = e.Command.ChatMessage.DisplayName;

                if (arguments != null && arguments.Count > 0)
                {
                    //Allow seeing other users' credits count
                    if (userName != arguments[0])
                    {
                        userName = arguments[0];
                        sameName = false;
                    }
                }

                string userLower = userName.ToLower();

                if (BotProgram.BotData.Users.ContainsKey(userLower) == false)
                {
                    if (sameName == true)
                    {
                        //Kimimaru: comment out for now - unsure if commands would happen before messages
                        //UserCredits.Add(userLower, 0);
                        //
                        //SaveDict();
                    }
                    else
                    {
                        BotProgram.QueueMessage($"{userName} is not in the database.");
                        return;
                    }
                }

                User user = BotProgram.BotData.Users[userLower];
                if (user == null)
                {
                    return;
                }

                if (user.OptedOut == true)
                {
                    BotProgram.QueueMessage("This user opted out of bot stats, so you can't see their credits.");
                    return;
                }

                BotProgram.QueueMessage($"{userName} has {BotProgram.BotData.Users[userLower].Credits} credit(s)!");
            }
            else
            {
                //Compare credits
                string name1 = arguments[0];
                string name2 = arguments[1];

                string name1Lower = name1.ToLower();
                string name2Lower = name2.ToLower();

                if (BotProgram.BotData.Users.ContainsKey(name1Lower) == false)
                {
                    BotProgram.QueueMessage($"{name1} is not in the database!");
                    return;
                }
                if (BotProgram.BotData.Users.ContainsKey(name2Lower) == false)
                {
                    BotProgram.QueueMessage($"{name2} is not in the database!");
                    return;
                }

                User user1 = BotProgram.BotData.Users[name1Lower];
                User user2 = BotProgram.BotData.Users[name2Lower];

                if (user1.OptedOut == true || user2.OptedOut == true)
                {
                    BotProgram.QueueMessage("At least one of these users opted out of bot stats, so you can't see their credits.");
                    return;
                }

                long credits1 = BotProgram.BotData.Users[name1Lower].Credits;
                long credits2 = BotProgram.BotData.Users[name2Lower].Credits;

                string message = string.Empty;
                if (credits1 < credits2)
                {
                    long diff = credits2 - credits1;
                    message = $"{name1} has {diff} fewer credit(s) than {name2}!";
                }
                else if (credits1 > credits2)
                {
                    long diff = credits1 - credits2;
                    message = $"{name1} has {diff} more credit(s) than {name2}!";
                }
                else
                {
                    message = $"{name1} and {name2} have an equal number of credits!";
                }

                BotProgram.QueueMessage(message);
            }
        }
    }
}
