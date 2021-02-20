/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that changes the periodic input port.
    /// </summary>
    public sealed class GetSetPeriodicInputPortCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - no arguments (get value) or \"controller port (int)\"";

        public GetSetPeriodicInputPortCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Ignore with too few arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            if (arguments.Count == 0)
            {
                long periodicInputPort = DataHelper.GetSettingInt(SettingsConstants.PERIODIC_INPUT_PORT, 0L);

                QueueMessage($"The default port for periodic inputs is currently {periodicInputPort + 1}. To change the periodic input port, pass a controller port as an argument."); 
                return;
            }

            //Check for sufficient permissions
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_PERIODIC_INPUT_PORT_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the default periodic input port!");
                    return;
                }
            }

            string portValStr = arguments[0].ToLowerInvariant();

            if (int.TryParse(portValStr, out int newPort) == false)
            {
                QueueMessage("That's not a valid number!");
                return;
            }

            int controllerCount = DataContainer.ControllerMngr.ControllerCount;

            if (newPort <= 0 || newPort > controllerCount)
            {
                QueueMessage($"Please specify a number in the range of 1 through the current controller count ({controllerCount}).");
                return;
            }

            //Change to zero-based index for referencing
            int controllerNum = newPort - 1;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings periodicInputPort = DataHelper.GetSettingNoOpen(SettingsConstants.PERIODIC_INPUT_PORT, context);
                if (periodicInputPort == null)
                {
                    periodicInputPort = new Settings(SettingsConstants.PERIODIC_INPUT_PORT, string.Empty, 0L);
                    context.SettingCollection.Add(periodicInputPort);
                }

                periodicInputPort.ValueInt = controllerNum;

                context.SaveChanges();
            }

            QueueMessage($"Set the default periodic input controller port to {newPort}!"); 
        }
    }
}
