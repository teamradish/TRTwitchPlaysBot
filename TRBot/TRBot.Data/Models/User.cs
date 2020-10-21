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
        
        public bool TryGetAbility(string abilityName, out UserAbility userAbility)
        {
            DateTime now = DateTime.UtcNow;
            
            //Check for name and expiration
            userAbility = UserAbilities.FirstOrDefault(p => p.PermAbility.Name == abilityName && p.expiration != null && p.expiration > now);

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
