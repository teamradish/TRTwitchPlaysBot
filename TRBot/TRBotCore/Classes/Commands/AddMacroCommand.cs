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
using System.Text.RegularExpressions;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Adds a macro.
    /// </summary>
    public sealed class AddMacroCommand : BaseCommand
    {
        public const int MAX_MACRO_LENGTH = 50;

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 2)
            {
                BotProgram.MsgHandler.QueueMessage($"{Globals.CommandIdentifier}addmacro usage: \"#macroname\" \"command\"");
                return;
            }

            //Make sure the first argument has at least two characters
            if (args[0].Length < 2)
            {
                BotProgram.MsgHandler.QueueMessage("Macros need to be at least two characters!");
                return;
            }

            string macroName = args[0].ToLowerInvariant();

            if (macroName[0] != Globals.MacroIdentifier)
            {
                BotProgram.MsgHandler.QueueMessage($"Macros must start with '{Globals.MacroIdentifier}'.");
                return;
            }

            //For simplicity with wait inputs, force the first character in the macro name to be alphanumeric
            if (char.IsLetterOrDigit(args[0][1]) == false)
            {
                BotProgram.MsgHandler.QueueMessage("The first character in macro names must be alphanumeric!");
                return;
            }

            if (macroName.Length > MAX_MACRO_LENGTH)
            {
                BotProgram.MsgHandler.QueueMessage($"The max macro length is {MAX_MACRO_LENGTH} characters!");
                return;
            }

            string macroVal = e.Command.ArgumentsAsString.Remove(0, macroName.Length + 1).ToLowerInvariant();

            bool isDynamic = false;
            string parsedVal = macroVal;

            //Check for a dynamic macro
            if (macroName.Contains("(*") == true)
            {
                isDynamic = true;

                //Dynamic macros can't be fully verified until we do some brute forcing
                //An example is: "a500ms [b .]*<0>"
                //The <0> should be a number, so if we use a valid input it won't work
                //The brute force approach would check with all possible combinations of the first input (Ex. "a") and a number
                //If any are valid, it's a valid dynamic macro

                //NOTE: We need to verify that the dynamic macro takes the form of "#macroname(*,*)"
                //It needs to have an open parenthesis followed by a number of asterisks separated by commas, then ending with a closed parenthesis
                //string parseMsg = string.Empty;
                //try
                //{
                //    parseMsg = Parser.Expandify(Parser.PopulateMacros(parsedVal));
                //}
                //catch (Exception exception)
                //{
                //    BotProgram.MsgHandler.QueueMessage("Invalid dynamic macro. Ensure that variables are listed in order (Ex. (*,*,...) = <0>, <1>,...)");
                //    Console.WriteLine(exception.Message);
                //    return;
                //}
                //
                //MatchCollection matches = Regex.Matches(parseMsg, @"<[0-9]+>", RegexOptions.Compiled);
                //
                ////Kimimaru: Replace all variables with a valid input to verify its validity
                ////Any input will do, so just grab the first one
                //string input = InputGlobals.ValidInputs[0];
                //
                //for (int i = 0; i < matches.Count; i++)
                //{
                //    Match match = matches[i];
                //
                //    parsedVal = parsedVal.Replace(match.Value, input);
                //}
            }

            //Validate input if not dynamic
            if (isDynamic == false)
            {
                try
                {
                    string parse_message = Parser.Expandify(Parser.PopulateMacros(parsedVal, BotProgram.BotData.Macros, BotProgram.BotData.ParserMacroLookup));
                    parse_message = Parser.PopulateSynonyms(parse_message, InputGlobals.InputSynonyms);
                    Parser.InputSequence inputSequence = Parser.ParseInputs(parse_message, InputGlobals.ValidInputRegexStr, new Parser.ParserOptions(0, BotProgram.BotData.DefaultInputDuration, true, BotProgram.BotData.MaxInputDuration));
                    //var val = Parser.Parse(parse_message);

                    if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)//val.Item1 == false)
                    {
                        BotProgram.MsgHandler.QueueMessage("Invalid macro.");
                        return;
                    }
                }
                catch
                {
                    BotProgram.MsgHandler.QueueMessage("Invalid macro.");
                    return;
                }
            }

            //Parser.InputSequence inputSequence = default;
            //
            //try
            //{
            //    //Parse the macro to check for valid input
            //    string parse_message = Parser.Expandify(Parser.PopulateMacros(parsedVal));
            //
            //    inputSequence = Parser.ParseInputs(parse_message);
            //}
            //catch
            //{
            //    if (isDynamic == false)
            //    {
            //        BotProgram.MsgHandler.QueueMessage("Invalid macro.");
            //    }
            //    else
            //    {
            //        BotProgram.MsgHandler.QueueMessage("Invalid dynamic macro. Ensure that variables are listed in order (Ex. (*,*,...) = <0>, <1>,...)");
            //    }
            //    return;
            //}

            //if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
            //{
            //    if (isDynamic == false)
            //    {
            //        BotProgram.MsgHandler.QueueMessage("Invalid macro.");
            //    }
            //    else
            //    {
            //        BotProgram.MsgHandler.QueueMessage("Invalid dynamic macro. Ensure that variables are listed in order (Ex. (*,*,...) = <0>, <1>,...)");
            //    }
            //}
            //else
            //{
                string message = string.Empty;

                if (BotProgram.BotData.Macros.ContainsKey(macroName) == false)
                {
                    if (isDynamic == false)
                        message = $"Added macro {macroName}!";
                    else message = $"Added dynamic macro {macroName}! Dynamic macros can't be validated beforehand, so verify it works manually.";
                    
                    AddMacroToParserList(macroName);
                }
                else
                {
                    if (isDynamic == false)
                        message = $"Updated macro {macroName}!";
                    else message = $"Updated dynamic macro {macroName}! Dynamic macros can't be validated beforehand, so verify it works manually.";
                }

                BotProgram.BotData.Macros[macroName] = macroVal;
                BotProgram.SaveBotData();

                BotProgram.MsgHandler.QueueMessage(message);
            //}
        }

        private void AddMacroToParserList(string macroName)
        {
            DataInit.AddMacroToParserList(macroName, BotProgram.BotData.ParserMacroLookup);
        }
    }
}
