/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
        public int ID { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public long Level { get; set; } = 0;
        public long ControllerPort { get; set; } = 0;

        /// <summary>
        /// User stats.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual UserStats Stats { get; set; }

        /// <summary>
        /// The permissions available to the user.
        /// This is used by the database and should not be assigned manually.
        /// </summary>
        public virtual List<UserAbility> UserAbilities { get; set; }

        /// <summary>
        /// The inputs the user is restricted from performing.
        /// This is used by the database and should not be assigned manually.
        /// </summary>
        public virtual List<RestrictedInput> RestrictedInputs { get; set; }

        /// <summary>
        /// Recent valid inputs the user made.
        /// This is used by the database and should not be assigned manually. 
        /// </summary>
        public virtual List<RecentInput> RecentInputs { get; set; }

        /// <summary>
        /// Tells if the user is opted out of stats.
        /// </summary>
        public bool IsOptedOut => (Stats.OptedOut > 0);

        /// <summary>
        /// Tells if the user is ignoring memes.
        /// </summary>
        public bool IsIgnoringMemes => (Stats.IgnoreMemes > 0);

        /// <summary>
        /// Tells if the user is opted into simulate data.
        /// </summary>
        public bool IsOptedIntoSimulate => (Stats.OptedInSimulate > 0);

        public User()
        {
            
        }

        public User(string userName)
        {
            Name = userName;

            Stats = new UserStats();
            UserAbilities = new List<UserAbility>();
            RestrictedInputs = new List<RestrictedInput>();
            RecentInputs = new List<RecentInput>();
        }

        public User(string userName, in long level)
            : this(userName)
        {
            Level = level;
        }
        
        /// <summary>
        /// Sets the user's status on being opted out of bot stats.
        /// </summary>
        /// <param name="optIntoStats">Whether to opt into or out of bot stats.</param>
        public void SetOptStatus(in bool optIntoStats)
        {
            Stats.OptedOut = (optIntoStats == true) ? 0L : 1L;
        }

        /// <summary>
        /// Sets the user's status on ignoring memes.
        /// </summary>
        /// <param name="ignoreMemes">Whether to ignore memes.</param>
        public void SetIgnoreMemes(in bool ignoreMemes)
        {
            Stats.IgnoreMemes = (ignoreMemes == true) ? 1L : 0L;
        }

        /// <summary>
        /// Sets the user's status on being opted into simulate data.
        /// </summary>
        /// <param name="optIntoSimulate">Whether to opt into or out of simulate data.</param>
        public void SetOptSimulate(in bool optIntoSimulate)
        {
            Stats.OptedInSimulate = (optIntoSimulate == true) ? 1L : 0L;
        }

        /// <summary>
        /// Enables a user ability and adds one if it doesn't already exist.
        /// </summary>
        /// <param name="permAbility">The PermissionAbility.</param>
        /// <returns>true if the ability was enabled, otherwise false.</returns>
        public bool EnableAbility(PermissionAbility permAbility)
        {
            //Check if the ability exists
            UserAbility curAbility = UserAbilities.FirstOrDefault(p => p.PermabilityID == permAbility.ID);

            //Add the ability
            if (curAbility == null)
            {
                //Console.WriteLine($"Ability {permAbility.Name} not found, adding to {Name}");

                UserAbility newAbility = new UserAbility(permAbility, true, permAbility.ValueStr, permAbility.ValueInt, -1, null);
                newAbility.UserID = ID;
                UserAbilities.Add(newAbility);

                //Console.WriteLine($"New ability user id: {newAbility.user_id}");

                return true;
            }

            curAbility.SetEnabledState(true);
            curAbility.Expiration = null;
            curAbility.GrantedByLevel = -1;

            //Console.WriteLine($"Ability {permAbility.Name} already found on {Name}, not adding");

            return false;
        }

        /// <summary>
        /// Tells if this user can add a given PermissionAbility to another user. 
        /// </summary>
        /// <param name="permAbility">The PermissionAbility.</param>
        /// <param name="otherUser">The user to add the PermissionAbility to.</param>
        /// <returns>true if this user can add the PermissionAbility to the given user, otherwise false.
        public bool CanAddAbilityToUser(PermissionAbility permAbility, User otherUser)
        {
            //No ability or other user
            if (permAbility == null || otherUser == null)
            {
                return false;
            }

            //Equal level with the other user (if not self) - can't grant
            if (ID != otherUser.ID && Level <= otherUser.Level)
            {
                return false;
            }

            //Can't grant this ability as it's not available to your level
            if ((long)permAbility.AutoGrantOnLevel > Level)
            {
                return false;
            }

            //Min level to grant is higher than level
            if ((long)permAbility.MinLevelToGrant > Level)
            {
                return false;
            }

            //Get the user ability for additional checks
            if (otherUser.TryGetAbility(permAbility.ID, out UserAbility otherUserAbility) == true)
            {
                //Can't grant this ability since it was granted by someone higher-leveled
                if (otherUserAbility.GrantedByLevel > Level)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a user ability if it doesn't already exist and updates one if it does.
        /// </summary>
        /// <param name="permAbility">The PermissionAbility.</param>
        /// <param name="enabledState">The enabled state of the ability.</param>
        /// <param name="valueStr">The string value of the ability.</param>
        /// <param name="valueInt">The integer value of the ability.</param>
        /// <param name="grantedByLevel">The level of the user that granted this ability.</param>
        /// <param name="expirationDate">The date when the ability expires.</param>
        /// <returns>true if the ability was added or updated, otherwise false.</returns>
        public bool AddAbility(PermissionAbility permAbility, in bool enabledState, string valueStr, in int valueInt,
            in long grantedByLevel, in DateTime? expirationDate)
        {
            //Check if the ability exists
            UserAbility curAbility = UserAbilities.FirstOrDefault(p => p.PermabilityID == permAbility.ID);

            //Add the ability
            if (curAbility == null)
            {
                UserAbility newAbility = new UserAbility(permAbility, enabledState, valueStr, valueInt, grantedByLevel, expirationDate);
                newAbility.UserID = ID;
                UserAbilities.Add(newAbility);
            }
            //Update the ability
            else
            {
                curAbility.SetEnabledState(enabledState);
                curAbility.ValueStr = valueStr;
                curAbility.ValueInt = valueInt;
                curAbility.GrantedByLevel = grantedByLevel;
                curAbility.Expiration = expirationDate;
            }

            return true;
        }

        /// <summary>
        /// Disables a user ability with a given name.
        /// </summary>
        /// <param name="abilityName">The name of the ability to disable.</param>
        /// <returns>true if the ability was disabled, otherwise false.</returns>
        public bool DisableAbility(int permAbilityID)
        {
            //Find the ability
            UserAbility ability = UserAbilities.FirstOrDefault(p => p.PermabilityID == permAbilityID);

            if (ability != null)
            {
                ability.SetEnabledState(false);
                ability.Expiration = null;
                return true;
            }

            return false;
        }

        public bool HasAbility(in int permAbilityID)
        {
            return TryGetAbility(permAbilityID, out UserAbility ability);
        }

        public bool HasAbility(string abilityName)
        {
            return TryGetAbility(abilityName, out UserAbility ability);
        }

        public bool HasEnabledAbility(int permAbilityID)
        {
            TryGetAbility(permAbilityID, out UserAbility ability);

            if (ability == null)
            {
                return false;
            }

            return ability.IsEnabled;
        }

        public bool HasEnabledAbility(string abilityName)
        {
            TryGetAbility(abilityName, out UserAbility ability);

            if (ability == null)
            {
                return false;
            }

            return ability.IsEnabled;
        }

        public bool TryGetAbility(string abilityName, out UserAbility userAbility)
        {
            userAbility = UserAbilities.FirstOrDefault(p => p.PermAbility.Name == abilityName);

            return (userAbility != null);
        }

        public bool TryGetAbility(int permAbilityID, out UserAbility userAbility)
        {
            userAbility = UserAbilities.FirstOrDefault(p => p.PermabilityID == permAbilityID);

            return (userAbility != null);
        }

        /// <summary>
        /// Gets all restricted inputs.
        /// </summary>
        public Dictionary<string, int> GetRestrictedInputs()
        {
            DateTime now = DateTime.UtcNow;
            IEnumerable<RestrictedInput> restrictedInputs = RestrictedInputs.Where(r => r.HasExpired == false);

            Dictionary<string, int> restrictedInpDict = new Dictionary<string, int>();

            //Put all restricted inputs in a dictionary
            foreach (RestrictedInput rInput in restrictedInputs)
            {
                string inputName = rInput.inputData.Name;

                //Search by name
                if (restrictedInpDict.TryGetValue(inputName, out int val) == false)
                {
                    restrictedInpDict.Add(inputName, 1);
                    continue;
                }

                val += 1;
                restrictedInpDict[inputName] = val;
            }

            return restrictedInpDict;
        }
    }
}
