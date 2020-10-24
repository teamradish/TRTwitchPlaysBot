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
using TRBot.VirtualControllers;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Misc;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Sets the number of controllers used.
    /// </summary>
    public sealed class ControllerCountCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"controller count (int) (optional)\"";
        
        public ControllerCountCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            using BotDBContext context = DatabaseManager.OpenContext();

            Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);
            int prevJoystickCount = (int)joystickCountSetting.value_int;

            //See the number of controllers
            if (arguments.Count == 0)
            {
                QueueMessage($"There are {joystickCountSetting.value_int} controller(s) plugged in! To set the controller count, please provide a number as an argument.");
                return;
            }

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Check if the user has the ability to set the controller count
            User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

            if (user != null && user.HasAbility(PermissionConstants.SET_VCONTROLLER_COUNT_ABILITY) == false)
            {
                QueueMessage("You don't have the ability to change the number of controllers!");
                return;
            }

            string countStr = arguments[0];

            //Parse
            if (int.TryParse(countStr, out int newJoystickCount) == false)
            {
                QueueMessage("Invalid controller count.");
                return;
            }

            //Same type
            if (newJoystickCount == joystickCountSetting.value_int)
            {
                QueueMessage($"There are already {newJoystickCount} controllers plugged in.");
                return;
            }
            
            int minControllers = DataContainer.ControllerMngr.MinControllers;
            int maxControllers = DataContainer.ControllerMngr.MaxControllers;

            if (newJoystickCount < minControllers)
            {
                QueueMessage($"New controller count of {newJoystickCount} is invalid. The minimum number of controllers is {minControllers}.");
                return;
            }
            else if (newJoystickCount > maxControllers)
            {
                QueueMessage($"New controller count of {newJoystickCount} is invalid. The maximum number of controllers is {maxControllers}.");
                return;
            }

            //Set the value and save
            joystickCountSetting.value_int = newJoystickCount;
            context.SaveChanges();

            //Stop and halt all inputs
            InputHandler.StopAndHaltAllInputs();
            
            try
            {
                //Clean up the controller manager
                DataContainer.ControllerMngr?.CleanUp();

                //Re-initialize and initialize controllers
                DataContainer.ControllerMngr.Initialize();
                int acquiredCount = DataContainer.ControllerMngr.InitControllers(newJoystickCount);

                QueueMessage($"Changed controller count from {prevJoystickCount} to {newJoystickCount}, acquired {acquiredCount} controllers, and reset all running inputs!");
            }
            catch (Exception e)
            {
                DataContainer.MessageHandler.QueueMessage($"Error changing virtual controller count: {e.Message}");
                return;
            }
            finally
            {
                //Resume inputs
                InputHandler.ResumeRunningInputs();
            }
        }
    }
}
