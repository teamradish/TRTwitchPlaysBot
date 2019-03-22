using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class AcceptCommand : BaseCommand
    {
        public Random Rand = new Random();

        public AcceptCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string name = e.Command.ChatMessage.DisplayName;
            string nameToLower = name.ToLower();

            if (DuelCommand.DuelRequests.ContainsKey(nameToLower) == true)
            {
                DuelCommand.DuelData data = DuelCommand.DuelRequests[nameToLower];
                DuelCommand.DuelRequests.Remove(nameToLower);

                TimeSpan diff = DateTime.Now - data.CurDuelTime;

                if (diff.TotalMinutes >= DuelCommand.DUEL_MINUTES)
                {
                    BotProgram.QueueMessage("You are not in a duel or your duel has expired!");
                    return;
                }

                long betAmount = data.BetAmount;
                string dueled = data.UserDueling;
                string dueledToLower = dueled.ToLower();

                User duelerUser = BotProgram.GetUser(nameToLower);
                User dueledUser = BotProgram.GetUser(dueledToLower);

                //First confirm both users have enough credits for the duel, as they could've lost some in that time
                if (duelerUser.Credits < betAmount || dueledUser.Credits < betAmount)
                {
                    BotProgram.QueueMessage("At least one user involved in the duel no longer has enough points for the duel! The duel is off!");
                    return;
                }

                //50/50 chance of either user winning
                int val = Rand.Next(0, 2);

                string message = string.Empty;

                if (val == 0)
                {
                    duelerUser.Credits += betAmount;
                    dueledUser.Credits -= betAmount;

                    message = $"{name} won the bet against {dueled} for {betAmount} credit(s)!";
                }
                else
                {
                    duelerUser.Credits -= betAmount;
                    dueledUser.Credits += betAmount;

                    message = $"{dueled} won the bet against {name} for {betAmount} credit(s)!";
                }

                BotProgram.SaveBotData();

                BotProgram.QueueMessage(message);
            }
            else
            {
                BotProgram.QueueMessage("You are not in a duel or your duel has expired!");
            }
        }
    }
}
