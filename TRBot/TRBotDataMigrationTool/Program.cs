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
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using TRBot.Utilities;
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Parsing;
using TRBot.Consoles;
using Newtonsoft.Json;

namespace TRBotDataMigrationTool
{
    class Program
    {
        private const string SKIP_ARG = "skip";
        private const string DATETIME_AT_PARSE = " at ";

        private const string OLD_BOT_DATA_FILE = "BotData.txt";
        private const string OLD_SETTINGS_FILE = "Settings.txt";

        private static readonly string OldBotDataFileMessage = $"Please specify the location of the \"{OLD_BOT_DATA_FILE}\" file from your TRBot 1.8 install. Type \"{SKIP_ARG}\" to skip this step.";
        private static readonly string OldSettingsFileMessage = $"Please specify the location of the \"{OLD_SETTINGS_FILE}\" file from your TRBot 1.8 install, Type \"{SKIP_ARG}\" to skip this step.";

        private static Dictionary<AccessLevels.Levels, PermissionLevels> AccessLvlMap = new Dictionary<AccessLevels.Levels, TRBot.Permissions.PermissionLevels>(5)
        {
            { AccessLevels.Levels.User, PermissionLevels.User },
            { AccessLevels.Levels.Whitelisted, PermissionLevels.Whitelisted },
            { AccessLevels.Levels.VIP, PermissionLevels.VIP },
            { AccessLevels.Levels.Moderator, PermissionLevels.Moderator },
            { AccessLevels.Levels.Admin, PermissionLevels.Admin }
        };

        static void Main(string[] args)
        {
            //Use invariant culture
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine($"Welcome to the TRBot 1.8 to 2.0+ data migration tool! This is currently running TRBot version {Application.VERSION_NUMBER}.\n");
            Console.WriteLine();
            Console.WriteLine("Keep in mind that the 2.0+ releases have a vastly different data structure, so some data may not be able to be migrated.\nPress any key to continue.\n");
            Console.ReadKey();

            Console.WriteLine($"The first thing we are going to do is create a template database file for your new 2.0+ data. You can also provide an existing \"{DataConstants.DATABASE_FILE_NAME}\" data file in this application's \"{DataConstants.DATA_FOLDER_NAME}\" folder to migrate the data to. HOWEVER, keep in mind that doing so WILL OVERWRITE EXISTING DATA! Please be careful.\nPress any key to continue.\n");
            Console.ReadKey();
            
            Console.WriteLine("Initializing the template file now...");

            //Make template
            MakeTemplateDatabaseFile();

            Console.WriteLine("Template file created and/or validated!");
            Console.WriteLine(OldBotDataFileMessage);
            Console.WriteLine();

            bool fileExists = false;
            bool skippedData = false;

            //Bot data
            do
            {
                string line = Console.ReadLine();
                
                //Skip if we hit the skip argument
                if (line == SKIP_ARG)
                {
                    skippedData = true;
                    break;
                }

                fileExists = File.Exists(line);
                if (fileExists == false)
                {
                    Console.WriteLine($"Sorry, I can't find the file specified.\n{OldBotDataFileMessage}");
                    Console.WriteLine();
                    continue;
                }

                string botDataStr = string.Empty;

                //Try to read the file and put it in the database
                try
                {
                    botDataStr = File.ReadAllText(line);
                    BotData botData = JsonConvert.DeserializeObject<BotData>(botDataStr);
                    
                    //Convert all the bot data
                    AddOldBotDataToNewDB(botData);
                }
                catch (Exception e)
                {
                    fileExists = false;
                    Console.WriteLine($"Sorry, I was unable to read data from the given file for the following reason: {e.Message}\n{e.StackTrace}\n\n{OldBotDataFileMessage}");
                    continue;
                }
            }
            while (fileExists == false);

            //Reset flag
            fileExists = false;
            bool skippedSettings = false;

            Console.WriteLine(OldSettingsFileMessage);
            Console.WriteLine();

            //Bot settings
            do
            {
                string line = Console.ReadLine();
                
                //Skip if we hit the skip argument
                if (line == SKIP_ARG)
                {
                    skippedSettings = true;
                    break;
                }

                fileExists = File.Exists(line);
                if (fileExists == false)
                {
                    Console.WriteLine($"Sorry, I can't find the file specified.\n{OldSettingsFileMessage}");
                    Console.WriteLine();
                    continue;
                }

                string settingsDataStr = string.Empty;

                //Try to read the file and put it in the database
                try
                {
                    settingsDataStr = File.ReadAllText(line);
                    TRBotDataMigrationTool.Settings botSettings = JsonConvert.DeserializeObject<TRBotDataMigrationTool.Settings>(settingsDataStr);
                    
                    //Convert all the settings data
                    AddOldBotSettingsToNewDB(botSettings);
                }
                catch (Exception e)
                {
                    fileExists = false;
                    Console.WriteLine($"Sorry, I was unable to read data from the given file for the following reason: {e.Message}\n{e.StackTrace}\n\n{OldSettingsFileMessage}");
                    continue;
                }
            }
            while (fileExists == false);

            if (skippedData == false || skippedSettings == false)
            {
                Console.WriteLine("All bot data and settings have been imported! Don't forget to double check the data and make sure it's fine!");
            }
        }

