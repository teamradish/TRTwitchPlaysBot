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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Outputs an input sequence in natural language.
    /// </summary>
    public sealed class ReverseInputCommand : BaseCommand
    {
        public ReverseInputCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count == 0)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"input sequence\"");
                return;
            }

            string inputs = e.Command.ArgumentsAsString;

            Parser.InputSequence inputSequence = default;

            try
            {
                string parse_message = Parser.Expandify(Parser.PopulateMacros(inputs, BotProgram.BotData.Macros, BotProgram.BotData.ParserMacroLookup));
                parse_message = Parser.PopulateSynonyms(parse_message, InputGlobals.InputSynonyms);
                inputSequence = Parser.ParseInputs(parse_message, InputGlobals.ValidInputRegexStr, new Parser.ParserOptions(0, BotProgram.BotData.DefaultInputDuration, true, BotProgram.BotData.MaxInputDuration));

                if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
                {
                    BotProgram.MsgHandler.QueueMessage("Invalid input.");
                    return;
                }
            }
            catch
            {
                BotProgram.MsgHandler.QueueMessage("Invalid input.");
                return;
            }

            string reverseParsed = ReverseParser.ReverseParseNatural(inputSequence);

            //Handle empty message
            if (string.IsNullOrEmpty(reverseParsed) == true)
            {
                BotProgram.MsgHandler.QueueMessage("Reversed message is empty?!");
                return;
            }

            //For now don't allow inputs longer than char limit
            if (reverseParsed.Length > BotProgram.BotSettings.BotMessageCharLimit)
            {
                BotProgram.MsgHandler.QueueMessage("Reversed message is longer than character limit. Try doing a little at a time.");
                return;
            }

            BotProgram.MsgHandler.QueueMessage(reverseParsed);
        }
    }
}
