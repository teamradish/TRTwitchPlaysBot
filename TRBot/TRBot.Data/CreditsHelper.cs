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
        /// If the user is not found, the returned user position is -1.</returns>
        public static (int, int) GetPositionOnLeaderboard(string userName)
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                //Get the number of scores on the leaderboard
                int leaderBoardCount = context.Users.Where(u => u.Stats.OptedOut <= 0).Count();

                if (user == null)
                {
                    return (-1, leaderBoardCount);
                }

                //Get the position - take all credits greater than or equal to the user's credits
                //Order them in descending order, and get the count of users greater than current count
                //Don't include opted out users
                int position = context.Users.Where(u => u.Stats.Credits >= user.Stats.Credits && u.Stats.OptedOut <= 0).OrderByDescending(u => u.Stats.Credits).Count(u => u.Stats.Credits > user.Stats.Credits);

                //We have how many users have more credits than this one, so add 1 to get the user's actual position
                position += 1;

                return (position, leaderBoardCount);
            }
        }

        /// <summary>
        /// Gets a subset of the credits leaderboard around a given username.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="surroundingCount">The total number of entries to obtain around the given username.
        /// This will be spread out as evenly as possible.
        /// <para>For example, if this is 10, it will attempt to obtain 5 above and 5 below the user's position;
        /// however, if the user is at the top of the leaderboard, it will obtain 10 below the user's position.</para>
        /// </param>
        /// <returns>A SortedDictionary with integer keys and tuple values.
        /// The key is the zero-based position in the leaderboard.
        /// The tuple's first value is a long representing the number of credits the user has.
        /// The second value is a string containing the user's name.
        /// If the user is not found, the returned SortedDictionary is null.
        /// </returns>
        public static SortedDictionary<int, (long, string)> GetLeaderboardSubset(string userName, in int surroundingCount)
        {
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User namedUser = DataHelper.GetUserNoOpen(userName, context);

                if (namedUser == null)
                {
                    return null;
                }

                //Get the position - take all credits greater than or equal to the user's credits
                //Order them in descending order, and get the count of users greater than current count
                //Don't include opted out users, but always include the user we're looking for the subsets around
                List<User> allUsers = context.Users.Where(u => u.Stats.OptedOut <= 0).OrderByDescending(u => u.Stats.Credits).ToList();

                int userIndex = allUsers.IndexOf(namedUser);
                
                //Somehow, the user isn't in this list
                if (userIndex < 0)
                {
                    return null;
                }

                SortedDictionary<int, (long, string)> leaderboardDict = new SortedDictionary<int, (long, string)>();

                //Add users with credit values greater
                //Keep going until either the top of the list or we found half of the surrounding value
                for (int i = userIndex - 1, foundCount = 0; i >= 0 && foundCount < (surroundingCount / 2); i--, foundCount++)
                {
                    User user = allUsers[i];
                    leaderboardDict[i] = (user.Stats.Credits, user.Name);
                }
                
                //Add the user
                leaderboardDict[userIndex] = (namedUser.Stats.Credits, namedUser.Name);

                //Add users with credit values lower
                //Keep going until either the end of the list or we found the rest of the surrounding value
                for (int i = userIndex + 1, foundCount = leaderboardDict.Count - 1; i < allUsers.Count && foundCount < surroundingCount; i++, foundCount++)
                {
                    User user = allUsers[i];
                    leaderboardDict[i] = (user.Stats.Credits, user.Name);
                }

                return leaderboardDict;
            }
        }
    }
}