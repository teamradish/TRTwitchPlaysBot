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
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;

namespace TRBot.Data
{
    /// <summary>
    /// A collection of users.
    /// </summary>
    public class UserCollection
    {
        private ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

        public UserCollection()
        {
            
        }

        /// <summary>
        /// Adds a user to the collection.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        public void AddUser(string userName)
        {
            //Don't add if the user exists
            if (Users.TryGetValue(userName, out User user) == true)
            {
                return;
            }

            //Add the user
            user = new User();
            user.Name = userName;
            Users[userName] = user;
        }

        /// <summary>
        /// Overwrites a user object at a given key with another.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="userData">The user data to overwrite with.</param>
        public void OverwriteUser(string userName, User userData)
        {
            Users[userName] = userData;
        }

        /// <summary>
        /// Removes a user from the collection.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        public void RemoveUser(string userName)
        {
            Users.TryRemove(userName, out User user);
        }

        /// <summary>
        /// Retrieves a user from the collection.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A User object correlating to the given username if it exists, otherwise null.</returns>
        public User GetUser(string userName)
        {
            Users.TryGetValue(userName, out User user);
            
            return user;
        }

        /// <summary>
        /// Retrieves a user from the collection, and if not found, adds the user to the collection.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A User object correlating to the given username.</returns>
        public User GetOrAddUser(string userName)
        {
            AddUser(userName);
            return GetUser(userName);
        }
    }
}
