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
using TRBot.Utilities;

namespace TRBot.Permissions
{
    /// <summary>
    /// Helper methods relating to permissions.
    /// </summary>
    public static class PermissionHelpers
    {
        /// <summary>
        /// Converts the string representation of a PermissionLevel to its PermissionLevel equivalent.
        /// This fails if a valid permission level isn't found.
        /// </summary>
        /// <param name="levelStr">The string to convert.</param>
        /// <param name="permLevel">The PermissionLevel returned. Defaults to <see cref="PermissionLevels.User" /> if not found.</param>
        /// <returns>A bool indicating if the conversion was successful.</returns>
        public static bool TryParsePermissionLevel(string levelStr, out PermissionLevels permLevel)
        {
            //Try to parse into a number
            if (long.TryParse(levelStr, out long levelNum) == false)
            {
                //Failed to parse into a number, so check if the string passed in is a valid name
                if (Enum.TryParse<PermissionLevels>(levelStr, true, out PermissionLevels result) == false)
                {
                    permLevel = PermissionLevels.User;
                    return false;
                }
                
                permLevel = result;
                return true;
            }

            //Check if the given number matches any of these values
            PermissionLevels[] levelArray = EnumUtility.GetValues<PermissionLevels>.EnumValues;

            for (int i = 0; i < levelArray.Length; i++)
            {
                long lvlVal = (long)levelArray[i];

                //Check if the values match
                if (levelNum == lvlVal)
                {
                    permLevel = levelArray[i];
                    return true;
                }
            }

            permLevel = PermissionLevels.User;
            return false;
        }

        /// <summary>
        /// Tells if a given value is a valid permission level.
        /// </summary>
        /// <param name="permissionVal">The integer representation of the permission value.</param>
        /// <returns>true if the given value matches one of the available permissions, otherwise false.</returns>
        public static bool IsValidPermissionValue(in long permissionVal)
        {
            //Check if the given number matches any of these values
            PermissionLevels[] levelArray = EnumUtility.GetValues<PermissionLevels>.EnumValues;

            for (int i = 0; i < levelArray.Length; i++)
            {
                long lvlVal = (long)levelArray[i];

                //Check if the values match
                if (permissionVal == lvlVal)
                {
                    return true;
                }
            }

            return true;
        }
    }
}
