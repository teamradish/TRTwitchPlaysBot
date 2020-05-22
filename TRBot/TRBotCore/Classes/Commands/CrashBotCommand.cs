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

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
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
