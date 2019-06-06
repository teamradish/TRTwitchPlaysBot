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
            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 2)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}addmacro usage: \"#macroname\" \"command\"");
                return;
            }

            string macroName = args[0].ToLowerInvariant();

            string macroVal = e.Command.ArgumentsAsString.Remove(0, macroName.Length + 1).ToLowerInvariant();

            if (macroName[0] != Globals.MacroIdentifier)
            {
                BotProgram.QueueMessage($"Macros must start with '{Globals.MacroIdentifier}'.");
                return;
            }

            if (macroName.Length > MAX_MACRO_LENGTH)
            {
                BotProgram.QueueMessage($"The max macro length is {MAX_MACRO_LENGTH} characters!");
                return;
            }

            bool isDynamic = false;
            string parsedVal = macroVal;

            //Check for a dynamic macro
            if (macroName.Contains("(*") == true)
            {
                isDynamic = true;

                //NOTE: We need to verify that the dynamic macro takes the form of "#macroname(*,*)"
                //It needs to have an open parenthesis followed by a number of asterisks separated by commas, then ending with a closed parenthesis
                string parseMsg = string.Empty;
                try
                {
                    parseMsg = Parser.expandify(Parser.populate_macros(parsedVal));
                }
                catch
                {
                    BotProgram.QueueMessage("Invalid dynamic macro. Ensure that variables are listed in order (Ex. (*,*,...) = <0>, <1>,...)");
                    return;
                }

                MatchCollection matches = Regex.Matches(parseMsg, @"<[0-9]+>");

                //Kimimaru: Replace all variables with a valid input to verify its validity
                //Any input will do, so just grab the first one
                string input = InputGlobals.ValidInputs[0];

                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];

                    parsedVal = parsedVal.Replace(match.Value, input);
                }
            }

            (bool valid, List<List<Parser.Input>> inputList, bool containsStartInput, int durationCounter)
                parsedData = default;

            try
            {
                //Parse the macro to check for valid input
                string parse_message = Parser.expandify(Parser.populate_macros(parsedVal));

                parsedData = Parser.Parse(parse_message);
            }
            catch
            {
                if (isDynamic == false)
                {
                    BotProgram.QueueMessage("Invalid macro.");
                }
                else
                {
                    BotProgram.QueueMessage("Invalid dynamic macro. Ensure that variables are listed in order (Ex. (*,*,...) = <0>, <1>,...)");
                }
                return;
            }

            if (parsedData.valid == false)
            {
                if (isDynamic == false)
                {
                    BotProgram.QueueMessage("Invalid macro.");
                }
                else
                {
                    BotProgram.QueueMessage("Invalid dynamic macro. Ensure that variables are listed in order (Ex. (*,*,...) = <0>, <1>,...)");
                }
            }
            else
            {
                string message = string.Empty;

                if (BotProgram.BotData.Macros.ContainsKey(macroName) == false)
                {
                    if (isDynamic == false)
                        message = $"Added macro {macroName}!";
                    else message = $"Added dynamic macro {macroName}!";
                }
                else
                {
                    if (isDynamic == false)
                        message = $"Updated macro {macroName}!";
                    else message = $"Updated dynamic macro {macroName}!";
                }

                BotProgram.BotData.Macros[macroName] = macroVal;
                BotProgram.SaveBotData();

                BotProgram.QueueMessage(message);
            }
        }
    }
}
