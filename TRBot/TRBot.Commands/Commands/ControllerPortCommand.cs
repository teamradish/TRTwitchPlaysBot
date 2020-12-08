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
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Displays or changes which controller port a user is on.
    /// </summary>
    public sealed class ControllerPortCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"controllerPort (int - starting from 1) (optional)\"";

        public ControllerPortCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            string userName = args.Command.ChatMessage.Username;

            User user = DataHelper.GetUser(userName);

            //Display controller port with no arguments
            if (arguments.Count == 0)
            {
                if (user == null)
                {
                    QueueMessage("Somehow, you're an invalid user not in the database, so I can't display your controller port.");
                }
                else
                {
                    QueueMessage($"Your controller port is {user.ControllerPort + 1}!");
                }

                return;
            }

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            if (int.TryParse(arguments[0], out int portNum) == false)
            {
                QueueMessage("That is not a valid number!");
                return;
            }

            int controllerCount = DataContainer.ControllerMngr.ControllerCount;

            if (portNum <= 0 || portNum > controllerCount)
            {
                QueueMessage($"Please specify a number in the range of 1 through the current controller count ({controllerCount}).");
                return;
            }

            //Change to zero-based index for referencing
            int controllerNum = portNum - 1;

            if (user == null)
            {
                QueueMessage("Somehow, you're an invalid user not in the database, so I can't change your controller port.");
                return;
            }

            if (user.ControllerPort == controllerNum)
            {
                QueueMessage("You're already on this controller port!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                user = DataHelper.GetUserNoOpen(userName, context);

                //Change port and save data
                user.ControllerPort = controllerNum;

                context.SaveChanges();
            }

            QueueMessage($"{user.Name} changed their controller port to {portNum}!");
        }
    }
}
