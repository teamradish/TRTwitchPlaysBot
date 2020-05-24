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
using TwitchLib;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class SayCommand : BaseCommand
    {
        public SayCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            if (e.Command.ChatMessage.Message.Length > 5)
            {
                string realMsg = e.Command.ChatMessage.Message.Remove(0, 5).Trim();// + ". The statement said to me expresses the views of the one instructing me, not myself :D";
                if (realMsg.StartsWith("/") == true)
                {
                    BotProgram.QueueMessage("I can't say any Twitch chat commands for you - no hard feelings!");
                }
                else
                {
                    BotProgram.QueueMessage(realMsg);
                }
            }
        }
    }
}
