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

namespace TRBot
{
    public sealed class ConsoleCommand : BaseCommand
    {
        private StringBuilder StrBuilder = null;

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            //See the console
            if (args.Count == 0)
            {
                BotProgram.QueueMessage($"The current console is {InputGlobals.CurrentConsoleVal}. To set the console, add one as an argument: {GetValidConsoleStr()}");
                return;
            }

            string consoleStr = args[0];

            if (Enum.TryParse<InputGlobals.InputConsoles>(consoleStr, true, out InputGlobals.InputConsoles console) == false)
            {
                BotProgram.QueueMessage($"Please enter a valid console: {GetValidConsoleStr()}");
                return;
            }

            if (console == InputGlobals.CurrentConsoleVal)
            {
                BotProgram.QueueMessage($"The current console is already {InputGlobals.CurrentConsoleVal}!");
                return;
            }

            //First stop all inputs completely while changing consoles - we don't want data from other inputs remaining
            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {

            }

            //Set console and buttons
            InputGlobals.SetConsole(console, args);
            
            for (int i = 0; i < InputGlobals.ControllerMngr.ControllerCount; i++)
            {
                IVirtualController controller = InputGlobals.ControllerMngr.GetController(i);
                if (controller.IsAcquired == true)
                {
                    controller.Reset();
                }
            }

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            BotProgram.QueueMessage($"Set console to {InputGlobals.CurrentConsoleVal} and reset all running inputs!");
        }

        private string GetValidConsoleStr()
        {
            string[] names = EnumUtility.GetNames<InputGlobals.InputConsoles>.EnumNames;

            if (StrBuilder == null)
            {
                StrBuilder = new StringBuilder(500);
            }

            StrBuilder.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                StrBuilder.Append(names[i]).Append(',').Append(' ');
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            return StrBuilder.ToString();
        }
    }
}
