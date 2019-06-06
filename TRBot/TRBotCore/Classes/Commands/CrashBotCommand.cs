using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class CrashBotCommand : BaseCommand
    {
        private List<string> Messages = new List<string>()
        {
            "Nice try Kappa",
            "Nice try Kappa",
            "Nice try Kappa",
            "Try again Kappa",
            "Try again Kappa",
            "Nice try Kappa",
            "Good luck this time Kappa",
            "Try again Kappa",
            "Not yet Kappa",
            "Not yet Kappa",
            "Nice try Kappa",
            "Nice try Kappa",
            "Nice try Kappa",
            "Nice try Kappa",
            "Almost there Kappa",
            "Almost there Kappa",
            "Almost there Kappa",
            "Almost there Kappa",
            "BOOM!",
            "/me has been timed out for 600 seconds."
        };

        private int Attempts = 0;

        public CrashBotCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage(Messages[Attempts]);

            if (Attempts >= (Messages.Count - 2))
            {
                BotProgram.QueueMessage(Messages[Attempts + 1]);

                Attempts = 0;
            }
            else
            {
                Attempts++;
            }
        }
    }
}
