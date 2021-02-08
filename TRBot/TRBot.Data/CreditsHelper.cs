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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using TRBot.Utilities;
using TRBot.Logging;

namespace TRBot.Data
{
    /// <summary>
    /// Helps retrieve data regarding user credits.
    /// </summary>
    public static class CreditsHelper
    {
        /// <summary>
        /// Gets a user's position in relation to all others based on credits count.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <returns>An tuple of integers; the first represents the user's position on the credits leaderboard,
        /// and the second represents the number of unique credit counts on the leaderboard.
        /// If the user is not found, the user's position is -1.</returns>
        public static (int, int) GetPositionOnLeaderboard(string userName)
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                //Get the number of scores on the leaderboard
                int leaderBoardCount = context.Users.Select(u => u.Stats.Credits).Count();

                if (user == null)
                {
                    return (-1, leaderBoardCount);
                }

                //Get the position - take all credits greater than or equal to the user's credits
                //Order them in descending order, and get the count of users greater than current count
                int position = context.Users.Where(u => u.Stats.Credits >= user.Stats.Credits).OrderByDescending(u => u.Stats.Credits).Count(u => u.Stats.Credits > user.Stats.Credits);

                //We have how many users have more credits than this one, so add 1 to get the user's actual position
                position += 1;

                return (position, leaderBoardCount);
            }
        }
    }
}