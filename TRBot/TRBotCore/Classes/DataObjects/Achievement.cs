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

namespace TRBot
{
    /// <summary>
    /// An achievement.
    /// </summary>
    public sealed class Achievement
    {
        /// <summary>
        /// The name of the achievement.
        /// <para>The name should mirror the key in the achievements dictionary.
        /// Avoid spaces to make it easier for viewers to type.</para>
        /// </summary>
        public string Name;

        /// <summary>
        /// The description of the achievement.
        /// </summary>
        public string Description;

        /// <summary>
        /// The type of achievement.
        /// </summary>
        public AchievementTypes AchType;

        /// <summary>
        /// Required value for the achievement.
        /// </summary>
        public long ReqValue;

        /// <summary>
        /// The credit reward for completing the achievement.
        /// </summary>
        public long CreditReward;

        public Achievement(string name, string description, in AchievementTypes achType, in long reqVal, in long creditReward)
        {
            Name = name;
            Description = description;
            AchType = achType;
            ReqValue = reqVal;
            CreditReward = creditReward;
        }
    }
}
