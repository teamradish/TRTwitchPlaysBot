using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class DenyCommand : BaseCommand
    {
        public DenyCommand()
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

                BotProgram.QueueMessage($"{name} has denied to duel with {data.UserDueling} and miss out on a potential {data.BetAmount} credit(s)!");
            }
            else
            {
                BotProgram.QueueMessage("You are not in a duel or your duel has expired!");
            }
        }
    }
}
