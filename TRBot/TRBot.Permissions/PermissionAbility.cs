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
using System.Collections.Concurrent;

namespace TRBot.Permissions
{
    /// <summary>
    /// Describes an available permission ability.
    /// </summary>
    public class PermissionAbility
    {
        public int id { get; set; } = 0;

        /// <summary>
        /// Which permission level to automatically grant the permission on.
        /// A value less than 0 means the permission is not granted automatically.
        /// </summary>
        public PermissionLevels AutoGrantOnLevel { get; set; } = (PermissionLevels)(-1);

        public string Name { get; set; } = string.Empty;

        public string value_str { get; set; } = string.Empty;

        public int value_int { get; set; } = 0;

        public PermissionAbility()
        {

        }

        public PermissionAbility(string name)
        {
            Name = name;
        }

        public PermissionAbility(string name, in PermissionLevels autoGrantOnLevel)
            : this(name)
        {
            AutoGrantOnLevel = autoGrantOnLevel;
        }

        public PermissionAbility(string name, string valueStr, in int valueInt)
            : this(name)
        {
            Name = name;
            value_str = valueStr;
            value_int = valueInt;
        }

        public PermissionAbility(string name, in PermissionLevels autoGrantOnLevel, string valueStr, in int valueInt)
            : this(name, valueStr, valueInt)
        {
            AutoGrantOnLevel = autoGrantOnLevel;
        }
    }
}
