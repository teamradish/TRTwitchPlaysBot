using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class BetCommand : BaseCommand
    {
        public Random Rand = new Random();

        public BetCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> amount = e.Command.ArgumentsAsList;

            if (amount.Count != 1)
            {
                BotProgram.QueueMessage($"Sorry, please enter a valid bet amount!");
                return;
            }

            if (long.TryParse(amount[0], out long creditBet) == true)
            {
                if (creditBet <= 0)
                {
                    BotProgram.QueueMessage("Bet amount must be greater than or equal to 0!");
                    return;
                }

                string name = e.Command.ChatMessage.DisplayName;
                string nameToLower = name.ToLower();

                if (CreditsCommand.UserCredits.ContainsKey(nameToLower) == false)
                {
                    CreditsCommand.UserCredits.Add(nameToLower, 0);
                    CreditsCommand.SaveDict();
                }

                long credits = CreditsCommand.UserCredits[nameToLower];

                if (creditBet > credits)
                {
                    BotProgram.QueueMessage("Bet amount is greater than credits!");
                }
                else
                {
                    bool success = (Rand.Next(0, 2) == 0);
                    string message = string.Empty;

                    if (success)
                    {
                        credits += creditBet;
                        message = $"{name} won {creditBet} credits :D !";
                    }
                    else
                    {
                        credits -= creditBet;
                        message = $"{name} lost {creditBet} credits :(";
                    }

                    BotProgram.QueueMessage(message);

                    CreditsCommand.UserCredits[nameToLower] = credits;
                    CreditsCommand.SaveDict();
                }
            }
            else
            {
                BotProgram.QueueMessage($"Sorry, please enter a valid bet amount!");
            }
        }
    }
}
