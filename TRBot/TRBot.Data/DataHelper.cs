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
        /// <returns>A Settings object corresponding to <paramref name="settingName">. If the setting is not found, null.</returns>
        public static Settings GetSetting(string settingName)
        {
            using (BotDBContext dbContext = DatabaseManager.OpenContext())
            {
                return dbContext.SettingCollection.FirstOrDefault((set) => set.key == settingName);
            }
        }

        /// <summary>
        /// Obtains a setting integer value from the database.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="defaultVal">The default value to fallback to if not found.</param>
        /// <returns>An integer for <paramref name="settingName">. If the setting is not found, the default value.</returns>
        public static long GetSettingInt(string settingName, in long defaultVal)
        {
            Settings setting = GetSetting(settingName);

            return setting != null ? setting.value_int : defaultVal;
        }

        /// <summary>
        /// Obtains a setting string value from the database.
        /// </summary>        
        /// <param name="settingName">The name of the setting to retrieve.</param>
        /// <param name="defaultVal">The default value to fallback to if not found.</param>
        /// <returns>A string value for <paramref name="settingName">. If the setting is not found, the default value.</returns>
        public static string GetSettingString(string settingName, string defaultVal)
        {
            Settings setting = GetSetting(settingName);

            return setting != null ? setting.value_str : defaultVal;
        }
    }
}
