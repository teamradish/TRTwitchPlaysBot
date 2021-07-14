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
using System.Linq;
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Views the highest valid input count among users.
    /// </summary>
    public sealed class InputLeaderboardCommand : BaseCommand
    {
        private const int SURROUNDING_COUNT = 19;
        private string UsageMessage = "Usage: \"username (optional)\"";

        public InputLeaderboardCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = string.Empty;

            //Find the top user in leaderboard
            if (arguments.Count == 0)
            {
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User topUser = context.Users.Where(u => u.Stats.OptedOut <= 0).OrderByDescending(u => u.Stats.ValidInputCount).FirstOrDefault();
                    
                    //The only way this user can be null is if there are none in the database
                    if (topUser == null)
                    {
                        QueueMessage("Either there are no users in the database or everyone is opted out of bot stats.");
                        return;
                    }

                    userName = topUser.Name;
                }
            }
            //Verify the user exists and is opted into stats
            else
            {
                userName = arguments[0];

                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User user = DataHelper.GetUserNoOpen(userName, context);

                    //User not found
                    if (user == null)
                    {
                        QueueMessage($"User \"{userName}\" not found in the database.");
                        return;
                    }

                    //This user is opted out
                    if (user.IsOptedOut == true)
                    {
                        QueueMessage("This user is opted out of bot stats, so they aren't on the leaderboard.");
                        return;
                    }
                }
            }

            SortedDictionary<int, (long, string)> leaderboardDict = LeaderboardHelper.GetInputLeaderboardSubset(userName, SURROUNDING_COUNT);

            if (leaderboardDict == null || leaderboardDict.Count == 0)
            {
                QueueMessage("There are no users opted into the database.");
                return;
            }

            StringBuilder strBuilder = new StringBuilder((SURROUNDING_COUNT + 1) * 8);

            foreach (KeyValuePair<int, (long, string)> kvPair in leaderboardDict)
            {
                strBuilder.Append(kvPair.Key + 1).Append('.').Append(' ');

                strBuilder.Append(kvPair.Value.Item2).Append(' ').Append('(').Append(kvPair.Value.Item1).Append(')');

                strBuilder.Append(' ');
            }

            strBuilder.Remove(strBuilder.Length - 1, 1);

            int botCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(strBuilder.ToString(), botCharLimit, ") ");
        }
    }
}
