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
    /// Displays or changes the virtual controller used.
    /// </summary>
    public sealed class VirtualControllerCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"virtual controller type (string/int)\"";
        private string CachedVCTypesStr = string.Empty;
        
        public VirtualControllerCommand()
        {
            
        }

        public override void Initialize(CommandHandler cmdHandler, DataContainer dataContainer)
        {
            base.Initialize(cmdHandler, dataContainer);

            string[] names = EnumUtility.GetNames<VirtualControllerTypes>.EnumNames;

            for (int i = 0; i < names.Length; i++)
            {
                //Don't add Invalid to the list
                if (names[i] == VirtualControllerTypes.Invalid.ToString())
                {
                    continue;
                }

                CachedVCTypesStr += names[i];

                if (i < (names.Length - 1))
                {
                    CachedVCTypesStr += ", ";
                }
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            using BotDBContext context = DatabaseManager.OpenContext();

            long lastVControllerType = DataHelper.GetSettingIntNoOpen(SettingsConstants.LAST_VCONTROLLER_TYPE, context, 0L);
            VirtualControllerTypes curVCType = (VirtualControllerTypes)lastVControllerType;

            //See the virtual controller
            if (arguments.Count == 0)
            {
                QueueMessage($"The current virtual controller is {(VirtualControllerTypes)lastVControllerType}. To set the virtual controller, add one as an argument: {CachedVCTypesStr}");
                return;
            }

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Check if the user has the ability to set the type
            User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

            if (user != null && user.HasAbility(PermissionConstants.SET_VCONTROLLER_TYPE_ABILITY) == false)
            {
                QueueMessage("You don't have the ability to set the virtual controller type!");
                return;
            }

            string vControllerStr = arguments[0];

            //Parse
            if (EnumUtility.TryParseEnumValue(vControllerStr, out VirtualControllerTypes parsedVCType) == false)
            {
                QueueMessage($"Please enter a valid virtual controller: {CachedVCTypesStr}");
                return;
            }

            //Same type
            if (parsedVCType == curVCType)
            {
                QueueMessage($"The current virtual controller is already {curVCType}!");
                return;
            }
            
            //Make sure this virtual controller is supported on this platform
            if (VControllerHelper.IsVControllerSupported(parsedVCType, TRBotOSPlatform.CurrentOS) == false)
            {
                QueueMessage($"{parsedVCType} virtual controllers are not supported on your operating system.");
                return;
            }
            
            //Set the value and save
            Settings lastVControllerSetting = DataHelper.GetSettingNoOpen(SettingsConstants.LAST_VCONTROLLER_TYPE, context);
            lastVControllerSetting.value_int = (long)parsedVCType;

            context.SaveChanges();

            //Stop and halt all inputs
            InputHandler.StopAndHaltAllInputs();
            
            try
            {
                //Clean up the controller manager
                DataContainer.ControllerMngr?.CleanUp();

                DataContainer.SetCurVControllerType(parsedVCType);

                //Assign the new controller manager
                IVirtualControllerManager controllerMngr = VControllerHelper.GetVControllerMngrForType(parsedVCType);

                DataContainer.SetControllerManager(controllerMngr);

                DataContainer.ControllerMngr.Initialize();
                
                //Ensure we clamp the controller count to the correct value for this virtual controller manager
                int minControllerCount = DataContainer.ControllerMngr.MinControllers;
                int maxControllerCount = DataContainer.ControllerMngr.MaxControllers;

                Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);
                if (joystickCountSetting.value_int < minControllerCount)
                {
                   QueueMessage($"Controller count of {joystickCountSetting.value_int} is invalid. Clamping to the min of {minControllerCount}.");
                   joystickCountSetting.value_int = minControllerCount;
                   context.SaveChanges();
                }
                else if (joystickCountSetting.value_int > maxControllerCount)
                {
                   QueueMessage($"Controller count of {joystickCountSetting.value_int} is invalid. Clamping to the max of {maxControllerCount}.");
                   joystickCountSetting.value_int = maxControllerCount;
                   context.SaveChanges();
                }

                int acquiredCount = DataContainer.ControllerMngr.InitControllers((int)joystickCountSetting.value_int);

                QueueMessage($"Set virtual controller to {parsedVCType} with {acquiredCount} controllers and reset all running inputs!");
            }
            catch (Exception e)
            {
                DataContainer.MessageHandler.QueueMessage($"Error changing virtual controller type: {e.Message}");
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
