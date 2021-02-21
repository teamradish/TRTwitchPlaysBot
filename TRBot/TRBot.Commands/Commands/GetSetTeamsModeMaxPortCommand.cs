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
using TRBot.Consoles;
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Obtains or sets the max controller port for teams mode.
    /// </summary>
    public sealed class GetSetTeamsModeMaxPortCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"zero-based max controller port (int)\"";

        public GetSetTeamsModeMaxPortCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            if (arguments.Count == 0)
            {
                long teamsModePort = DataHelper.GetSettingInt(SettingsConstants.TEAMS_MODE_MAX_PORT, 0L);

                QueueMessage($"The max controller port for teams mode is currently {teamsModePort}. To change the max controller port for teams mode, pass it as an argument zero-based (Ex. 0 = port 1)."); 
                return;
            }

            //Check for sufficient permissions
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_TEAMS_MODE_MAX_PORT_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the max controller port for teams mode!");
                    return;
                }
            }

            string portStr = arguments[0];

            if (int.TryParse(portStr, out int newPort) == false)
            {
                QueueMessage("Invalid port argument. To change the max controller port for teams mode, pass it as an argument zero-based (Ex. 0 = port 1).");
                return;
            }

            //Ensure port number is valid
            if (newPort <= 0)
            {
                QueueMessage("The max controller port for teams mode must be greater than 0 (port 1)!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings teamsModeMaxPort = DataHelper.GetSettingNoOpen(SettingsConstants.TEAMS_MODE_MAX_PORT, context);
                if (teamsModeMaxPort == null)
                {
                    teamsModeMaxPort = new Settings(SettingsConstants.TEAMS_MODE_MAX_PORT, string.Empty, 0L);
                }

                teamsModeMaxPort.ValueInt = newPort;

                context.SaveChanges();
            }

            QueueMessage($"Set the maximum controller port for teams mode to {newPort}!");

            int controllerCount = DataContainer.ControllerMngr.ControllerCount;

            //Issue a warning if this value is set greater than the available controller count
            if (newPort >= controllerCount)
            {
                QueueMessage($"Warning: New max controller port of {newPort} for teams mode is greater than or equal to the available number of virtual controllers, {controllerCount}. New users assigned to ports greater than or equal to {controllerCount} (port {controllerCount + 1}) will need to manually lower their controller ports to play. Consider using a value lower than {controllerCount} for the max teams mode port.");
            }
        }
    }
}
