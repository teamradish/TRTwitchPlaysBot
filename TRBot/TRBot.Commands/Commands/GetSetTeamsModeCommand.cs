/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
    /// Enables or disables teams mode.
    /// </summary>
    public sealed class GetSetTeamsModeCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"true\" or \"false\"";

        public GetSetTeamsModeCommand()
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
                long teamsModeVal = DataHelper.GetSettingInt(SettingsConstants.TEAMS_MODE_ENABLED, 0L);
                string enabledStr = (teamsModeVal <= 0) ? "disabled" : "enabled";

                QueueMessage($"Teams mode is currently {enabledStr}. To change the teams mode state, pass either \"true\" or \"false\" as an argument."); 
                return;
            }

            //Check for sufficient permissions
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_TEAMS_MODE_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set teams mode!");
                    return;
                }
            }

            string newStateStr = arguments[0].ToLowerInvariant();

            if (bool.TryParse(newStateStr, out bool newState) == false)
            {
                QueueMessage("Invalid state argument. To change the teams mode state, pass either \"true\" or \"false\" as an argument.");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings teamsMode = DataHelper.GetSettingNoOpen(SettingsConstants.TEAMS_MODE_ENABLED, context);
                if (teamsMode == null)
                {
                    teamsMode = new Settings(SettingsConstants.TEAMS_MODE_ENABLED, string.Empty, 0L);
                    context.SettingCollection.Add(teamsMode);
                }

                teamsMode.ValueInt = (newState == true) ? 1L : 0L;

                context.SaveChanges();
            }

            if (newState == true)
            {
                QueueMessage("Enabled teams mode! New users will be assigned different controller ports based on settings.");
            }
            else
            {
                QueueMessage("Disabled teams mode. New users will be assigned controller port 1.");
            }
        }
    }
}
