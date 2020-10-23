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
using Newtonsoft.Json;
using TRBot.Permissions;
using System.Linq;

namespace TRBot.Data
{
    /// <summary>
    /// Represents a user object.
    /// </summary>
    public class User
    {
        public int id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public long Level { get; set; } = 0;
        public long ControllerPort { get; set; } = 0;

        /// <summary>
        /// User stats.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual UserStats Stats { get; set; } = null;

        /// <summary>
        /// The permissions available to the user.
        /// This is used by the database and should not be assigned manually.
        /// </summary>
        public virtual List<UserAbility> UserAbilities { get; set; } = null;

        /// <summary>
        /// The inputs the user is restricted from performing.
        /// This is used by the database and should not be assigned manually.
        /// </summary>
        public virtual List<RestrictedInput> RestrictedInputs { get; set; } = null;

        public bool IsOptedOut => (Stats.OptedOut != 0);

        public User()
        {
            
        }

        public User(string userName)
            : this()
        {
            Name = userName;

            Stats = new UserStats();
            UserAbilities = new List<UserAbility>();
            RestrictedInputs = new List<RestrictedInput>();
        }

        public User(string userName, in long level)
            : this(userName)
        {
            Level = level;
        }
        
        /// <summary>
        /// Adds a user ability if it doesn't already exist.
        /// </summary>
        /// <param name="permAbility">The PermissionAbility.</param>
        public bool TryAddAbility(PermissionAbility permAbility)
        {
            if (permAbility == null)
            {
                return false;
            }

            //Check if the ability exists
            UserAbility curAbility = UserAbilities.FirstOrDefault(p => p.PermAbility?.Name == permAbility.Name);

            //Add the ability
            if (curAbility == null)
            {
                UserAbility newAbility = new UserAbility(permAbility, permAbility.value_str, permAbility.value_int, -1, null);
                UserAbilities.Add(newAbility);

                return true;
            }

            return true;
        }

        /// <summary>
        /// Adds a user ability if it doesn't already exist and updates one if it does.
        /// </summary>
        /// <param name="permAbility">The PermissionAbility.</param>
        /// <param name="valueStr">The string value of the ability.</param>
        /// <param name="valueInt">The integer value of the ability.</param>
        /// <param name="grantedByLevel">The level of the user that granted this ability.</param>
        /// <param name="expirationDate">The date when the ability expires.</param>
        public bool AddAbility(PermissionAbility permAbility, string valueStr, in int valueInt,
            in long grantedByLevel, in DateTime? expirationDate)
        {
            if (permAbility == null)
            {
                return false;
            }

            //Check if the ability exists
            UserAbility curAbility = UserAbilities.FirstOrDefault(p => p.PermAbility?.Name == permAbility.Name);

            //Add the ability
            if (curAbility == null)
            {
                UserAbility newAbility = new UserAbility(permAbility, valueStr, valueInt, grantedByLevel, expirationDate);
                UserAbilities.Add(newAbility);
            }
            //Update the ability
            else
            {
                curAbility.value_str = valueStr;
                curAbility.value_int = valueInt;
                curAbility.GrantedByLevel = grantedByLevel;
                curAbility.expiration = expirationDate;
            }

            return true;
        }

        /// <summary>
        /// Removes a user ability with a given name.
        /// </summary>
        /// <param name="abilityName">The name of the ability to remove.</param>
        public bool RemoveAbility(string abilityName)
        {
            //Find the ability
            UserAbility ability = UserAbilities.FirstOrDefault(p => p.PermAbility.Name == abilityName);

            if (ability != null)
            {
                UserAbilities.Remove(ability);
                return true;
            }

            return false;
        }

        public bool HasAbility(string abilityName)
        {
            return TryGetAbility(abilityName, out UserAbility ability);
        }

        public bool TryGetAbility(string abilityName, out UserAbility userAbility)
        {
            DateTime now = DateTime.UtcNow;
            
            //Check for name and expiration
            userAbility = UserAbilities.FirstOrDefault(p => (p.PermAbility?.Name == abilityName)
                && (p.expiration == null || (p.expiration != null && p.expiration > now)));

            return (userAbility != null);
        }

        /// <summary>
        /// Gets all restricted inputs.
        /// </summary>
        public Dictionary<string, int> GetRestrictedInputs()
        {
            DateTime now = DateTime.UtcNow;
            IEnumerable<RestrictedInput> restrictedInputs = RestrictedInputs.Where(r => r.expiration == null || r.expiration > now);

            Dictionary<string, int> restrictedInpDict = new Dictionary<string, int>();

            //Put all restricted inputs in a dictionary
            foreach (RestrictedInput rInput in restrictedInputs)
            {
                if (restrictedInpDict.TryGetValue(rInput.input_name, out int val) == false)
                {
                    restrictedInpDict.Add(rInput.input_name, 1);
                    continue;
                }

                val += 1;
                restrictedInpDict[rInput.input_name] = val;
            }

            return restrictedInpDict;
        }
    }
}
