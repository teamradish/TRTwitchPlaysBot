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
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace TRBot.Data
{
    public class SQLiteManager
    {
        public const string DEFAULT_SCHEMA_RELATIVE_ROOT = "SQLiteSchema";
        public const string DEFAULT_SCHEMA_FILE_NAME = "TRBotData.sql";

        private SQLiteConnection SqLiteConnection = null;
        private string DatabasePath = string.Empty;
        private string SchemaPath = string.Empty;

        public bool Initialized { get; private set; } = false;

        public SQLiteManager(string databasePath, string schemaPath)
        {
            DatabasePath = databasePath;
            SchemaPath = schemaPath;
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

            //Check if the database exists
            bool dbExists = File.Exists(DatabasePath);
            bool schemaExists = File.Exists(SchemaPath);

            SqLiteConnection = new SQLiteConnection($"data source={DatabasePath}");
            SqLiteConnection.Open();

            //Console.WriteLine($"dbExists at {DatabasePath}: {dbExists} | schemaExists at {SchemaPath}: {schemaExists}");

            DataTable tables = SqLiteConnection.GetSchema("Tables");

            //Console.WriteLine("Table Rows: " + tables.Rows.Count);

            //If the database didn't exist and the schema does,
            //or if the database is empty, update the db with the schema
            //In this case, empty also means having only the "sqlite_sequence" row in the "Tables" 
            if ((dbExists == false && schemaExists == true) || tables.Rows.Count <= 1)
            {
                //Read the schema
                string schemaText = File.ReadAllText(SchemaPath);

                using SQLiteCommand cmd = new SQLiteCommand();
                
                //Execute the schema to create the tables
                cmd.CommandText = schemaText;
                cmd.Connection = SqLiteConnection;
                cmd.ExecuteNonQuery();

                Console.WriteLine($"Imported schema at {SchemaPath} to database!");
            }

            Initialized = true;
        }

        public void PrintVersion()
        {
            string stm = "SELECT SQLITE_VERSION();";

            using SQLiteCommand cmd = new SQLiteCommand(stm, SqLiteConnection);
            string version = cmd.ExecuteScalar().ToString();
            
            Console.WriteLine($"SQLite version: {version}");
        }

        public void CleanUp()
        {
            SqLiteConnection?.Dispose();
            SqLiteConnection = null;

            Initialized = false;
        }
    }
}