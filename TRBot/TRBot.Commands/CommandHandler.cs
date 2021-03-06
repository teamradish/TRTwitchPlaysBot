﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TRBot.Connection;
using TRBot.Data;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Routines;
using TRBot.Permissions;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// Manages commands.
    /// </summary>
    public class CommandHandler
    {
        private ConcurrentDictionary<string, BaseCommand> AllCommands = new ConcurrentDictionary<string, BaseCommand>(Environment.ProcessorCount * 2, 32);
        private DataContainer DataContainer = null;
        private BotRoutineHandler RoutineHandler = null;

        /// <summary>
        /// Additional assemblies to look in when adding commands.
        /// This is useful if the command's Type is outside this assembly.
        /// </summary>
        private Assembly[] AdditionalAssemblies = Array.Empty<Assembly>();

        public CommandHandler()
        {

        }

        public CommandHandler(Assembly[] additionalAssemblies)
        {
            AdditionalAssemblies = additionalAssemblies;
        }

        public void Initialize(DataContainer dataContainer, BotRoutineHandler routineHandler)
        {
            DataContainer = dataContainer;

            DataContainer.DataReloader.SoftDataReloadedEvent -= OnDataReloadedSoft;
            DataContainer.DataReloader.SoftDataReloadedEvent += OnDataReloadedSoft;

            DataContainer.DataReloader.HardDataReloadedEvent -= OnDataReloadedHard;
            DataContainer.DataReloader.HardDataReloadedEvent += OnDataReloadedHard;

            RoutineHandler = routineHandler;

            PopulateCommandsFromDB();
        }

        public void CleanUp()
        {
            DataContainer.DataReloader.SoftDataReloadedEvent -= OnDataReloadedSoft;
            DataContainer.DataReloader.HardDataReloadedEvent -= OnDataReloadedHard;

            DataContainer = null;
            RoutineHandler = null;

            CleanUpCommands();
        }

        public void HandleCommand(EvtChatCommandArgs args)
        {
            if (args == null || args.Command == null || args.Command.ChatMessage == null)
            {
                DataContainer.MessageHandler.QueueMessage($"{nameof(EvtChatCommandArgs)} or its Command or ChatMessage is null! Not parsing command");
                return;
            }

            string commandToLower = args.Command.CommandText.ToLower();

            if (AllCommands.TryGetValue(commandToLower, out BaseCommand command) == true)
            {
                if (command == null)
                {
                    DataContainer.MessageHandler.QueueMessage($"Command {commandToLower} is null! Not executing.");
                    return;
                }

                //Return if the command is disabled
                if (command.Enabled == false)
                {
                    return;
                }

                long userLevel = 0L;

                //Check if the user has permission to perform this command
                User user = DataHelper.GetUser(args.Command.ChatMessage.Username);
                
                if (user != null)
                {
                    userLevel = user.Level;
                }

                if (userLevel < command.Level)
                {
                    DataContainer.MessageHandler.QueueMessage($"You need at least level {command.Level} ({(PermissionLevels)command.Level}) to perform that command!");
                    return;
                }

                //Execute the command
                command.ExecuteCommand(args);
            }
        }

        public BaseCommand GetCommand(string commandName)
        {
            AllCommands.TryGetValue(commandName, out BaseCommand command);

            return command;
        }

        public bool AddCommand(string commandName, string commandTypeName, string valueStr,
            in long level, in bool commandEnabled, in bool displayInHelp)
        {
            Type commandType = Type.GetType(commandTypeName, false, true);
            if (commandType == null && AdditionalAssemblies?.Length > 0)
            {
                //Look for the type in our other assemblies
                for (int i = 0; i < AdditionalAssemblies.Length; i++)
                {
                    Assembly asm = AdditionalAssemblies[i];

                    commandType = asm.GetType(commandTypeName, false, true);
                    
                    if (commandType != null)
                    {
                        TRBotLogger.Logger.Debug($"Found \"{commandTypeName}\" in assembly \"{asm.GetName()}\"!");
                        break;
                    }
                }                
            }

            if (commandType == null)
            {
                DataContainer.MessageHandler.QueueMessage($"Cannot find command type \"{commandTypeName}\" for command \"{commandName}\" in all provided assemblies.");
                return false;
            }

            BaseCommand command = null;

            //Try to create an instance
            try
            {
                command = (BaseCommand)Activator.CreateInstance(commandType, Array.Empty<object>());
                command.Enabled = commandEnabled;
                command.DisplayInHelp = displayInHelp;
                command.Level = level;
                command.ValueStr = valueStr;
            }
            catch (Exception e)
            {
                DataContainer.MessageHandler.QueueMessage($"Unable to add command \"{commandName}\": \"{e.Message}\"");
            }

            return AddCommand(commandName, command);
        }

        public bool AddCommand(string commandName, BaseCommand command)
        {
            if (command == null)
            {
                TRBotLogger.Logger.Warning("Cannot add null command.");
                return false;
            }

            //Clean up the existing command before overwriting it with the new value
            if (AllCommands.TryGetValue(commandName, out BaseCommand existingCmd) == true)
            {
                existingCmd.CleanUp();
            }

            //Set and initialize the command
            AllCommands[commandName] = command;
            AllCommands[commandName].SetRequiredData(this, DataContainer, RoutineHandler);
            AllCommands[commandName].Initialize();

            return true;
        }

        public bool RemoveCommand(string commandName)
        {
            bool removed = AllCommands.Remove(commandName, out BaseCommand command);
            
            //Clean up the command
            command?.CleanUp();

            return removed;
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
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                foreach (CommandData cmdData in context.Commands)
                {
                    AddCommand(cmdData.Name, cmdData.ClassName, cmdData.ValueStr, cmdData.Level, cmdData.Enabled > 0, cmdData.DisplayInList > 0);
                }
            }
        }

        private void OnDataReloadedSoft()
        {
            UpdateCommandsFromDB();
        }

        private void OnDataReloadedHard()
        {
            //Clean up and clear all commands
            CleanUpCommands();
            AllCommands.Clear();

            PopulateCommandsFromDB();
        }

        private void UpdateCommandsFromDB()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                List<string> encounteredCommands = new List<string>(context.Commands.Count());

                foreach (CommandData cmdData in context.Commands)
                {
                    string commandName = cmdData.Name;
                    if (AllCommands.TryGetValue(commandName, out BaseCommand baseCmd) == true)
                    {
                        //Remove this command if the type name is different so we can reconstruct it
                        if (baseCmd.GetType().FullName != cmdData.ClassName)
                        {
                            RemoveCommand(commandName);
                        }

                        baseCmd = null;
                    }

                    //Add this command if it doesn't exist and should
                    if (baseCmd == null)
                    {
                        //Add this command
                        AddCommand(commandName, cmdData.ClassName, cmdData.ValueStr,
                            (int)cmdData.Level, cmdData.Enabled > 0, cmdData.DisplayInList > 0 );
                    }
                    else
                    {
                        baseCmd.Level = (int)cmdData.Level;
                        baseCmd.Enabled = cmdData.Enabled > 0;
                        baseCmd.DisplayInHelp = cmdData.DisplayInList > 0;
                        baseCmd.ValueStr = cmdData.ValueStr;
                    }

                    encounteredCommands.Add(commandName);
                }

                //Remove commands that are no longer in the database
                foreach (string cmd in AllCommands.Keys)
                {
                    if (encounteredCommands.Contains(cmd) == false)
                    {
                        RemoveCommand(cmd);
                    }
                }
            }
        }
    }
}
