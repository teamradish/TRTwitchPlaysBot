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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that adds a game console.
    /// </summary>
    public sealed class AddConsoleCommand : BaseCommand
    {
        /// <summary>
        /// The max name length for new consoles.
        /// </summary>
        public const int MAX_NAME_LENGTH = 30;

        private string UsageMessage = $"Usage - \"console name\"";

        public AddConsoleCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Ignore with incorrect number of arguments
            if (arguments.Count != 1)
            {
                DataContainer.MessageHandler.QueueMessage(UsageMessage);
                return;
            }

            string consoleName = arguments[0].ToLowerInvariant();

            //Ensure the console name isn't too long
            if (consoleName.Length > MAX_NAME_LENGTH)
            {
                DataContainer.MessageHandler.QueueMessage($"Consoles may have up to a max of {MAX_NAME_LENGTH} characters in their name.");
                return;
            }

            //Allow only alphanumeric characters
            bool matched = Regex.IsMatch(consoleName, @"^[a-zA-Z0-9]+$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            if (matched == false)
            {
                DataContainer.MessageHandler.QueueMessage($"Consoles may contain only alphanumeric characters.");
                return;
            }

            //Note that by modifying the database directly, the streamer can bypass these restrictions
            //However this at least helps get across that consoles should have good names
            //Imagine a console named "#%*&+-\n" being available

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                if (console != null)
                {
                    DataContainer.MessageHandler.QueueMessage($"There already exists a console named \"{consoleName}\"!");
                    return;
                }

                context.Consoles.Add(new GameConsole(consoleName));

                context.SaveChanges();
            }

            DataContainer.MessageHandler.QueueMessage($"Added console \"{consoleName}\"!");
        }
    }
}
