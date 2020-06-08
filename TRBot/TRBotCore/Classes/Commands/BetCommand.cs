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
    public sealed class BetCommand : BaseCommand
    {
        public Random Rand = new Random();

        public BetCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> amount = e.Command.ArgumentsAsList;

            if (amount.Count != 1)
            {
                BotProgram.MsgHandler.QueueMessage($"Sorry, please enter a valid bet amount!");
                return;
            }

            if (long.TryParse(amount[0], out long creditBet) == true)
            {
                if (creditBet <= 0)
                {
                    BotProgram.MsgHandler.QueueMessage("Bet amount must be greater than 0!");
                    return;
                }

                string name = e.Command.ChatMessage.DisplayName;
                string nameToLower = name.ToLower();

                User user = BotProgram.GetOrAddUser(nameToLower);
                if (user == null)
                {
                    return;
                }

                if (user.OptedOut == true)
                {
                    BotProgram.MsgHandler.QueueMessage("You cannot bet while opted out of bot stats.");
                    return;
                }

                if (creditBet > user.Credits)
                {
                    BotProgram.MsgHandler.QueueMessage("Bet amount is greater than credits!");
                }
                else
                {
                    bool success = (Rand.Next(0, 2) == 0);
                    string message = string.Empty;

                    if (success)
                    {
                        user.AddCredits(creditBet);
                        message = $"{name} won {creditBet} credits :D !";
                    }
                    else
                    {
                        user.SubtractCredits(creditBet);
                        message = $"{name} lost {creditBet} credits :(";
                    }

                    BotProgram.MsgHandler.QueueMessage(message);

                    BotProgram.SaveBotData();
                }
            }
            else
            {
                BotProgram.MsgHandler.QueueMessage($"Sorry, please enter a valid bet amount!");
            }
        }
    }
}
