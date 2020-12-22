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
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Shows the length of an input sequence.
    /// </summary>
    public sealed class InputLengthCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"input sequence\"";

        public InputLengthCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string input = args.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(input) == true)
            {
                QueueMessage(UsageMessage);
                return;
            }

            GameConsole usedConsole = null;

            int lastConsoleID = 1;

            lastConsoleID = (int)DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);

                if (lastConsole != null)
                {
                    //Create a new console using data from the database
                    usedConsole = new GameConsole(lastConsole.Name, lastConsole.InputList, lastConsole.InvalidCombos);
                }
            }

            //If there are no valid inputs, don't attempt to parse
            if (usedConsole == null)
            {
                QueueMessage($"The current console does not point to valid data. Please set a different console to use, or if none are available, add one.");
                return;
            }

            if (usedConsole.ConsoleInputs.Count == 0)
            {
                QueueMessage($"The current console, \"{usedConsole.Name}\", does not have any available inputs. Cannot determine length.");
                return;
            }

            ParsedInputSequence inputSequence = default;

            try
            {
                string userName = args.Command.ChatMessage.Username;

                //Get default and max input durations
                //Use user overrides if they exist, otherwise use the global values
                int defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(userName);
                int maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(userName);
                
                string regexStr = usedConsole.InputRegex;

                string readyMessage = string.Empty;

                Parser parser = new Parser();

                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Get input synonyms for this console
                    IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == lastConsoleID);

                    //Prepare the message for parsing
                    readyMessage = parser.PrepParse(input, context.Macros, synonyms);
                }

                //Parse inputs to get our parsed input sequence
                inputSequence = parser.ParseInputs(readyMessage, regexStr, new ParserOptions(0, defaultDur, true, maxDur));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Handle parsing exceptions
                inputSequence.ParsedInputResult = ParsedInputResults.Invalid;

                QueueMessage($"Invalid input: {excMsg}");
                return;
            }

            //Check for non-valid messages
            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid)
            {
                const string dyMacroLenErrorMsg = "Note that length cannot be determined for dynamic macros without inputs filled in.";

                if (inputSequence.ParsedInputResult == ParsedInputResults.NormalMsg
                    || string.IsNullOrEmpty(inputSequence.Error) == true)
                {
                    QueueMessage($"Invalid input. {dyMacroLenErrorMsg}");
                }
                else
                {
                    QueueMessage($"Invalid input: {inputSequence.Error}. {dyMacroLenErrorMsg}");
                }
                
                return;
            }

            QueueMessage($"Total length: {inputSequence.TotalDuration}ms");
        }
    }
}
