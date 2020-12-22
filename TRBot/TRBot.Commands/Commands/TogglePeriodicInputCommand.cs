/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Routines;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that toggles the periodic input on or off.
    /// </summary>
    public sealed class TogglePeriodicInputCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - no arguments (get value) or \"true\" or \"false\"";

        public TogglePeriodicInputCommand()
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
                long periodicInputVal = DataHelper.GetSettingInt(SettingsConstants.PERIODIC_INPUT_ENABLED, 0L);
                string enabledStr = (periodicInputVal <= 0) ? "disabled" : "enabled";

                QueueMessage($"The periodic input is currently {enabledStr}. To change the periodic input enabled state, pass either \"true\" or \"false\" as an argument."); 
                return;
            }

            //Check for sufficient permissions
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_PERIODIC_INPUT_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to change the periodic input state!");
                    return;
                }
            }

            string newStateStr = arguments[0].ToLowerInvariant();

            if (bool.TryParse(newStateStr, out bool newState) == false)
            {
                QueueMessage("Invalid argument. To enable or disable the periodic input, pass either \"true\" or \"false\" as an argument.");
                return;
            }

            long newVal = (newState == true) ? 1L : 0L;
            bool oldState = false;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings periodicInput = DataHelper.GetSettingNoOpen(SettingsConstants.PERIODIC_INPUT_ENABLED, context);
                if (periodicInput == null)
                {
                    periodicInput = new Settings(SettingsConstants.PERIODIC_INPUT_ENABLED, string.Empty, 0L);
                    context.SettingCollection.Add(periodicInput);

                    context.SaveChanges();
                }

                //Same value - don't make changes
                if ((periodicInput.ValueInt > 0 && newVal > 0) || (periodicInput.ValueInt <= 0 && newVal <= 0))
                {
                    QueueMessage("The periodic input state is already this value!");
                    return;
                }

                oldState = (periodicInput.ValueInt > 0) ? true : false;

                periodicInput.ValueInt = newVal;

                context.SaveChanges();
            }

            //Now add or remove the routine if we should
            if (oldState == true && newState == false)
            {
                RoutineHandler.RemoveRoutine(RoutineConstants.PERIODIC_INPUT_ROUTINE_ID);
            }
            //Add the routine
            else if (oldState == false && newState == true)
            {
                BaseRoutine routine = RoutineHandler.FindRoutine(RoutineConstants.PERIODIC_INPUT_ROUTINE_ID, out int indexFound);
                if (routine == null)
                {
                    RoutineHandler.AddRoutine(new PeriodicInputRoutine());
                }
                else
                {
                    QueueMessage("Huh? The periodic input routine is somehow already running even though it was just enabled.");
                }
            }

            if (newState == true)
            {
                QueueMessage("Enabled periodic inputs. Every now and then, an input sequence will be automatically performed.");
            }
            else
            {
                QueueMessage("Disabled periodic inputs.");
            }
        }
    }
}
