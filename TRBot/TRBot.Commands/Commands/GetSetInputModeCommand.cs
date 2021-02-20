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
using System.Text;
using TRBot.Connection;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Misc;
using TRBot.Permissions;
using TRBot.Routines;

namespace TRBot.Commands
{
    /// <summary>
    /// Displays or changes the current input mode.
    /// </summary>
    public sealed class GetSetInputModeCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"input mode (string/int)\"";
        private string CachedInputModesStr = string.Empty;
        
        public GetSetInputModeCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            //Show all input modes
            InputModes[] inputModeArr = EnumUtility.GetValues<InputModes>.EnumValues;

            for (int i = 0; i < inputModeArr.Length; i++)
            {
                InputModes inputMode = inputModeArr[i];

                CachedInputModesStr += inputMode.ToString();

                if (i < (inputModeArr.Length - 1))
                {
                    CachedInputModesStr += ", ";
                }
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long curInputMode = DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE, 0L);
            InputModes inpMode = (InputModes)curInputMode;

            //See the input mode
            if (arguments.Count == 0)
            {
                QueueMessage($"The current input mode is {inpMode}. To set the input mode, add one as an argument: {CachedInputModesStr}");
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
                //Check if the user has the ability to set the mode
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_INPUT_MODE_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to set the input mode!");
                    return;
                }
            }

            string inputModeStr = arguments[0];

            //Parse
            if (EnumUtility.TryParseEnumValue(inputModeStr, out InputModes parsedInputMode) == false)
            {
                QueueMessage($"Please enter a valid input mode: {CachedInputModesStr}");
                return;
            }

            //Same mode
            if (parsedInputMode == inpMode)
            {
                QueueMessage($"The current input mode is already {inpMode}!");
                return;
            }
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Set the value and save
                Settings inputModeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.INPUT_MODE, context);
                inputModeSetting.ValueInt = (long)parsedInputMode;

                context.SaveChanges();
            }
            
            QueueMessage($"Changed the input mode from {inpMode} to {parsedInputMode}!");

            //If we set it to Anarchy, check if the Democracy routine is active and remove it if so
            if (parsedInputMode == InputModes.Anarchy)
            {
                BaseRoutine democracyRoutine = RoutineHandler.FindRoutine(RoutineConstants.DEMOCRACY_ROUTINE_ID, out int indexFound);
                if (democracyRoutine != null)
                {
                    RoutineHandler.RemoveRoutine(indexFound);
                }
            }
            //If we set it to Democracy, add the routine if it's not already active
            else if (parsedInputMode == InputModes.Democracy)
            {
                DemocracyRoutine democracyRoutine = RoutineHandler.FindRoutine<DemocracyRoutine>();

                if (democracyRoutine == null)
                {
                    long votingTime = DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_VOTE_TIME, 10000L);

                    democracyRoutine = new DemocracyRoutine(votingTime);
                    RoutineHandler.AddRoutine(democracyRoutine);
                }
            }
        }
    }
}
