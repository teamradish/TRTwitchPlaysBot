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
            (bool valid, List<List<Parser.Input>> inputList, bool containsStartInput, int durationCounter)
                        parsedData = default;

            try
            {
                string parse_message = Parser.Expandify(Parser.PopulateMacros(args));

                parsedData = Parser.Parse(parse_message);
            }
            catch
            {
                parsedData.valid = false;
            }

            if (parsedData.valid == false)
            {
                BotProgram.QueueMessage("Invalid input. Note that length cannot be determined for dynamic macros without inputs filled in.");
                return;
            }

            BotProgram.QueueMessage($"Total length: {parsedData.durationCounter}ms");
        }
    }
}
