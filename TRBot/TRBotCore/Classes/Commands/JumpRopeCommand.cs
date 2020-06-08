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
using TwitchLib;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace TRBot
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

        public override void ExecuteCommand(EvtChatCommandArgs e)
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

                    BotProgram.MsgHandler.QueueMessage($"Ouch! I tripped and fell after {JumpRopeCount} attempt(s) at Jump Rope! Wow, it's a new record!");
                }
                else
                {
                    BotProgram.MsgHandler.QueueMessage($"Ouch! I tripped and fell after {JumpRopeCount} attempt(s) at Jump Rope!");
                }

                JumpRopeCount = 0;

                RandJumpRopeChance = Rand.Next(4, 9);
            }
            else
            {
                JumpRopeCount++;
                BotProgram.MsgHandler.QueueMessage($"Yay :D I succeeded in Jump Rope {JumpRopeCount} time(s) in a row!");
            }
        }
    }
}
