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
    /// Describes an ability the user has.
    /// It's linked to a user and permission ability.
    /// </summary>
    public class UserAbility
    {
        public int id { get; set; } = 0;
        public int user_id { get; set; } = 0;
        public int permability_id { get; set; } = 0;

        public string value_str { get; set; } = string.Empty;
        public int value_int { get; set; } = 0;

        /// <summary>
        /// When the ability expires.
        /// A value of null indicates the ability never expires.
        /// </summary>
        public DateTime? expiration { get; set; } = null;

        /// <summary>
        /// The PermissionAbility this references.
        /// </summary>
        public virtual PermissionAbility PermAbility { get; set; } = null;

        public UserAbility()
        {

        }

        public UserAbility(PermissionAbility permAbility, string valueStr, in int valueInt, in DateTime? expirationDate)
            : this(valueStr, valueInt, expirationDate)
        {
            PermAbility = permAbility;
            permability_id = PermAbility.id;
        }

        public UserAbility(string valueStr, in int valueInt, in DateTime? expirationDate)
        {
            value_str = valueStr;
            value_int = valueInt;
            expiration = expirationDate;
        }
    }
}
