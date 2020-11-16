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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Misc;
using TRBot.Data;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine that periodically makes an input.
    /// This is useful for newer console games that go to sleep if no inputs are pressed after a while.
    /// </summary>
    public class PeriodicInputRoutine : BaseRoutine
    {
        private DateTime CurInputTime;

        public PeriodicInputRoutine()
        {
            Identifier = RoutineConstants.PERIODIC_INPUT_ROUTINE_ID;
        }

        public override void Initialize()
        {
            base.Initialize();

            CurInputTime = DateTime.UtcNow;
        }

        public override void CleanUp()
        {
            base.CleanUp();
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            TimeSpan timeDiff = currentTimeUTC - CurInputTime;

            using BotDBContext context = DatabaseManager.OpenContext();

            long inputTimeMS = DataHelper.GetSettingIntNoOpen(SettingsConstants.PERIODIC_INPUT_TIME, context, -1L);

            //Don't do anything if the input time is less than 0 to prevent spam
            if (inputTimeMS < 0L)
            {
                CurInputTime = currentTimeUTC;
                return;
            }

            //Check if we surpassed the time
            if (timeDiff.TotalMilliseconds < inputTimeMS)
            {
                return;
            }
            
            //Refresh time
            CurInputTime = currentTimeUTC;

            string periodicInputValue = DataHelper.GetSettingStringNoOpen(SettingsConstants.PERIODIC_INPUT_VALUE, context, string.Empty);
            int controllerPort = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.PERIODIC_INPUT_PORT, context, 0L);

            //Don't perform the input if it's empty
            if (string.IsNullOrEmpty(periodicInputValue) == true)
            {
                DataContainer.MessageHandler.QueueMessage($"Failed periodic input: {SettingsConstants.PERIODIC_INPUT_VALUE} is null or empty"); 
                return;
            }

            //Don't perform the input if the controller port is out of range
            if (controllerPort < 0 || controllerPort >= DataContainer.ControllerMngr?.ControllerCount)
            {
                DataContainer.MessageHandler.QueueMessage($"Failed periodic input: The controller port is {controllerPort}, which is out of range for this virtual controller");
                return;
            }

            //Set up the console
            GameConsole usedConsole = null;

            int lastConsoleID = 1;

            lastConsoleID = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.LAST_CONSOLE, context, 1L);
            GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);

            if (lastConsole != null)
            {
                //Create a new console using data from the database
                usedConsole = new GameConsole(lastConsole.Name, lastConsole.InputList, lastConsole.InvalidCombos);
            }

            //If there are no valid inputs, don't attempt to parse
            if (usedConsole == null)
            {
                DataContainer.MessageHandler.QueueMessage("Failed periodic input: The current console does not point to valid data. Please set a different console to use, or if none are available, add one.");
                return;
            }

            if (usedConsole.ConsoleInputs.Count == 0)
            {
                DataContainer.MessageHandler.QueueMessage($"Failed periodic input: The current console, \"{usedConsole.Name}\", does not have any available inputs. Cannot determine length.");
                return;
            }

            ParsedInputSequence inputSequence = default;

            try
            {
                string regexStr = usedConsole.InputRegex;

                string readyMessage = string.Empty;

                int defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(null, context);
                int maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(null, context);
                
                //Get input synonyms for this console
                IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == lastConsoleID);
                
                Parser parser = new Parser();

                //Prepare the message for parsing
                readyMessage = parser.PrepParse(periodicInputValue, context.Macros, synonyms);

                //Parse inputs to get our parsed input sequence
                inputSequence = parser.ParseInputs(readyMessage, regexStr, new ParserOptions(0, defaultDur, true, maxDur));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Handle parsing exceptions
                inputSequence.ParsedInputResult = ParsedInputResults.Invalid;

                DataContainer.MessageHandler.QueueMessage($"Failed periodic input: {excMsg}");
                return;
            }

            //Check for non-valid messages
            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid)
            {
                string message = inputSequence.Error;
                if (string.IsNullOrEmpty(message) == true)
                {
                    message = "Input is invalid";
                }
                
                DataContainer.MessageHandler.QueueMessage($"Failed periodic input: {message}");
                return;
            }

            //Now, perform the input
            if (InputHandler.InputsHalted == true)
            {
                DataContainer.MessageHandler.QueueMessage("Inputs are halted! Unable to perform periodic input.");
            }
            else
            {
                InputHandler.CarryOutInput(inputSequence.Inputs, usedConsole, DataContainer.ControllerMngr);
                
                //Print a message with the input to show it's processed
                DataContainer.MessageHandler.QueueMessage($"Periodic input: {periodicInputValue}");
            }
        }
    }
}
