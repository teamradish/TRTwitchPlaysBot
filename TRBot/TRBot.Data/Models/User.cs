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
        public virtual List<PermissionAbility> PermAbilities { get; set; } = null;

        public User()
        {
            if (Stats == null)
            {
                Stats = new UserStats();
            }

            if (PermAbilities == null)
            {
                PermAbilities = new List<PermissionAbility>();
            }
        }

        public bool HasPermission(string permName)
        {
            PermissionAbility ability = PermAbilities.FirstOrDefault(p => p.Name == permName);

            return (ability != null);
        }
    }
}
