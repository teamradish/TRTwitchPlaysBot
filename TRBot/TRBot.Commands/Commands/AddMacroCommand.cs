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
using TRBot.Logging;

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
                QueueMessage($"Input macros need to be at least {MIN_MACRO_NAME_LENGTH} characters long.");
                return;
            }

            if (macroName.StartsWith(Parser.DEFAULT_PARSER_REGEX_MACRO_INPUT) == false)
            {
                QueueMessage($"Input macros must start with \"{Parser.DEFAULT_PARSER_REGEX_MACRO_INPUT}\".");
                return;
            }

            //For simplicity with wait inputs, force the first character in the macro name to be alphanumeric
            if (char.IsLetterOrDigit(arguments[0][1]) == false)
            {
                QueueMessage("The first character in input macro names must be alphanumeric.");
                return;
            }

            //Check for max macro name
            if (macroName.Length > MAX_MACRO_NAME_LENGTH)
            {
                QueueMessage($"Input macros may have up to a max of {MAX_MACRO_NAME_LENGTH} characters in their name.");
                return;
            }

            int curConsoleID = (int)DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);

            GameConsole consoleInstance = null;
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole curConsole = context.Consoles.FirstOrDefault(c => c.ID == curConsoleID);
                if (curConsole == null)
                {
                    QueueMessage("Cannot validate input macro, as the current console is invalid. Fix this by setting another console.");
                    return;
                }

                consoleInstance = new GameConsole(curConsole.Name, curConsole.InputList);
            }

            Parser parser = new Parser();

            //Trim the macro name from the input sequence
            string macroVal = args.Command.ArgumentsAsString.Remove(0, macroName.Length + 1).ToLowerInvariant();
            //Console.WriteLine(macroVal);

            bool isDynamic = false;

            //Check for a dynamic macro
            int openParenIndex = macroName.IndexOf('(', 0);
            if (openParenIndex >= 0)
            {
                //If we found the open parenthesis, check for the asterisk
                //This is not comprehensive, but it should smooth out a few issues
                if (openParenIndex == (macroName.Length - 1) || macroName[openParenIndex + 1] != '*')
                {
                    QueueMessage("Invalid input macro. Dynamic macro arguments must be specified with \"*\".");
                    return;
                }

                if (macroName[macroName.Length - 1] != ')')
                {
                    QueueMessage("Invalid input macro. Dynamic macros must end with \")\".");
                    return;
                }

                isDynamic = true;
            }

            //Validate input if not dynamic
            if (isDynamic == false)
            {
                ParsedInputSequence inputSequence = default;

                try
                {
                    string userName = args.Command.ChatMessage.Username;

                    //Get default and max input durations
                    //Use user overrides if they exist, otherwise use the global values
                    int defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(userName);
                    int maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(userName);

                    string regexStr = consoleInstance.InputRegex;

                    string readyMessage = string.Empty;

                    using (BotDBContext context = DatabaseManager.OpenContext())
                    {
                        IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == curConsoleID);
    
                        readyMessage = parser.PrepParse(macroVal, context.Macros, synonyms);
                    }

                    inputSequence = parser.ParseInputs(readyMessage, regexStr, new ParserOptions(0, defaultDur, true, maxDur));
                    TRBotLogger.Logger.Debug(inputSequence.ToString());

                    if (inputSequence.ParsedInputResult != ParsedInputResults.Valid)
                    {
                        if (string.IsNullOrEmpty(inputSequence.Error) == true)
                        {
                            QueueMessage("Invalid input macro.");
                        }
                        else
                        {
                            QueueMessage($"Invalid input macro: {inputSequence.Error}");
                        }

                        return;
                    }   
                }
                catch (Exception e)
                {
                    QueueMessage($"Invalid input macro: {e.Message}");
                    return;
                }
            }

            string message = string.Empty;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                InputMacro inputMacro = context.Macros.FirstOrDefault(m => m.MacroName == macroName);

                //Not an existing macro, so add it
                if (inputMacro == null)
                {
                    InputMacro newMacro = new InputMacro(macroName, macroVal);

                    context.Macros.Add(newMacro);

                    if (isDynamic == false)
                        message = $"Added input macro \"{macroName}\"!";
                    else message = $"Added dynamic input macro \"{macroName}\"! Dynamic input macros can't be validated beforehand, so verify it works manually.";
                }
                //Update the macro value
                else
                {
                    inputMacro.MacroValue = macroVal;

                    if (isDynamic == false)
                        message = $"Updated input macro \"{macroName}\"!";
                    else message = $"Updated dynamic input macro \"{macroName}\"! Dynamic input macros can't be validated beforehand, so verify it works manually.";
                }

                context.SaveChanges();
            }
            
            QueueMessage(message);
        }
    }
}
