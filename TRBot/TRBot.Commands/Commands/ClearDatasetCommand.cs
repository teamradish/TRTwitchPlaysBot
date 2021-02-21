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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that clears a database table in the database.
    /// </summary>
    public sealed class ClearDatasetCommand : BaseCommand
    {
        private const string CONFIRMATION_ARG = "confirm";

        private string UsageMessage = "Usage: (\"settings\", \"gamelogs\", \"commands\", \"memes\", \"macros\", \"synonyms\", \"consoles\", \"users\", or \"permabilities\"), \"confirmation (string)\"";
        private Dictionary<string, Action> ClearDict = null;

        //Check the users attempting to clear the database tables to give an additional confirmation
        private Dictionary<string, List<string>> UserClearAttempts = new Dictionary<string, List<string>>(4);

        public ClearDatasetCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            ClearDict = new Dictionary<string, Action>(9)
            {
                { "settings", ClearSettings },
                { "gamelogs", ClearGamelogs },
                { "commands", ClearCommands },
                { "memes", ClearMemes },
                { "macros", ClearMacros },
                { "synonyms", ClearSynonyms },
                { "consoles", ClearConsoles },
                { "users", ClearUsers },
                { "permabilities", ClearPermAbilities },
            };
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not the correct number of arguments
            if (argCount < 1 || argCount > 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = args.Command.ChatMessage.Username;
            string tableName = arguments[0].ToLowerInvariant();

            //Check for a table name
            if (ClearDict.TryGetValue(tableName, out Action invokedAction) == false)
            {
                QueueMessage("Invalid table name!");
                return;
            }

            //Add the user to the list if so
            if (UserClearAttempts.TryGetValue(args.Command.ChatMessage.Username, out List<string> tableList) == false)
            {
                tableList = new List<string>(9);
                UserClearAttempts.Add(userName, tableList);
            }

            //Check if it contains this table
            if (tableList.Contains(tableName) == false)
            {
                tableList.Add(tableName);

                QueueMessage($"Are you ABSOLUTELY sure you want to clear the \"{tableName}\" table in the database? Supply \"{CONFIRMATION_ARG}\" as the second argument if you're 100% sure.");
                return;
            }

            //They have to supply confirm as an argument, so make sure they know
            if (argCount == 1)
            {
                QueueMessage($"Are you ABSOLUTELY sure you want to clear the \"{tableName}\" table in the database? Supply \"{CONFIRMATION_ARG}\" as the second argument if you're 100% sure.");
                return;
            }

            string confirmationArg = arguments[1].ToLowerInvariant();

            //Incorrect argument - it has to be exact
            if (confirmationArg != CONFIRMATION_ARG)
            {
                QueueMessage($"Invalid argument. Please explictly pass \"{CONFIRMATION_ARG}\" as the second argument if you're 100% sure you want to clear the \"{tableName}\" table in the database.");
                return;
            }

            //Invoke the action - this should clear the appropriate table
            invokedAction.Invoke();

            //Remove the table name from the list
            tableList.Remove(tableName);

            //If the list is now empty, remove the user from the dictionary
            if (tableList.Count == 0)
            {
                UserClearAttempts.Remove(userName);
            }

            QueueMessage($"Successfully cleared the \"{tableName}\" table!");
        }

        private void ClearSettings()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings[] settings = context.SettingCollection.ToArray();
                for (int i = 0; i < settings.Length; i++)
                {
                    context.SettingCollection.Remove(settings[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearGamelogs()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameLog[] logs = context.GameLogs.ToArray();
                for (int i = 0; i < logs.Length; i++)
                {
                    context.GameLogs.Remove(logs[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearCommands()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                CommandData[] cmds = context.Commands.ToArray();
                for (int i = 0; i < cmds.Length; i++)
                {
                    context.Commands.Remove(cmds[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearMemes()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Meme[] memes = context.Memes.ToArray();
                for (int i = 0; i < memes.Length; i++)
                {
                    context.Memes.Remove(memes[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearMacros()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                InputMacro[] macros = context.Macros.ToArray();
                for (int i = 0; i < macros.Length; i++)
                {
                    context.Macros.Remove(macros[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearSynonyms()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                InputSynonym[] synonyms = context.InputSynonyms.ToArray();
                for (int i = 0; i < synonyms.Length; i++)
                {
                    context.InputSynonyms.Remove(synonyms[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearConsoles()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole[] consoles = context.Consoles.ToArray();
                for (int i = 0; i < consoles.Length; i++)
                {
                    context.Consoles.Remove(consoles[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearUsers()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User[] users = context.Users.ToArray();
                for (int i = 0; i < users.Length; i++)
                {
                    context.Users.Remove(users[i]);
                }

                context.SaveChanges();
            }
        }

        private void ClearPermAbilities()
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                PermissionAbility[] permAbilities = context.PermAbilities.ToArray();
                for (int i = 0; i < permAbilities.Length; i++)
                {
                    context.PermAbilities.Remove(permAbilities[i]);
                }

                context.SaveChanges();
            }
        }
    }
}
