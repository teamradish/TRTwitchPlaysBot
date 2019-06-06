using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// A command listing all the valid inputs for the current console.
    /// </summary>
    public sealed class ValidInputsCommand : BaseCommand
    {
        private StringBuilder StrBuilder = new StringBuilder(500);

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string[] validInputs = InputGlobals.CurrentConsole.ValidInputs;

            if (validInputs == null || validInputs.Length == 0)
            {
                BotProgram.QueueMessage($"Interesting! There are no valid inputs for {InputGlobals.CurrentConsoleVal}?!");
                return;
            }

            StrBuilder.Clear();

            StrBuilder.Append("Valid inputs for ").Append(InputGlobals.CurrentConsoleVal.ToString()).Append(": ");

            for (int i = 0; i < validInputs.Length; i++)
            {
                StrBuilder.Append(validInputs[i]).Append(", ");
            }

            //Remove trailing comma
            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            string validInputsStr = StrBuilder.ToString();

            BotProgram.QueueMessage(validInputsStr);
        }
    }
}
