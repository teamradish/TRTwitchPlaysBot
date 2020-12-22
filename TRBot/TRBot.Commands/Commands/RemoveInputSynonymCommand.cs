/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    public class RemoveInputSynonymCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"console name\" \"synonymName\"";

        public RemoveInputSynonymCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleName = arguments[0].ToLowerInvariant();
            long consoleID = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                //Check if a valid console is specified
                if (console == null)
                {
                    QueueMessage($"\"{consoleName}\" is not a valid console.");
                    return;
                }

                consoleID = console.ID;
            }

            string synonymName = arguments[1].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                InputSynonym inputSynonym = context.InputSynonyms.FirstOrDefault(syn => syn.ConsoleID == consoleID && syn.SynonymName == synonymName);

                if (inputSynonym == null)
                {
                    QueueMessage($"No input synonym \"{synonymName}\" exists for console {consoleName}.");
                    return;
                }

                context.InputSynonyms.Remove(inputSynonym);
                context.SaveChanges();
            }

            QueueMessage($"Successfully removed input synonym \"{synonymName}\"!");
        }
    }
}
