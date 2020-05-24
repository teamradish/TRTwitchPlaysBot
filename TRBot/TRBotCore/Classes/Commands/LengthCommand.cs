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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Returns the length of an input or non-dynamic macro.
    /// </summary>
    public sealed class LengthCommand : BaseCommand
    {
        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            string args = e.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(args) == true)
            {
                BotProgram.QueueMessage("Usage: \"Input\"");
                return;
            }

            //Parse the input
            //Parser.InputSequence inputSequence = default;
            //(bool, List<List<Parser.Input>>, bool, int) parsedVal = default;
            Parser.InputSequence inputSequence = default;

            try
            {
                string parse_message = Parser.Expandify(Parser.PopulateMacros(args));

                inputSequence = Parser.ParseInputs(parse_message, true);
                //parsedVal = Parser.Parse(parse_message);
            }
            catch
            {
                inputSequence.InputValidationType = Parser.InputValidationTypes.Invalid;
                //parsedVal.Item1 = false;
            }

            if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
            {
                BotProgram.QueueMessage("Invalid input. Note that length cannot be determined for dynamic macros without inputs filled in.");
                return;
            }

            BotProgram.QueueMessage($"Total length: {inputSequence.TotalDuration}ms");
        }
    }
}
