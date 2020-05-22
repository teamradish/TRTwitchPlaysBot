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
using System.Diagnostics;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class LoadstateCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage("Usage: state #");
                return;
            }

            string stateNumStr = args[0];

            if (int.TryParse(stateNumStr, out int stateNum) == false)
            {
                BotProgram.QueueMessage($"Invalid state number.");
                return;
            }

            string loadStateStr = $"ls{stateNum}";
            if (InputGlobals.CurrentConsole.ButtonInputMap.ContainsKey(loadStateStr) == false)
            {
                BotProgram.QueueMessage($"Invalid state number.");
                return;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            //Check if the user can perform this input
            ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissions(user.Level, loadStateStr, BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            if (inputValidation.IsValid == false)
            {
                if (string.IsNullOrEmpty(inputValidation.Message) == false)
                {
                    BotProgram.QueueMessage(inputValidation.Message);
                }

                return;
            }

            //Load states are always performed on the first controller
            IVirtualController joystick = InputGlobals.ControllerMngr.GetController(0);
            joystick.PressButton(InputGlobals.CurrentConsole.ButtonInputMap[loadStateStr]);
            joystick.UpdateController();

            BotProgram.QueueMessage($"Loaded state {stateNum}!");

            //Wait a bit before releasing the input
            const float wait = 50f;
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < wait)
            {

            }

            joystick.ReleaseButton(InputGlobals.CurrentConsole.ButtonInputMap[loadStateStr]);
            joystick.UpdateController();
        }
    }
}
