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
using System.Text;
using System.IO;
using Microsoft.Data.Sqlite;

namespace TRBot.Data
{
    public class SQLiteManager
    {
        private SqliteConnection SQLiteConnection = null;
        private string DatabasePath = string.Empty;

        public bool Initialized { get; private set; } = false;

        public SQLiteManager(string databasePath)
        {
            DatabasePath = databasePath;
        }

        public void Initialize()
        {
            if (Initialized == true)
            {
                return;
            }

            //Get the directory name for the database file
            string dirName = Path.GetDirectoryName(DatabasePath);
            //Console.WriteLine(dirName);

            //Create the directory if it doesn't exist
            if (Directory.Exists(dirName) == false)
            {
                Directory.CreateDirectory(dirName);
            }

            SQLiteConnection = new SqliteConnection($"data source={DatabasePath}");
            SQLiteConnection.Open();

            Initialized = true;
        }

        public void PrintVersion()
        {
            string stm = "SELECT SQLITE_VERSION();";

            using SqliteCommand cmd = new SqliteCommand(stm, SQLiteConnection);
            string version = cmd.ExecuteScalar().ToString();
            
            Console.WriteLine($"SQLite version: {version}");
        }

        public void CleanUp()
        {
            SQLiteConnection?.Dispose();
            SQLiteConnection = null;

            Initialized = false;
        }
    }
}