        private static void MakeTemplateDatabaseFile()
        {
            //Initialize database
            string databasePath = Path.Combine(DataConstants.DataFolderPath, DataConstants.DATABASE_FILE_NAME);

            Console.WriteLine($"Validating database at: {databasePath}");
            if (FileHelpers.ValidatePathForFile(databasePath) == false)
            {
                Console.WriteLine($"Cannot create database path at {databasePath}. Check if you have permission to write to this directory. Aborting.");
                return;
            }

            Console.WriteLine("Database path validated! Initializing database and importing migrations.");

            DatabaseManager.SetDatabasePath(databasePath);
            DatabaseManager.InitAndMigrateContext();
            
            Console.WriteLine("Checking to initialize default values for missing database entries.");

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check for and initialize default values if the database was newly created or needs updating
                int addedDefaultEntries = DataHelper.InitDefaultData(context);

                if (addedDefaultEntries > 0)
                {
                    context.SaveChanges();
                    Console.WriteLine($"Added {addedDefaultEntries} additional entries to the database.");
                }
            }
        }

        private static void AddOldBotDataToNewDB(BotData oldBotData)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            /* Macros */
            Console.WriteLine("Begin importing macros...");
            int macroCount = 0;

            foreach (KeyValuePair<string, string> macros in oldBotData.Macros)
            {
                Console.WriteLine($"Importing macro \"{macros.Key}\" to new data.");

                InputMacro newMacro = context.Macros.FirstOrDefault(m => m.MacroName == macros.Key);
                if (newMacro == null)
                {
                    newMacro = new InputMacro();
                    context.Macros.Add(newMacro);
                }

                newMacro.MacroName = macros.Key;
                newMacro.MacroValue = macros.Value;
                macroCount++;
            }

            Console.WriteLine($"Completed importing {macroCount} macros!");
            context.SaveChanges();

            /* Memes */
            Console.WriteLine("Begin importing memes...");
            int memeCount = 0;

            foreach (KeyValuePair<string, string> memes in oldBotData.Memes)
            {
                Console.WriteLine($"Importing meme \"{memes.Key}\" to new data.");

                Meme newMeme = context.Memes.FirstOrDefault(m => m.MemeName == memes.Key);
                if (newMeme == null)
                {
                    newMeme = new Meme();
                    context.Memes.Add(newMeme);
                }

                newMeme.MemeName = memes.Key;
                newMeme.MemeValue = memes.Value;
                memeCount++;
            }

            Console.WriteLine($"Completed importing {memeCount} memes!");
            context.SaveChanges();

            /* Users */
            Console.WriteLine("Begin importing user data...");
            int userCount = 0;

            //Migrate users
            foreach (KeyValuePair<string, TRBotDataMigrationTool.User> oldUser in oldBotData.Users)
            {
                string oldUserName = oldUser.Key;
                TRBotDataMigrationTool.User oldUserObj = oldUser.Value;

                Console.WriteLine($"Importing user \"{oldUserName}\" to new data.");

                TRBot.Data.User newUser = DataHelper.GetUserNoOpen(oldUserName, context);
                if (newUser == null)
                {
                    newUser = new TRBot.Data.User(oldUserName);
                    context.Users.Add(newUser);

                    //Save changes here so the navigation properties are applied
                    context.SaveChanges();
                }

                //Migrate the user data
                MigrateUserFromOldToNew(oldUserObj, newUser);

                //Update user abilities
                DataHelper.UpdateUserAutoGrantAbilities(newUser, context);

                userCount++;
            }

