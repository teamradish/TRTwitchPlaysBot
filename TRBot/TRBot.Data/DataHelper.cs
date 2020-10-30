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
using System.Linq;
using TRBot.Permissions;
using TRBot.Connection;

namespace TRBot.Data
{
    /// <summary>
    /// Helps retrieve data from the database.
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// Obtains a setting from the database.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <returns>A Settings object corresponding to settingName. If the setting is not found, null.</returns>
        public static Settings GetSetting(string settingName)
        {
            using (BotDBContext dbContext = DatabaseManager.OpenContext())
            {
                return dbContext.SettingCollection.FirstOrDefault((set) => set.key == settingName);
            }
        }

        /// <summary>
        /// Obtains a setting from the database with an opened context.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="context">The open database context.</param>
        /// <returns>A Settings object corresponding to settingName. If the setting is not found, null.</returns>
        public static Settings GetSettingNoOpen(string settingName, BotDBContext context)
        {
            return context.SettingCollection.FirstOrDefault((set) => set.key == settingName);
        }

        /// <summary>
        /// Obtains a setting integer value from the database.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="defaultVal">The default value to fallback to if not found.</param>
        /// <returns>An integer for settingName. If the setting is not found, the default value.</returns>
        public static long GetSettingInt(string settingName, in long defaultVal)
        {
            Settings setting = GetSetting(settingName);

            return setting != null ? setting.value_int : defaultVal;
        }

        /// <summary>
        /// Obtains a setting integer value from the database with an opened context.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="context">The open database context.</param>
        /// <param name="defaultVal">The default value to fallback to if not found.</param>
        /// <returns>An integer for settingName. If the setting is not found, the default value.</returns>
        public static long GetSettingIntNoOpen(string settingName, BotDBContext context, in long defaultVal)
        {
            Settings setting = GetSettingNoOpen(settingName, context);

            return setting != null ? setting.value_int : defaultVal;
        }

        /// <summary>
        /// Obtains a setting string value from the database.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="defaultVal">The default value to fallback to if not found.</param>
        /// <returns>A string value for settingName. If the setting is not found, the default value.</returns>
        public static string GetSettingString(string settingName, string defaultVal)
        {
            Settings setting = GetSetting(settingName);

            return setting != null ? setting.value_str : defaultVal;
        }

        /// <summary>
        /// Obtains a setting string value from the database with an opened context.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="context">The open database context.</param>
        /// <param name="defaultVal">The default value to fallback to if not found.</param>
        /// <returns>A string value for settingName. If the setting is not found, the default value.</returns>
        public static string GetSettingStringNoOpen(string settingName, BotDBContext context, string defaultVal)
        {
            Settings setting = GetSettingNoOpen(settingName, context);

            return setting != null ? setting.value_str : defaultVal;
        }

        /// <summary>
        /// A helper method to obtain the name of the bot credits.
        /// </summary>        
        /// <returns>The name of the bot credits.</returns>
        public static string GetCreditsName()
        {
            return GetSettingString(SettingsConstants.CREDITS_NAME, "Credits");
        }

        /// <summary>
        /// A helper method to obtain the name of the bot credits with an opened context.
        /// </summary>
        /// <param name="context">The open database context.</param>
        /// <returns>The name of the bot credits.</returns>
        public static string GetCreditsNameNoOpen(BotDBContext context)
        {
            return GetSettingStringNoOpen(SettingsConstants.CREDITS_NAME, context, "Credits");
        }

        /// <summary>
        /// A helper method to obtain the current client service type.
        /// </summary>
        /// <returns>The current ClientServiceType.</returns>
        public static ClientServiceTypes GetClientServiceType()
        {
            return (ClientServiceTypes)GetSettingInt(SettingsConstants.CLIENT_SERVICE_TYPE, 0L);
        }

        /// <summary>
        /// A helper method to obtain the current client service type with an opened context.
        /// </summary>
        /// <param name="context">The open database context.</param>
        /// <returns>The current ClientServiceType.</returns>
        public static ClientServiceTypes GetClientServiceTypeNoOpen(BotDBContext context)
        {
            return (ClientServiceTypes)GetSettingInt(SettingsConstants.CLIENT_SERVICE_TYPE, 0L);
        }

        /// <summary>
        /// Obtains a user object from the database.
        /// </summary>        
        /// <param name="userName">The name of the user.</param>
        /// <returns>A user object with the given userName. null if not found.</returns>
        public static User GetUser(string userName)
        {
            using BotDBContext dBContext = DatabaseManager.OpenContext();

            return GetUserNoOpen(userName, dBContext);
        }

        /// <summary>
        /// Obtains a user object from the database with an opened context.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="context">The open database context.</param>
        /// <returns>A user object with the given userName. null if not found.</returns>
        public static User GetUserNoOpen(string userName, BotDBContext context)
        {
            string userNameLowered = userName.ToLowerInvariant();

            return context.Users.FirstOrDefault(u => u.Name == userNameLowered);
        }

        /// <summary>
        /// Obtains a user object from the database.
        //  If it doesn't exist, a new one will be added to the database.
        /// </summary>        
        /// <param name="userName">The name of the user.</param>
        /// <param name="added">Whether a new user was added to the database.</param>
        /// <returns>A user object with the given userName.</returns>
        public static User GetOrAddUser(string userName, out bool added)
        {
            using BotDBContext dbContext = DatabaseManager.OpenContext();

            return GetOrAddUserNoOpen(userName, dbContext, out added);
        }

