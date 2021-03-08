/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    public class AddInputSynonymCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"console name\" \"synonymName\" \"inputs\"";

        public AddInputSynonymCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count < 3)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.ADD_INPUT_SYNONYM_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to add input synonyms.");
                    return;
                }
            }

            string consoleName = arguments[0].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                //Check if a valid console is specified
                if (console == null)
                {
                    QueueMessage($"\"{consoleName}\" is not a valid console.");
                    return;
                }
            }

            string synonymName = arguments[1].ToLowerInvariant();

            //Get the actual synonym from the remaining arguments
            string synonymValue = args.Command.ArgumentsAsString.Remove(0, arguments[0].Length + 1).Remove(0, synonymName.Length + 1);

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);
                InputSynonym inputSynonym = context.InputSynonyms.FirstOrDefault(syn => syn.ConsoleID == console.ID && syn.SynonymName == synonymName);

                //Add if it doesn't exist
                if (inputSynonym == null)
                {
                    InputSynonym newSynonym = new InputSynonym(console.ID, synonymName, synonymValue);

                    context.InputSynonyms.Add(newSynonym);
                    QueueMessage($"Added input synonym \"{synonymName}\" for \"{synonymValue}\"!");
                }
                //Otherwise update it
                else
                {
                    inputSynonym.SynonymValue = synonymValue;
                    QueueMessage($"Updated input synonym \"{synonymName}\" for \"{synonymValue}\"!");
                }

                context.SaveChanges();
            }
        }
    }
}
