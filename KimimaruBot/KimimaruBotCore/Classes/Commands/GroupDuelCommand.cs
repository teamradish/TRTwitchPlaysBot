using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class GroupDuelCommand : BaseCommand
    {
        public const int MinUsersForDuel = 3;
        private const int GroupDuelMinutes = 2;
        public static readonly Dictionary<string, long> UsersInDuel = new Dictionary<string, long>();

        public static bool DuelStarted = false;

        public GroupDuelCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage("Please specify a bet amount!");
                return;
            }

            string displayName = e.Command.ChatMessage.DisplayName;
            string displayLower = displayName.ToLower();

            //If the user participating in the group duel isn't in the database, add them
            User user = BotProgram.GetOrAddUser(displayLower, true);

            //Check if we can parse the bet amount
            long betAmount = -1L;
            bool success = long.TryParse(args[0], out betAmount);
            if (success == false || betAmount <= 0)
            {
                BotProgram.QueueMessage("Please specify a positive whole number of credits greater than 0!");
                return;
            }

            if (user.Credits < betAmount)
            {
                BotProgram.QueueMessage("You don't have enough credits to bet this much!");
                return;
            }

            string message = string.Empty;

            bool prevStarted = DuelStarted;

            //Check if the user isn't in the duel
            if (UsersInDuel.ContainsKey(displayLower) == false)
            {
                //Add them
                UsersInDuel.Add(displayLower, betAmount);
                message = $"{displayName} entered the group duel with {betAmount} credit(s)!";

                int count = UsersInDuel.Count;
                if (count < MinUsersForDuel)
                {
                    int diff = MinUsersForDuel - count;

                    message += $" {diff} more user(s) are required to start the group duel!";
                }
                else
                {
                    //Start duel
                    if (DuelStarted == false)
                    {
                        StartGroupDuel();
                    }
                }
            }
            //The user is already in the group duel, so adjust their bet
            else
            {
                long prevBet = UsersInDuel[displayLower];

                message = $"{displayName} adjusted their group duel bet from {prevBet} to {betAmount} credit(s)!";

                UsersInDuel[displayLower] = betAmount;
            }

            BotProgram.QueueMessage(message);

            if (prevStarted == false && DuelStarted == true)
            {
                BotProgram.QueueMessage($"The group duel has enough participants and will start in {GroupDuelMinutes} minute(s), so join before then if you want in!");
            }
        }

        public static void StartGroupDuel()
        {
            if (DuelStarted == true)
            {
                Console.WriteLine("****Can't start group duel since it already started!****");
                return;
            }

            BotProgram.AddRoutine(new GroupDuelRoutine(GroupDuelMinutes));

            DuelStarted = true;
        }

        public static void StopGroupDuel()
        {
            if (DuelStarted == false)
            {
                Console.WriteLine("****Can't stop group duel since it hasn't started!****");
                return;
            }

            BotProgram.RemoveRoutine(BotProgram.FindRoutine<GroupDuelRoutine>());

            DuelStarted = false;
        }
    }
}
