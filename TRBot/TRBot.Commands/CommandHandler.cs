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
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Manages commands.
    /// </summary>
    public class CommandHandler
    {
        private Dictionary<string, BaseCommand> AllCommands = new Dictionary<string, BaseCommand>(32);
        private BotMessageHandler MessageHandler = null;
        private DataReloader DataReloader = null;

        public CommandHandler()
        {
            //AllCommands.Add("sourcecode", new MessageCommand(SettingsConstants.SOURCE_CODE_MESSAGE, string.Empty));
            //AllCommands.Add("info", new MessageCommand(SettingsConstants.INFO_MESSAGE, string.Empty));
        }

        public void Initialize(BotMessageHandler messageHandler, DataReloader dataReloader)
        {
            MessageHandler = messageHandler;
            DataReloader = dataReloader;

            DataReloader.DataReloadedEvent -= OnDataReloaded;
            DataReloader.DataReloadedEvent += OnDataReloaded;

            PopulateCommandsFromDB();            
            InitializeCommands();
        }

        public void CleanUp()
        {
            DataReloader.DataReloadedEvent -= OnDataReloaded;

            MessageHandler = null;
            DataReloader = null;

            CleanUpCommands();
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

        private void InitializeCommands()
        {
            foreach (KeyValuePair<string, BaseCommand> cmd in AllCommands)
            {
                cmd.Value.Initialize(MessageHandler, DataReloader);
            }
        }

        private void CleanUpCommands()
        {
            foreach (KeyValuePair<string, BaseCommand> cmd in AllCommands)
            {
                cmd.Value.CleanUp();
            }
        }

        private void PopulateCommandsFromDB()
        {
            object[] parameters = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                foreach (CommandData cmdData in context.Commands)
                {
                    //Find the type corresponding to this class name
                    Type commandType = Type.GetType(cmdData.class_name, false, true);
                    if (commandType == null)
                    {
                        MessageHandler.QueueMessage($"Cannot find command type \"{cmdData.class_name}\": skipping");
                        continue;
                    }

                    //Get arguments
                    object[] constructorParams = Array.Empty<object>();
                    if (string.IsNullOrEmpty(cmdData.value_str) == false)
                    {
                        if (parameters == null)
                        {
                            parameters = new object[1] { cmdData.value_str };
                        }
                        else
                        {
                            parameters[0] = cmdData.value_str;
                        }

                        constructorParams = parameters;
                    }

                    //Create the type
                    try
                    {
                        AllCommands[cmdData.name] = (BaseCommand)Activator.CreateInstance(commandType, constructorParams);
                    }
                    catch (Exception e)
                    {
                        MessageHandler.QueueMessage($"Unable to create class type \"{cmdData.class_name}\": {e.Message}");
                    }
                }
            }
        }

        private void OnDataReloaded()
        {
            //Clean up and clear all commands
            CleanUpCommands();
            AllCommands.Clear();

            PopulateCommandsFromDB();

            //Re-initialize all commands
            InitializeCommands();
        }
    }
}
