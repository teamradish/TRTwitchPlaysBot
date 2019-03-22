using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace KimimaruBot
{
    public sealed class JumpRopeCommand : BaseCommand
    {
        private Random Rand = new Random();
        public int JumpRopeCount = 0;
        public int RandJumpRopeChance = 5;

        public JumpRopeCommand()
        {
            RandJumpRopeChance = Rand.Next(4, 9);
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            int randNum = 1;
            try
            {
                randNum = Rand.Next(0, RandJumpRopeChance);
            }
            catch(Exception exc)
            {
                Console.WriteLine("EXCEPTION: " + exc.Message);
            }

            if (randNum == 0 && JumpRopeCount > 0)
            {
                if (JumpRopeCount > BotProgram.BotData.JRData.Streak)
                {
                    BotProgram.BotData.JRData.User = e.Command.ChatMessage.Username;
                    BotProgram.BotData.JRData.Streak = JumpRopeCount;
                    BotProgram.SaveBotData();

                    BotProgram.QueueMessage($"Ouch! I tripped and fell after {JumpRopeCount} attempt(s) at Jump Rope! Wow, it's a new record!");
                }
                else
                {
                    BotProgram.QueueMessage($"Ouch! I tripped and fell after {JumpRopeCount} attempt(s) at Jump Rope!");
                }

                JumpRopeCount = 0;

                RandJumpRopeChance = Rand.Next(4, 9);
            }
            else
            {
                JumpRopeCount++;
                BotProgram.QueueMessage($"Yay :D I succeeded in Jump Rope {JumpRopeCount} time(s) in a row!");
            }
        }
    }
}
