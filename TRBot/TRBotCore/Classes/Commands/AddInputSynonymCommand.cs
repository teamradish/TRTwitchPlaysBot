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
    public sealed class AddInputSynonymCommand : BaseCommand
    {
        public AddInputSynonymCommand()
        {
            
        }

        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 3)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"console\" \"synonymName\" \"input\"");
                return;
            }

            string consoleStr = args[0];

            //Check if a valid console is specified
            if (Enum.TryParse<InputGlobals.InputConsoles>(consoleStr, true, out InputGlobals.InputConsoles console) == false
                || InputGlobals.Consoles.ContainsKey(console) == false)
            {
                BotProgram.MsgHandler.QueueMessage($"Please specify a valid console.");
                return;
            }

            InputSynonymData inputSyns = BotProgram.BotData.InputSynonyms;

            string synonymName = args[1];

            //Get the actual synonym from the remaining arguments
            string actualSynonym = e.Command.ArgumentsAsString.Remove(0, args[0].Length + 1).Remove(0, args[1].Length + 1);

            //Add to the dictionary if it doesn't exist
            if (inputSyns.SynonymDict.TryGetValue(console, out Dictionary<string, string> dict) == false)
            {
                dict = new Dictionary<string, string>(8);
                inputSyns.SynonymDict[console] = dict;
            }

            string message = string.Empty;

            if (dict.ContainsKey(synonymName) == true)
            {
                message = $"Updated input synonym \"{synonymName}\" for \"{actualSynonym}\"!";
            }
            else
            {
                message = $"Added input synonym \"{synonymName}\" for \"{actualSynonym}\"!";
            }
            
            //Update value and save bot data
            dict[synonymName] = actualSynonym;

            BotProgram.SaveBotData();

            BotProgram.MsgHandler.QueueMessage(message);
        }
    }
}
