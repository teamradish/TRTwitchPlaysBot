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
using TRBot.Consoles;

namespace TRBot.Data
{
    /// <summary>
    /// Describes a restricted input for a user.
    /// </summary>
    public class RestrictedInput
    {
        public int ID { get; set; } = 0;

        /// <summary>
        /// The ID of the user the restricted input belongs to.
        /// </summary>
        public int UserID { get; set; } = 0;

        /// <summary>
        /// The ID of the input the user is restricted from inputting.
        /// </summary>
        public int InputID { get; set; } = 0;

        /// <summary>
        /// When the restriction expires.
        /// A value of null indicates the restriction never expires.
        /// </summary>
        public DateTime? Expiration { get; set; } = null;

        /// <summary>
        /// The User associated with the restricted input.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual User user { get; set; } = null;

        /// <summary>
        /// The InputData associated with the restricted input.
        /// This is used by the database and should not be assigned or modified manually.
        /// </summary>
        public virtual InputData inputData { get; set; } = null;

        /// <summary>
        /// Tells if this restricted input has expired.
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
        /// Tells if this restricted input has an expiration date.
        /// </summary>
        public bool HasExpiration => Expiration.HasValue;

        public RestrictedInput()
        {

        }

        public RestrictedInput(int userID, int inputID, DateTime? expirationDate)
        {
            UserID = userID;
            InputID = inputID;
            Expiration = expirationDate;
        }
    }
}
