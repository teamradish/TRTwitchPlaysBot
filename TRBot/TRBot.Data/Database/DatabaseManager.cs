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
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static TRBot.Data.DatabaseDelegates;

namespace TRBot.Data
{
    /// <summary>
    /// Helps manage the database.
    /// </summary>
    public static class DatabaseManager
    {
        /// <summary>
        /// The path to the database.
        /// </summary>
        public static string DatabasePath { get; private set; } = string.Empty;
        
        /// <summary>
        /// Sets the path for the database.
        /// </summary>
        /// <param name="databasePath">The path to the database.</param>
        public static void SetDatabasePath(in string databasePath)
        {
            DatabasePath = databasePath;
        }

        /// <summary>
        /// Opens a database context and returns it.
        /// The caller is responsible for disposing the context. 
        /// </summary>
        /// <returns>An opened <see cref="BotDBContext" />.</returns>
        public static BotDBContext OpenContext()
        {
            BotDBContext context = new BotDBContext(DatabasePath);
            return context;
        }

        /// <summary>
        /// Opens up a database context and applies any migrations.
        /// If the database does not exist, it will be created.
        /// </summary>
        public static void InitAndMigrateContext()
        {
            using (BotDBContext context = OpenContext())
            {
                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Opens the database context, invokes an action, then disposes the context.
        /// </summary>
        /// <param name="dbContextAction">The action to perform on the database context.</param>
        public static void OpenCloseContext(DBContextAction<BotDBContext> dbContextAction)
        {
            if (dbContextAction == null)
            {
                throw new NullReferenceException($"{nameof(dbContextAction)} is null.");
            }

            //Open the context and perform the action
            using (BotDBContext botDBContext = OpenContext())
            {
                dbContextAction.Invoke(botDBContext);
            }
        }
    }
}