        /// <summary>
        /// Obtains a user object from the database with an opened context.
        //  If it doesn't exist, a new one will be added to the database.
        /// </summary>        
        /// <param name="userName">The name of the user.</param>
        /// <param name="context">The open database context.</param>
        /// <param name="added">Whether a new user was added to the database.</param>
        /// <returns>A user object with the given userName.</returns>
        public static User GetOrAddUserNoOpen(string userName, BotDBContext context, out bool added)
        {
            //Add the lowered version of their name to simplify retrieval
            string userNameLowered = userName.ToLowerInvariant();

            User user = context.Users.FirstOrDefault(u => u.Name == userNameLowered);
            
            added = false;

            //If the user doesn't exist, add it
            if (user == null)
            {
                //Give them User permissions
                user = new User(userNameLowered, (long)PermissionLevels.User);
                context.Users.Add(user);

                //Save the changes so the user object is in the database
                context.SaveChanges();

                //Update this user's abilities off the bat
                UpdateUserAutoGrantAbilities(user, context);

                //Save changes again to update the abilities
                context.SaveChanges();

                added = true;
            }

            return user;
        }

        /// <summary>
        /// Retrieves a user's overridden default input duration, or if they don't have one, the global default input duration.
        /// </summary>
        /// <param name="user">The User object.</param>
        /// <param name="context">The open database context</param>
        /// <returns>The user-overridden or global default input duration.</returns>
        public static long GetUserOrGlobalDefaultInputDur(User user, BotDBContext context)
        {
            //Check for a user-overridden default input duration
            if (user != null && user.TryGetAbility(PermissionConstants.USER_DEFAULT_INPUT_DIR_ABILITY, out UserAbility defaultDurAbility) == true)
            {
                return defaultDurAbility.value_int;
            }
            //Use global max input duration
            else
            {
                return DataHelper.GetSettingIntNoOpen(SettingsConstants.DEFAULT_INPUT_DURATION, context, 200L);
            }
        }

        /// <summary>
        /// Retrieves a user's overridden max input duration, or if they don't have one, the global max input duration.
        /// </summary>
        /// <param name="user">The User object.</param>
        /// <param name="context">The open database context</param>
        /// <returns>The user-overridden or global max input duration.</returns>
        public static long GetUserOrGlobalMaxInputDur(User user, BotDBContext context)
        {
            //Check for a user-overridden max input duration
            if (user != null && user.TryGetAbility(PermissionConstants.USER_MAX_INPUT_DIR_ABILITY, out UserAbility maxDurAbility) == true)
            {
                return maxDurAbility.value_int;
            }
            //Use global max input duration
            else
            {
                return DataHelper.GetSettingIntNoOpen(SettingsConstants.MAX_INPUT_DURATION, context, 60000L);
            }
        }

        /// <summary>
        /// Fully updates a user's available abilities based on their current level.
        /// </summary>
        /// <param name="user">The User object to update the abilities on.</param>
        /// <param name="newLevel">The new level the user will be set to.</param>
        /// <param name="context">The open database context.</param>
        public static void UpdateUserAutoGrantAbilities(User user, BotDBContext context)
        {
            long originalLevel = user.Level;

            //First, remove all auto grant abilities the user has
            //Don't remove abilities that were given by a higher level
            //This prevents users from removing constraints imposed by moderators and such
            int removed = user.UserAbilities.RemoveAll(p => (long)p.PermAbility.AutoGrantOnLevel >= 0
                    && p.GrantedByLevel <= originalLevel);

            Console.WriteLine($"Removed {removed} abilities!");

            //Get all auto grant abilities up to the user's level
            IEnumerable<PermissionAbility> permAbilities =
                context.PermAbilities.Where(p => (long)p.AutoGrantOnLevel >= 0
                    && (long)p.AutoGrantOnLevel <= originalLevel);

            Console.WriteLine($"Found {permAbilities.Count()} autogrant up to level {originalLevel}");

            //Add all of those abilities
            foreach (PermissionAbility permAbility in permAbilities)
            {
                bool added = user.TryAddAbility(permAbility);
                //if (added == true)
                //{
                //    Console.WriteLine($"Added ability {permAbility.Name} to {user.Name}. New count: {user.UserAbilities.Count}");
                //}
                //else
                //{
                //    Console.WriteLine($"Didn't add ability {permAbility.Name} to {user.Name} because it's already present. Count: {user.UserAbilities.Count}");
                //}
            }
        }

        /// <summary>
        /// Adds or removes user abilities upon changing the user's level.
        /// </summary>
        /// <param name="user">The User object to adjust the abilities on.</param>
        /// <param name="newLevel">The new level the user will be set to.</param>
        /// <param name="context">The open database context.</param>
        public static void AdjustUserAbilitiesOnLevel(User user, long newLevel, BotDBContext context)
        {
            long originalLevel = user.Level;

            //Nothing to do here if the levels are the same
            if (originalLevel == newLevel)
            {
                return;
            }

            //Remove all abilities down to the new level
            if (originalLevel > newLevel)
            {
                //Look for all auto grant abilities that are less than or equal to the original level
                //and greater than the new level, and remove them
                int removed = user.UserAbilities.RemoveAll(p => p.PermAbility.AutoGrantOnLevel >= 0
                    && (long)p.PermAbility.AutoGrantOnLevel <= originalLevel
                    && (long)p.PermAbility.AutoGrantOnLevel > newLevel);
            }
            //Add all abilities up to the new level
            else if (originalLevel < newLevel)
            {
                //Look for all auto grant abilities that are greater than the original level
                //and less than or equal to the new level
                IEnumerable<PermissionAbility> permAbilities =
                    context.PermAbilities.Where(p => (long)p.AutoGrantOnLevel >= 0
                        && (long)p.AutoGrantOnLevel > originalLevel && (long)p.AutoGrantOnLevel <= newLevel);

                //Add all these abilities
                foreach (PermissionAbility pAbility in permAbilities)
                {
                    user.TryAddAbility(pAbility);
                }
            }
        }
    }
}