            Console.WriteLine($"Completed importing {userCount} users!");

            //Save after migrating all users
            context.SaveChanges();

            /* Game Logs */
            Console.WriteLine("Begin importing game logs...");

            int logCount = 0;

            foreach (TRBotDataMigrationTool.GameLog log in oldBotData.Logs)
            {
                //The DateTime in the old logs were not standardized, so we have to parse them manually
                int separatorLength = DATETIME_AT_PARSE.Length;

                if (string.IsNullOrEmpty(log.DateTimeString) == true)
                {
                    continue;
                }

                int middleIndex = log.DateTimeString.IndexOf(DATETIME_AT_PARSE);
                if (middleIndex < 0)
                {
                    continue;
                }

                //Parse date
                string date = log.DateTimeString.Substring(0, middleIndex + 1);
                int.TryParse(date.Substring(0, 2), out int month);
                int.TryParse(date.Substring(3, 2), out int day);
                int.TryParse(date.Substring(6, 4), out int year);

                int endIndex = middleIndex + separatorLength;

                //Parse time
                string time = log.DateTimeString.Substring(endIndex, log.DateTimeString.Length - endIndex);
                int.TryParse(time.Substring(0, 2), out int hour);
                int.TryParse(time.Substring(3, 2), out int minute);
                int.TryParse(time.Substring(6, 2), out int seconds);

                DateTime newDateTime = new DateTime(year, month, day, hour, minute, seconds);

                TRBot.Data.GameLog newLog = new TRBot.Data.GameLog();
                newLog.LogDateTime = newDateTime;
                newLog.LogMessage = log.LogMessage;
                newLog.User = log.User;

                context.GameLogs.Add(newLog);
                logCount++;
            }

            Console.WriteLine($"Completed importing {logCount} game logs!");

            context.SaveChanges();

            /* Savestate Logs */
            Console.WriteLine("Skipping importing savestate logs, as they don't exist in TRBot 2.0+.");

            /* Silenced Users */
            Console.WriteLine("Skipping importing silenced users, as TRBot 2.0+ has a new ability system.");

            /* Input Callbacks */
            Console.WriteLine("Skipping importing input callbacks, as they don't exist in TRBot 2.0+.");

            /* Input Access */
            Console.WriteLine("Skipping importing InputAccess, as input access is console-specific in TRBot 2.0+.");

            /* Invalid Button Combos */
            Console.WriteLine("Begin importing invalid button combos...");
            int invalidButtonCombos = 0;

            foreach(KeyValuePair<int, List<string>> kvPair in oldBotData.InvalidBtnCombos.InvalidCombos)
            {
                int oldConsoleId = kvPair.Key;
                List<string> invalidCombos = kvPair.Value;

                if (EnumUtility.TryParseEnumValue(oldConsoleId.ToString(), out InputConsoles inputConsole) == true)
                {
                    //Find a console with this name
                    string consoleName = inputConsole.ToString().ToLowerInvariant();

                    GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                    //Couldn't find the console
                    if (console == null)
                    {
                        continue;
                    }

                    List<InvalidCombo> newInvalidCombo = new List<InvalidCombo>();

                    for (int i = 0; i < invalidCombos.Count; i++)
                    {
                        string inputName = invalidCombos[i];

                        //Try to find a valid input containing this name
                        InputData inpData = console.InputList.FirstOrDefault(inp => inp.Name == inputName);
                        if (inpData == null)
                        {
                            continue;
                        }

                        //Check if this button is already in an invalid combo and ignore if so
                        InvalidCombo existing = console.InvalidCombos.FirstOrDefault(ivc => ivc.input_id == inpData.id);
                        if (existing != null)
                        {
                            continue;
                        }

                        //Add the invalid combo
                        console.InvalidCombos.Add(new InvalidCombo(inpData));
                        invalidButtonCombos++;
                    }
                }
            }

            Console.WriteLine($"Completed importing {invalidButtonCombos} invalid button combos!");

            context.SaveChanges();

            /* Input Synonyms */
            Console.WriteLine("Begin importing input synonyms...");
            int importedSynonyms = 0;

