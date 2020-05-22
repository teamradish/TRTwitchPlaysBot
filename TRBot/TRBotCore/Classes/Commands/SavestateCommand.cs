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
using TwitchLib.Client.Events;
using System.Diagnostics;

namespace TRBot
{
    /// <summary>
    /// Saves a savestate.
    /// </summary>
    public sealed class SavestateCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 1)
            {
                BotProgram.QueueMessage("Usage: state #");
                return;
            }

            string stateNumStr = args[0];

            if (int.TryParse(stateNumStr, out int stateNum) == false)
            {
                BotProgram.QueueMessage("Invalid state number.");
                return;
            }

            string saveStateStr = $"ss{stateNum}";
            if (InputGlobals.CurrentConsole.ButtonInputMap.ContainsKey(saveStateStr) == false)
            {
                BotProgram.QueueMessage("Invalid state number.");
                return;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            //Check if the user can perform this input
            ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissions(user.Level, saveStateStr, BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            if (inputValidation.IsValid == false)
            {
                if (string.IsNullOrEmpty(inputValidation.Message) == false)
                {
                    BotProgram.QueueMessage(inputValidation.Message);
                }

                return;
            }

            //Savestates are always performed on the first controller
            IVirtualController joystick = InputGlobals.ControllerMngr.GetController(0);
            joystick.PressButton(InputGlobals.CurrentConsole.ButtonInputMap[saveStateStr]);
            joystick.UpdateController();

            //Track the time of the savestate
            DateTime curTime = DateTime.UtcNow;

            //Add a new savestate log
            GameLog newStateLog = new GameLog();
            newStateLog.User = e.Command.ChatMessage.Username;

            string date = curTime.ToShortDateString();
            string time = curTime.ToLongTimeString();
            newStateLog.DateTimeString = $"{date} at {time}";

            newStateLog.User = e.Command.ChatMessage.Username;
            newStateLog.LogMessage = string.Empty;

            //Add the message if one was specified
            if (args.Count > 1)
            {
                string message = e.Command.ArgumentsAsString.Remove(0, stateNumStr.Length + 1);
                newStateLog.LogMessage = message;
            }

            //Add or replace the log and save the bot data
            BotProgram.BotData.SavestateLogs[stateNum] = newStateLog;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Saved state {stateNum}!");

            //Wait a bit before releasing the input
            const float wait = 50f;
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < wait)
            {

            }

            joystick.ReleaseButton(InputGlobals.CurrentConsole.ButtonInputMap[saveStateStr]);
            joystick.UpdateController();
        }
    }
}
