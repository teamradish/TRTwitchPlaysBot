using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Returns the length of an input or non-dynamic macro.
    /// </summary>
    public sealed class LengthCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string args = e.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(args) == true)
            {
                BotProgram.QueueMessage("Usage: \"Input\"");
                return;
            }

            //Parse the input
            //Parser.InputSequence inputSequence = default;
            (bool, List<List<Parser.Input>>, bool, int) parsedVal = default;

            try
            {
                string parse_message = Parser.Expandify(Parser.PopulateMacros(args));

                parsedVal = Parser.Parse(parse_message);
            }
            catch
            {
                //inputSequence.InputValidationType = Parser.InputValidationTypes.Invalid;
                parsedVal.Item1 = false;
            }

            if (parsedVal.Item1 == false)//inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
            {
                BotProgram.QueueMessage("Invalid input. Note that length cannot be determined for dynamic macros without inputs filled in.");
                return;
            }

            BotProgram.QueueMessage($"Total length: {parsedVal.Item4}ms");
        }
    }
}