            foreach(KeyValuePair<InputConsoles, Dictionary<string, string>> kvPair in oldBotData.InputSynonyms.SynonymDict)
            {
                //Skip if there are none for this console
                if (kvPair.Value.Count == 0)
                {
                    continue;
                }

                string consoleName = kvPair.Key.ToString();
                Dictionary<string, string> synonyms = kvPair.Value;

                GameConsole console = context.Consoles.FirstOrDefault(c => c.Name == consoleName);

                //Couldn't find the console
                if (console == null)
                {
                    continue;
                }

                //Add all synonyms if they don't exist
                foreach (KeyValuePair<string, string> synonymKV in synonyms)
                {
                    string synonymName = synonymKV.Key;
                    InputSynonym synonym = context.InputSynonyms.FirstOrDefault(s => s.SynonymName == synonymName);

                    if (synonym != null)
                    {
                        continue;
                    }

                    InputSynonym newSyn = new InputSynonym(console.id, synonymName, synonymKV.Value);
                    context.InputSynonyms.Add(newSyn);
                }

                importedSynonyms++;
            }

            Console.WriteLine($"Completed importing {importedSynonyms} input synonyms!");

            context.SaveChanges();

            /* Other changes */
            Console.WriteLine("Now importing remaining bot data...");

            AddSettingStrHelper(SettingsConstants.GAME_MESSAGE, oldBotData.GameMessage, context);
            AddSettingStrHelper(SettingsConstants.INFO_MESSAGE, oldBotData.InfoMessage, context);
            AddSettingIntHelper(SettingsConstants.LAST_CONSOLE, oldBotData.LastConsole, context);
            AddSettingIntHelper(SettingsConstants.DEFAULT_INPUT_DURATION, oldBotData.DefaultInputDuration, context);
            AddSettingIntHelper(SettingsConstants.MAX_INPUT_DURATION, oldBotData.MaxInputDuration, context);
            AddSettingIntHelper(SettingsConstants.JOYSTICK_COUNT, oldBotData.JoystickCount, context);
            AddSettingIntHelper(SettingsConstants.LAST_VCONTROLLER_TYPE, oldBotData.LastVControllerType, context);

            AccessLevels.Levels inputPermLvl = (AccessLevels.Levels)oldBotData.InputPermissions;
            long finalPermVal = oldBotData.InputPermissions;
            if (AccessLvlMap.TryGetValue(inputPermLvl, out PermissionLevels permLvl) == true)
            {
                finalPermVal = (long)permLvl;
            }

            AddSettingIntHelper(SettingsConstants.GLOBAL_INPUT_LEVEL, finalPermVal, context);

            context.SaveChanges();

