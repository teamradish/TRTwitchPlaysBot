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
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.ParserData;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Adds or updates an input macro.
    /// </summary>
    public sealed class AddMacroCommand : BaseCommand
    {
        /// <summary>
        /// The max name length for macros.
        /// </summary>
        public const int MAX_MACRO_NAME_LENGTH = 50;

        /// <summary>
        /// The min name length for macros.
        /// </summary>
        public const int MIN_MACRO_NAME_LENGTH = 2;

        private string UsageMessage = "Usage: \"#macroname\" \"input sequence\"";

        public AddMacroCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count < 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string macroName = arguments[0].ToLowerInvariant();

            //Make sure the first argument has at least a minimum number of characters
            if (macroName.Length < MIN_MACRO_NAME_LENGTH)
            {
                QueueMessage($"Macros need to be at least {MIN_MACRO_NAME_LENGTH} characters long.");
                return;
            }

            if (macroName.StartsWith(Parser.DEFAULT_PARSER_REGEX_MACRO_INPUT) == false)
            {
                QueueMessage($"Macros must start with \"{Parser.DEFAULT_PARSER_REGEX_MACRO_INPUT}\".");
                return;
            }

            //For simplicity with wait inputs, force the first character in the macro name to be alphanumeric
            if (char.IsLetterOrDigit(arguments[0][1]) == false)
            {
                QueueMessage("The first character in macro names must be alphanumeric.");
                return;
            }

            //Check for max macro name
            if (macroName.Length > MAX_MACRO_NAME_LENGTH)
            {
                QueueMessage($"Macros may have up to a max of {MAX_MACRO_NAME_LENGTH} characters in their name.");
                return;
            }

            using BotDBContext context = DatabaseManager.OpenContext();

            int defaultInputDur = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.DEFAULT_INPUT_DURATION, context, 200L);
            int maxInputDur = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.MAX_INPUT_DURATION, context, 60000L);
            int lastConsole = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.LAST_CONSOLE, context, 1L);

            GameConsole curConsole = context.Consoles.FirstOrDefault(c => c.id == lastConsole);
            if (curConsole == null)
            {
                QueueMessage("Cannot validate macro, as the current console is invalid. Fix this by setting another console.");
                return;
            }

            GameConsole consoleInstance = new GameConsole(curConsole.Name, curConsole.InputList);

            Parser parser = new Parser();

            //Trim the macro name from the input sequence
            string macroVal = args.Command.ArgumentsAsString.Remove(0, macroName.Length + 1).ToLowerInvariant();
            Console.WriteLine(macroVal);

            bool isDynamic = false;

            //Check for a dynamic macro
            if (macroName.Contains("(*") == true)
            {
                isDynamic = true;
            }

            //Validate input if not dynamic
            if (isDynamic == false)
            {
                ParsedInputSequence inputSequence = default;

                try
                {
                    string regexStr = consoleInstance.InputRegex;

                    IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.console_id == curConsole.id);

                    string readyMessage = readyMessage = parser.PrepParse(macroVal, context.Macros, synonyms);

                    inputSequence = parser.ParseInputs(readyMessage, regexStr, new ParserOptions(0, defaultInputDur, true, maxInputDur));
                    //Console.WriteLine(inputSequence.ToString());

                    if (inputSequence.InputValidationType != InputValidationTypes.Valid)
                    {
                        QueueMessage("Invalid macro.");
                        return;
                    }   
                }
                catch
                {
                    QueueMessage("Invalid macro.");
                    return;
                }
            }

            string message = string.Empty;

            InputMacro inputMacro = context.Macros.FirstOrDefault(m => m.MacroName == macroName);

            //Not an existing macro, so add it
            if (inputMacro == null)
            {
                InputMacro newMacro = new InputMacro(macroName, macroVal);

                context.Macros.Add(newMacro);

                if (isDynamic == false)
                    message = $"Added macro \"{macroName}\"!";
                else message = $"Added dynamic macro \"{macroName}\"! Dynamic macros can't be validated beforehand, so verify it works manually.";
            }
            //Update the macro value
            else
            {
                inputMacro.MacroValue = macroVal;

                if (isDynamic == false)
                    message = $"Updated macro \"{macroName}\"!";
                else message = $"Updated dynamic macro \"{macroName}\"! Dynamic macros can't be validated beforehand, so verify it works manually.";
            }

            context.SaveChanges();
            
            QueueMessage(message);
        }
    }
}
