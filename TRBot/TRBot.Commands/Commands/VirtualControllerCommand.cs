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

        public override void Initialize()
        {
            base.Initialize();

            //Show only the virtual controllers supported on this platform
            VirtualControllerTypes[] vcTypes = EnumUtility.GetValues<VirtualControllerTypes>.EnumValues;

            for (int i = 0; i < vcTypes.Length; i++)
            {
                VirtualControllerTypes vControllerType = vcTypes[i];

                //Continue if not supported
                if (VControllerHelper.IsVControllerSupported(vControllerType,
                    TRBotOSPlatform.CurrentOS) == false)
                {
                    continue;
                }

                CachedVCTypesStr += vControllerType.ToString();

                if (i < (vcTypes.Length - 1))
                {
                    CachedVCTypesStr += ", ";
                }
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long lastVControllerType = DataHelper.GetSettingInt(SettingsConstants.LAST_VCONTROLLER_TYPE, 0L);
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

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user has the ability to set the type
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_VCONTROLLER_TYPE_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to set the virtual controller type!");
                    return;
                }
            }

            string vControllerStr = arguments[0];

            //Parse
            if (EnumUtility.TryParseEnumValue(vControllerStr, out VirtualControllerTypes parsedVCType) == false)
            {
                QueueMessage($"Please enter a valid virtual controller: {CachedVCTypesStr}");
                return;
            }

            //Same type
            if (parsedVCType == curVCType && DataContainer.ControllerMngr.Initialized == true)
            {
                QueueMessage($"The current virtual controller is already {curVCType}!");
                return;
            }
            
            //Make sure this virtual controller is supported on this platform
            if (VControllerHelper.IsVControllerSupported(parsedVCType, TRBotOSPlatform.CurrentOS) == false)
            {
                QueueMessage($"{parsedVCType} virtual controllers are not supported on {TRBotOSPlatform.CurrentOS} platforms.", Serilog.Events.LogEventLevel.Warning);
                return;
            }
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Set the value and save
                Settings lastVControllerSetting = DataHelper.GetSettingNoOpen(SettingsConstants.LAST_VCONTROLLER_TYPE, context);
                lastVControllerSetting.ValueInt = (long)parsedVCType;

                context.SaveChanges();
            }

            //Stop and halt all inputs
            InputHandler.StopAndHaltAllInputs();
            
            try
            {
                //Assign the new controller manager
                IVirtualControllerManager controllerMngr = VControllerHelper.GetVControllerMngrForType(parsedVCType);

                if (controllerMngr == null)
                {
                    QueueMessage($"Virtual controller manager of new type {parsedVCType} failed to initialize. This indicates an invalid {SettingsConstants.LAST_VCONTROLLER_TYPE} setting in the database or an unimplemented platform.", Serilog.Events.LogEventLevel.Error);
                    return;
                }

                //Dispose the controller manager
                DataContainer.ControllerMngr.Dispose();

                DataContainer.SetCurVControllerType(parsedVCType);

                DataContainer.SetControllerManager(controllerMngr);

                DataContainer.ControllerMngr.Initialize();
                
                //Ensure we clamp the controller count to the correct value for this virtual controller manager
                int minControllerCount = DataContainer.ControllerMngr.MinControllers;
                int maxControllerCount = DataContainer.ControllerMngr.MaxControllers;

                int joystickCount = (int)DataHelper.GetSettingInt(SettingsConstants.JOYSTICK_COUNT, 0L);
                int newJoystickCount = joystickCount;

                if (joystickCount < minControllerCount)
                {
                   QueueMessage($"Controller count of {joystickCount} is invalid. Clamping to the min of {minControllerCount}.");
                   newJoystickCount = minControllerCount;
                }
                else if (joystickCount > maxControllerCount)
                {
                   QueueMessage($"Controller count of {joystickCount} is invalid. Clamping to the max of {maxControllerCount}.");
                   newJoystickCount = maxControllerCount;
                }

                if (joystickCount != newJoystickCount)
                {
                    using (BotDBContext context = DatabaseManager.OpenContext())
                    {
                        //Adjust the joystick count setting
                        Settings joystickCountSetting = DataHelper.GetSettingNoOpen(SettingsConstants.JOYSTICK_COUNT, context);
                        joystickCountSetting.ValueInt = newJoystickCount;
                        
                        context.SaveChanges();
                    }
                }

                int acquiredCount = DataContainer.ControllerMngr.InitControllers(newJoystickCount);

                QueueMessage($"Set virtual controller to {parsedVCType} with {acquiredCount} controller(s) and reset all running inputs!");
            }
            catch (Exception e)
            {
                DataContainer.MessageHandler.QueueMessage($"Error changing virtual controller type: {e.Message}", Serilog.Events.LogEventLevel.Error);
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
