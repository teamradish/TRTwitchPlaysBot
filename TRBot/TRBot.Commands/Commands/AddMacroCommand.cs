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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            string userName = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.ADD_INPUT_MACRO_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to add input macros.");
                    return;
                }
            }

            string macroName = arguments[0].ToLowerInvariant();

            //Make sure the first argument has a minimum number of characters
            if (macroName.Length < MIN_MACRO_NAME_LENGTH)
            {
                QueueMessage($"Input macros need to be at least {MIN_MACRO_NAME_LENGTH} characters long.");
                return;
            }

            if (macroName.StartsWith(InputMacroPreparser.DEFAULT_MACRO_START) == false)
            {
                QueueMessage($"Input macros must start with \"{InputMacroPreparser.DEFAULT_MACRO_START}\".");
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

            //Trim the macro name from the input sequence
            string macroVal = args.Command.ArgumentsAsString.Remove(0, macroName.Length + 1).ToLowerInvariant();
            //Console.WriteLine(macroVal);

            bool macroExists = false;

            //Find an existing macro with this name
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                InputMacro inputMacro = context.Macros.FirstOrDefault(m => m.MacroName == macroName);
                macroExists = (inputMacro != null);
            }

            //Validate the macro name
            Match macroMatch = Regex.Match(macroName, InputMacroPreparser.MACRO_REGEX, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (macroMatch.Success == false)
            {
                QueueMessage($"Invalid input macro name! Input macros must start with \"{InputMacroPreparser.DEFAULT_MACRO_START}\".");
                return;
            }

            bool isDynamic = macroMatch.Groups.TryGetValue(InputMacroPreparser.MACRO_DYNAMIC_GROUP_NAME,
                out Group dynamicGroup) == true && dynamicGroup?.Success == true;

            //Validate the macro name
            string tempMacroName = macroName;

            //If it's dynamic, ensure it has arguments
            if (isDynamic == true)
            {
                if (macroMatch.Groups.TryGetValue(InputMacroPreparser.MACRO_DYNAMIC_ARGS_GROUP_NAME,
                    out Group dynamicArgsGroup) == false || dynamicArgsGroup.Success == false)
                {
                    QueueMessage("Dynamic input macros must have arguments! Specify them in generic form, such as \"#mash(*)\".");
                    return;
                }
            }

            InputMacro tempMacro = new InputMacro(tempMacroName, macroVal);

            //Run the macro through the preparser
            IQueryable<InputMacro> tempMacroData = new List<InputMacro>(1) { tempMacro }.AsQueryable();

            //Run it through the preparser to ensure it returns the same value as the macro value specified
            //Do this by specifying a max recursion of 1 and not filling dynamic macro arguments
            //This is a simple and effective way to validate the macro 
            InputMacroPreparser macroPreparser = new InputMacroPreparser(tempMacroData, 1, DynamicMacroArgOptions.DontFillArgs);

            string preparsed = macroPreparser.Preparse(tempMacroName);

            //If the values aren't the same, the macro name isn't valid
            if (preparsed != macroVal)
            {
                //TRBotLogger.Logger.Information($"Value: {macroVal} | Parsed: {preparsed}");

                string failedMsg = $"Macro \"{macroName}\" is invalid since the parser didn't retrieve the same macro value specified.";

                if (isDynamic == true)
                {
                    failedMsg += " Ensure dynamic macros are in generic form (Ex. \"#mash(a,b)\" -> \"#mash(*,*)\"). Input macro values should specify the argument they correspond to. (Ex. In \"#mash(a,b)\", \"<0>\" is replaced with \"a\" and \"<1>\" is replaced with \"b\").";
                }

                QueueMessage(failedMsg);
                return;
            }

            //Everything is good to go regarding the macro name at this point
            //Validate the inputs if not a dynamic macro
            if (isDynamic == false)
            {
                ParsedInputSequence inputSequence = default;

                try
                {
                    //Get default and max input durations
                    //Use user overrides if they exist, otherwise use the global values
                    int defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(userName);
                    int maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(userName);

                    using (BotDBContext context = DatabaseManager.OpenContext())
                    {
                        IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == curConsoleID);
    
                        //Copy the existing macro list and add the new macro for parsing
                        List<InputMacro> newMacroList = new List<InputMacro>(context.Macros);
                        newMacroList.Add(new InputMacro(macroName, macroVal));
                        
                        IQueryable<InputMacro> newMacroData = newMacroList.AsQueryable();

                        StandardParser standardParser = StandardParser.CreateStandard(newMacroData, synonyms,
                            consoleInstance.GetInputNames(), 0, DataContainer.ControllerMngr.ControllerCount - 1,
                            defaultDur, maxDur, true);
                        
                        inputSequence = standardParser.ParseInputs(macroVal);
                    }

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

            //Everything is verified; add the macro to the database or update it
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