            Console.WriteLine("Finished importing all bot data!");
        }

        private static void AddOldBotSettingsToNewDB(TRBotDataMigrationTool.Settings oldBotSettings)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            /* Client Settings */
            Console.WriteLine("Beginning import of various bot settings...");

            AddSettingIntHelper(SettingsConstants.CLIENT_SERVICE_TYPE, (long)oldBotSettings.ClientSettings.ClientType, context);
            
            /* Message Settings */
            //The original periodic message time is in minutes, so convert it to milliseconds
            AddSettingIntHelper(SettingsConstants.PERIODIC_MSG_TIME, oldBotSettings.MsgSettings.MessageTime * 60L * 1000L, context);
            AddSettingIntHelper(SettingsConstants.MESSAGE_COOLDOWN, (long)oldBotSettings.MsgSettings.MessageCooldown, context);
            AddSettingStrHelper(SettingsConstants.CONNECT_MESSAGE, oldBotSettings.MsgSettings.ConnectMessage, context);
            AddSettingStrHelper(SettingsConstants.RECONNECTED_MESSAGE, oldBotSettings.MsgSettings.ReconnectedMsg, context);
            AddSettingStrHelper(SettingsConstants.PERIODIC_MESSAGE, oldBotSettings.MsgSettings.PeriodicMessage, context);
            AddSettingStrHelper(SettingsConstants.AUTOPROMOTE_MESSAGE, oldBotSettings.MsgSettings.AutoWhitelistMsg, context);
            AddSettingStrHelper(SettingsConstants.NEW_USER_MESSAGE, oldBotSettings.MsgSettings.NewUserMsg, context);
            AddSettingStrHelper(SettingsConstants.BEING_HOSTED_MESSAGE, oldBotSettings.MsgSettings.BeingHostedMsg, context);
            AddSettingStrHelper(SettingsConstants.NEW_SUBSCRIBER_MESSAGE, oldBotSettings.MsgSettings.NewSubscriberMsg, context);
            AddSettingStrHelper(SettingsConstants.RESUBSCRIBER_MESSAGE, oldBotSettings.MsgSettings.ReSubscriberMsg, context);

            /* Bingo Settings */
            AddSettingIntHelper(SettingsConstants.BINGO_ENABLED, (oldBotSettings.BingoSettings.UseBingo == true) ? 1L : 0L, context);
            AddSettingIntHelper(SettingsConstants.BINGO_PIPE_PATH_IS_RELATIVE, 0L, context);
            AddSettingStrHelper(SettingsConstants.BINGO_PIPE_PATH, oldBotSettings.BingoSettings.BingoPipeFilePath, context);

            /* Credits Settings */
            //The original credits time is in minutes, so convert it to milliseconds
            AddSettingIntHelper(SettingsConstants.CREDITS_GIVE_TIME, (long)oldBotSettings.CreditsTime * 60L * 1000L, context);
            AddSettingIntHelper(SettingsConstants.CREDITS_GIVE_AMOUNT, oldBotSettings.CreditsAmount, context);
            AddSettingIntHelper(SettingsConstants.BOT_MSG_CHAR_LIMIT, oldBotSettings.BotMessageCharLimit, context);
            AddSettingIntHelper(SettingsConstants.MAIN_THREAD_SLEEP,  oldBotSettings.MainThreadSleep, context);

            /* Autopromote Settings */
            AddSettingIntHelper(SettingsConstants.AUTO_PROMOTE_ENABLED, (oldBotSettings.AutoWhitelistEnabled == true) ? 1L : 0L, context);
            AddSettingIntHelper(SettingsConstants.AUTO_PROMOTE_INPUT_REQ, oldBotSettings.AutoWhitelistInputCount, context);

            /* Chatbot Settings */
            AddSettingIntHelper(SettingsConstants.CHATBOT_ENABLED, (oldBotSettings.UseChatBot == true) ? 1L : 0L, context);
            AddSettingIntHelper(SettingsConstants.CHATBOT_SOCKET_PATH_IS_RELATIVE, 1L, context);
            AddSettingStrHelper(SettingsConstants.CHATBOT_SOCKET_PATH, oldBotSettings.ChatBotSocketFilename, context);

            context.SaveChanges();

            Console.WriteLine("Finished importing all bot settings!");
        }

        private static void AddSettingIntHelper(string settingKey, long oldVal, BotDBContext context)
        {
            TRBot.Data.Settings setting = DataHelper.GetSettingNoOpen(settingKey, context);
            if (setting == null)
            {
                setting = new TRBot.Data.Settings();
                setting.key = settingKey;
                context.SettingCollection.Add(setting);
            }

            setting.value_int = oldVal;
        }

        private static void AddSettingStrHelper(string settingKey, string oldVal, BotDBContext context)
        {
            TRBot.Data.Settings setting = DataHelper.GetSettingNoOpen(settingKey, context);
            if (setting == null)
            {
                setting = new TRBot.Data.Settings();
                setting.key = settingKey;
                context.SettingCollection.Add(setting);
            }

            setting.value_str = oldVal;
        }

        private static void MigrateUserFromOldToNew(TRBotDataMigrationTool.User oldUserObj, TRBot.Data.User newUser)
        {
            //Convert the old access level to the new one
            if (AccessLvlMap.TryGetValue((AccessLevels.Levels)oldUserObj.Level, out PermissionLevels newLvl) == true)
            {
                newUser.Level = (long)newLvl;
            }
            else
            {
                //Set the level if there's no associated rank
                newUser.Level = oldUserObj.Level;
            }

            newUser.ControllerPort = oldUserObj.Team;
            newUser.SetOptStatus(oldUserObj.OptedOut);
            newUser.Stats.AutoPromoted = (oldUserObj.AutoWhitelisted == true) ? 1L : 0L;
            newUser.Stats.BetCounter = oldUserObj.BetCounter;
            newUser.Stats.Credits = oldUserObj.Credits;
            newUser.Stats.IsSubscriber = oldUserObj.Subscriber;
            newUser.Stats.TotalMessageCount = oldUserObj.TotalMessages;
            newUser.Stats.ValidInputCount = oldUserObj.ValidInputs;
        }
    }
}
