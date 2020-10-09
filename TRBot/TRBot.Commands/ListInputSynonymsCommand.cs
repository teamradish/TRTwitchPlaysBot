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
using TRBot.Connection;
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.ParserData;
using TRBot.Data;

namespace TRBot.Commands
{
    public class ListInputSynonymsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"console name\"";

        public ListInputSynonymsCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string consoleName = arguments[0].ToLowerInvariant();

            using BotDBContext context = DatabaseManager.OpenContext();

            GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

            //Check if a valid console is specified
            if (console == null)
            {
                QueueMessage($"\"{consoleName}\" is not a valid console.");
                return;
            }

            IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.console_id == console.id);

            int count = synonyms.Count();
            
            if (count == 0)
            {
                QueueMessage($"The {consoleName} console does not have any input synonyms.");
                return;
            }

            StringBuilder stringBuilder = new StringBuilder(count * 15);

            stringBuilder.Append("Synonyms for ").Append(console.Name).Append(':').Append(' ');

            //Show all input synonyms for this console
            foreach (InputSynonym synonym in synonyms)
            {
                stringBuilder.Append('{').Append(' ').Append(synonym.SynonymName).Append(',').Append(' ');
                stringBuilder.Append(synonym.SynonymValue).Append(' ').Append('}').Append(',').Append(' ');
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            int maxCharCount = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.BOT_MSG_CHAR_LIMIT, context, 500L);

            QueueMessageSplit(stringBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
