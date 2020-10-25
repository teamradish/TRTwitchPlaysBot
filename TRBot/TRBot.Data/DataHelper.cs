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
        /// Fully updates a user's available abilities based on their current level.
        /// </summary>
        /// <param name="user">The User object to update the abilities on.</param>
        /// <param name="newLevel">The new level the user will be set to.</param>
        /// <param name="context">The open database context.</param>
        public static void UpdateUserAutoGrantAbilities(User user, BotDBContext context)
        {
            //First, remove all auto grant abilities the user has
            UserAbility[] userAbilities = user.UserAbilities.ToArray();
            for (int i = 0; i < userAbilities.Length; i++)
            {
                UserAbility userAbility = userAbilities[i];

                if (userAbility == null)
                {
                    Console.WriteLine($"User ability at {i} is somehow null! UserID: {user.id}");
                    continue;
                }

                PermissionAbility pAbility = userAbility.PermAbility;

                if (pAbility == null)
                {
                    Console.WriteLine($"User linked permission ability at {i} is somehow null! PermID: {userAbility.permability_id} | UserID: {userAbility.user_id}");
                    continue;
                }

                //Don't remove abilities that were given by a higher level
                //This prevents users from removing constraints imposed by moderators and such
                if ((long)pAbility.AutoGrantOnLevel >= 0
                    && userAbility.GrantedByLevel <= user.Level)
                {
                    user.RemoveAbility(pAbility.Name);
                }
            }

            long originalLevel = user.Level;

            //Get all auto grant abilities up to the user's level
            IEnumerable<PermissionAbility> permAbilities =
                context.PermAbilities.Where(p => (long)p.AutoGrantOnLevel >= 0
                    && (long)p.AutoGrantOnLevel <= originalLevel);

            //Add all of those abilities
            foreach (PermissionAbility permAbility in permAbilities)
            {
                if (user.TryAddAbility(permAbility) == true)
                {
                    //Save after adding each one to avoid issues with null navigation properties while searching
                    //NOTE: We will want to find a way to change this to not save inside this method
                    context.SaveChanges();
                }
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
                //and greater than the new level
                IEnumerable<PermissionAbility> permAbilities =
                    context.PermAbilities.Where(p => (long)p.AutoGrantOnLevel >= 0
                        && (long)p.AutoGrantOnLevel <= originalLevel && (long)p.AutoGrantOnLevel > newLevel);
            
                //Remove all these abilities
                foreach (PermissionAbility pAbility in permAbilities)
                {
                    //Don't remove abilities granted by a higher level
                    if (user.TryGetAbility(pAbility.Name, out UserAbility ability) == true
                        && ability.GrantedByLevel <= user.Level)
                    {
                        user.RemoveAbility(pAbility.Name);
                    }
                }
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
                    if (user.TryAddAbility(pAbility) == true)
                    {
                        //Save after adding each one to avoid issues with null navigation properties while searching
                        //NOTE: We will want to find a way to change this to not save inside this method
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
