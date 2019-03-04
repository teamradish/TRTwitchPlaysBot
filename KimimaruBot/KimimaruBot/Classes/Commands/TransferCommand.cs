using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class TransferCommand : BaseCommand
    {
        public TransferCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args == null || args.Count != 2)
            {
                BotProgram.QueueMessage("Please specify a single user followed by the amount of credits you wish to transfer!");
                return;
            }

            string giver = e.Command.ChatMessage.DisplayName;
            string giverToLower = giver.ToLower();

            string receiver = args[0];
            string receiverToLower = receiver.ToLower();

            if (giverToLower == receiverToLower)
            {
                BotProgram.QueueMessage("You cannot transfer points to yourself!");
                return;
            }

            //If the user transferring points isn't in the database, add them
            if (CreditsCommand.UserCredits.ContainsKey(giverToLower) == false)
            {
                CreditsCommand.UserCredits.Add(giverToLower, 0);
                CreditsCommand.SaveDict();
            }

            if (CreditsCommand.UserCredits.ContainsKey(receiverToLower) == false)
            {
                BotProgram.QueueMessage($"{receiver} is not in the database!");
                return;
            }

            long transferAmount = -1L;
            bool success = long.TryParse(args[1], out transferAmount);
            if (success == false || transferAmount <= 0)
            {
                BotProgram.QueueMessage("Please specify a positive whole number of credits greater than 0!");
                return;
            }

            if (CreditsCommand.UserCredits[giverToLower] < transferAmount)
            {
                BotProgram.QueueMessage("The transfer amount is greater than your credits!");
                return;
            }

            CreditsCommand.UserCredits[giverToLower] -= transferAmount;
            CreditsCommand.UserCredits[receiverToLower] += transferAmount;
            CreditsCommand.SaveDict();

            BotProgram.QueueMessage($"{giver} has transferred {transferAmount} points to {receiver} :D !");
        }
    }
}
