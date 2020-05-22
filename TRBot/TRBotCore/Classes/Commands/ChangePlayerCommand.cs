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
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Changes which player, or controller port, a user is on.
    /// </summary>
    public sealed class ChangePlayerCommand : BaseCommand
    {
        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count == 0 || args.Count > 1)
            {
                BotProgram.QueueMessage("Usage: \"controllerPort (starting from 1)\"");
                return;
            }

            if (int.TryParse(args[0], out int portNum) == false)
            {
                BotProgram.QueueMessage("That is not a valid number!");
                return;
            }

            if (portNum <= 0 || portNum > InputGlobals.ControllerMngr.ControllerCount)
            {
                BotProgram.QueueMessage($"Please specify a number in the range of 1 through the current controller count ({InputGlobals.ControllerMngr.ControllerCount}).");
                return;
            }

            //Change to zero-based index for referencing
            int controllerNum = portNum - 1;

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            if (user.Team == controllerNum)
            {
                BotProgram.QueueMessage("You're already on this controller port!");
                return;
            }

            //Change team and save data
            user.SetTeam(controllerNum);

            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Changed controller port to {portNum}!");
        }
    }
}
