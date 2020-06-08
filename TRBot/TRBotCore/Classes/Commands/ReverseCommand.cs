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
using System.Data;
using System.Numerics;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using NCalc;

namespace TRBot
{
    /// <summary>
    /// Reverses a message.
    /// </summary>
    public sealed class ReverseCommand : BaseCommand
    {
        public ReverseCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count == 0)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"message\"");
                return;
            }

            char[] msgArr = e.Command.ArgumentsAsString.ToCharArray();
            Array.Reverse(msgArr);
            
            string msg = new string(msgArr);
            
            //Ignore any output starting with a "/" to avoid exploiting Twitch chat commands
            if (msg.StartsWith('/') == true)
            {
                BotProgram.MsgHandler.QueueMessage("!uoy rof sdnammoc tahc hctiwT oN");
                return;
            }

            BotProgram.MsgHandler.QueueMessage(msg);
        }
    }
}
