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
    /// A command that exports a database table to a text file.
    /// </summary>
    public sealed class ExportDatasetToTextCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"settings\", \"gamelogs\", \"commands\", \"memes\", \"macros\", \"synonyms\", \"consoles\", \"users\", or \"permabilities\"";
        private Dictionary<string, Action<BotDBContext>> ExportDict = null;

        public ExportDatasetToTextCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            ExportDict = new Dictionary<string, Action<BotDBContext>>(9)
            {
                { "settings", ExportSettings },
                { "gamelogs", ExportGamelogs },
                { "commands", ExportCommands },
                { "memes", ExportMemes },
                { "macros", ExportMacros },
                { "synonyms", ExportSynonyms },
                { "consoles", ExportConsoles },
                { "users", ExportUsers },
                { "permabilities", ExportPermAbilities },
            };
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            int argCount = arguments.Count;

            //Ignore with not the correct number of arguments
            if (argCount != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string tableName = arguments[0].ToLowerInvariant();

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                if (ExportDict.TryGetValue(tableName, out Action<BotDBContext> val) == true)
                {
                    val.Invoke(context);
                }
            }
        }

        private void ExportSettings(BotDBContext context)
        {
            ExportTableToText("settings", context.SettingCollection);
        }

        private void ExportGamelogs(BotDBContext context)
        {
            ExportTableToText("gamelogs", context.GameLogs);
        }

        private void ExportCommands(BotDBContext context)
        {
            ExportTableToText("commands", context.Commands);
        }

        private void ExportMemes(BotDBContext context)
        {
            ExportTableToText("memes", context.Memes);
        }

        private void ExportMacros(BotDBContext context)
        {
            ExportTableToText("macros", context.Macros);
        }

        private void ExportSynonyms(BotDBContext context)
        {
            ExportTableToText("synonyms", context.InputSynonyms);
        }

        private void ExportConsoles(BotDBContext context)
        {
            ExportTableToText("consoles", context.Consoles);
        }

        private void ExportUsers(BotDBContext context)
        {
            ExportTableToText("users", context.Users);
        }

        private void ExportPermAbilities(BotDBContext context)
        {
            ExportTableToText("permabilities", context.PermAbilities);
        }

        private void ExportTableToText<T>(string dataName, DbSet<T> dbSet) where T: class
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

            string json = JsonConvert.SerializeObject(dbSet, Formatting.Indented, serializerSettings);
            string dateStr = Debug.GetFileFriendlyTimeStamp(DateTime.Now);

            string fileName = $"{dataName} - {dateStr}.txt";

            string finalPath = Path.Combine(DataConstants.DataFolderPath, fileName);

            if (FileHelpers.SaveToTextFile(finalPath, json) == false)
            {
                QueueMessage($"Failed saving \"{dataName}\" to a file. Please double check your folder permissions and try again.");
            }
            else
            {
                QueueMessage($"Successfully exported data for \"{dataName}\"!");
            }
        }
    }
}
