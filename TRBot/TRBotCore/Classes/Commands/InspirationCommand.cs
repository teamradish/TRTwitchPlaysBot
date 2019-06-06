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
            int randinspiration = Rand.Next(0, RandomInspiration.Length);

            string message = RandomInspiration[randinspiration].Replace("{name}", e.Command.ChatMessage.DisplayName);

            BotProgram.QueueMessage(message);
        }
    }
}
