/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Permissions;

namespace TRBot.Data
{
    /// <summary>
    /// Describes an ability the user has.
    /// It's linked to a user and permission ability.
    /// </summary>
    public class UserAbility
    {
        public int ID { get; set; } = 0;
        public int UserID { get; set; } = 0;
        public int PermabilityID { get; set; } = 0;

        /// <summary>
        /// The enabled state of the ability.
        /// </summary>
        public int Enabled { get; set; } = 1;

        public string ValueStr { get; set; } = string.Empty;
        public int ValueInt { get; set; } = 0;

        /// <summary>
        /// Describes the level of the user that assigned this ability.
        /// A value of -1 means no user assigned this ability.
        /// </summary>
        public long GrantedByLevel { get; set; } = -1;

        /// <summary>
        /// When the ability's current state expires.
        /// A value of null indicates the ability's current state never expires.
        /// </summary>
        public DateTime? Expiration { get; set; } = null;

        /// <summary>
        /// The User associated with the ability.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual User user { get; set; } = null;

        /// <summary>
        /// The PermissionAbility this references.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual PermissionAbility PermAbility { get; set; } = null;

        /// <summary>
        /// Tells if this user ability is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                //Abilities default to enabled
                //If the ability is disabled, check if it has expired
                if (Enabled == 0)
                {
                    return HasExpired == true;
                }

                //Ensure the ability hasn't expired
                return HasExpired == false;
            }
        }

        /// <summary>
        /// Tells if this user ability has expired.
        /// </summary>
        public bool HasExpired
        {
            get
            {
                //It hasn't expired if the expiration date is null
                if (Expiration.HasValue == false)
                {
                    return false;
                }

                //Compare dates
                return Expiration.Value < DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Tells if this user ability has an expiration date.
        /// </summary>
        public bool HasExpiration => Expiration.HasValue;

        public UserAbility()
        {

        }

        public UserAbility(PermissionAbility permAbility, in bool enabledState, string valueStr, in int valueInt,
            in long grantedByLevel, in DateTime? expirationDate)
        {
            PermabilityID = permAbility.ID;

            SetEnabledState(enabledState);
            ValueStr = valueStr;
            ValueInt = valueInt;
            GrantedByLevel = grantedByLevel;
            Expiration = expirationDate;
        }

        public void SetEnabledState(in bool enabledState)
        {
            Enabled = (enabledState == false) ? 0 : 1;
        }
    }
}
