using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Adds a macro.
    /// </summary>
    public sealed class AddMacroCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = 2;
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

            //Check for a dynamic macro
            if (macroName.Contains("(*") == true)
            {
                BotProgram.BotData.Macros[macroName] = macroVal;
                BotProgram.SaveBotData();
            
                BotProgram.QueueMessage($"Added dynamic macro {macroName}. NOTE: dynamic macro contents cannot be verified upon creation.");
            
                return;
            }

            (bool valid, List<List<Parser.Input>> inputList, bool containsStartInput, int durationCounter)
                parsedData = default;

            try
            {
                //Parse the macro to check for valid input
                string parse_message = Parser.expandify(Parser.populate_macros(macroVal));

                parsedData = Parser.Parse(parse_message);
            }
            catch
            {
                BotProgram.QueueMessage("Invalid macro.");
                return;
            }

            if (parsedData.valid == false)
            {
                BotProgram.QueueMessage("Invalid macro.");
            }
            else
            {
                string message = string.Empty;

                if (BotProgram.BotData.Macros.ContainsKey(macroName) == false)
                {
                    message = $"Added macro {macroName}!";
                }
                else
                {
                    message = $"Updated macro {macroName}!";
                }

                BotProgram.BotData.Macros[macroName] = macroVal;
                BotProgram.SaveBotData();

                BotProgram.QueueMessage(message);
            }
        }
    }
}
