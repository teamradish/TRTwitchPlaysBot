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
    public sealed class RemoveInputSynonymCommand : BaseCommand
    {
        public RemoveInputSynonymCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 2)
            {
                BotProgram.QueueMessage("Usage: \"console\" \"synonymName\"");
                return;
            }

            string consoleStr = args[0];

            //Check for a valid console
            if (Enum.TryParse<InputGlobals.InputConsoles>(consoleStr, true, out InputGlobals.InputConsoles console) == false
                || InputGlobals.Consoles.ContainsKey(console) == false)
            {
                BotProgram.QueueMessage($"Please specify a valid console.");
                return;
            }

            InputSynonymData inputSyns = BotProgram.BotData.InputSynonyms;

            string synonymName = args[1];

            //Check if input synonyms exist for this console
            if (inputSyns.SynonymDict.TryGetValue(console, out Dictionary<string, string> dict) == false)
            {
                BotProgram.QueueMessage($"No input synonyms exist for console {console}!");
                return;
            }

            //Remove the synonym
            if (dict.Remove(synonymName) == false)
            {
                BotProgram.QueueMessage($"No input synonym \"{synonymName}\" exists for console {console}!");
                return;
            }

            //Save bot data
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Successfully removed input synonym \"{synonymName}\"!");
        }
    }
}
