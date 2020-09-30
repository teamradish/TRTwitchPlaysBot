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
using TRBot.Connection;
using TRBot.Data;
using TRBot.Common;

namespace TRBot.Commands
{
    /// <summary>
    /// Manages commands.
    /// </summary>
    public class CommandHandler
    {
        private Dictionary<string, BaseCommand> AllCommands = new Dictionary<string, BaseCommand>(32);
        private BotMessageHandler MessageHandler = null;

        public CommandHandler()
        {
            AllCommands.Add("sourcecode", new MessageCommand(SettingsConstants.SOURCE_CODE_MESSAGE, string.Empty));
            AllCommands.Add("info", new MessageCommand(SettingsConstants.INFO_MESSAGE, string.Empty));
        }

        public void Initialize(BotMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;

            foreach (KeyValuePair<string, BaseCommand> cmd in AllCommands)
            {
                cmd.Value.Initialize(messageHandler);
            }
        }

        public void CleanUp()
        {
            MessageHandler = null;

            foreach (KeyValuePair<string, BaseCommand> cmd in AllCommands)
            {
                cmd.Value.CleanUp();
            }
        }

        public void HandleCommand(EvtChatCommandArgs args)
        {
            if (args == null || args.Command == null || args.Command.ChatMessage == null)
            {
                MessageHandler.QueueMessage($"{nameof(EvtChatCommandArgs)} or its Command or ChatMessage is null! Not parsing command");
                return;
            }

            string commandToLower = args.Command.CommandText.ToLower();

            if (AllCommands.TryGetValue(commandToLower, out BaseCommand command) == true)
            {
                if (command == null)
                {
                    MessageHandler.QueueMessage($"Command {commandToLower} is null! Not executing.");
                    return;
                }

                //Execute the command
                command.ExecuteCommand(args);
            }
        }
    }
}
