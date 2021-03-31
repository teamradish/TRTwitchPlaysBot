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
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Views a user's credit count.
    /// </summary>
    public sealed class CreditsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username (optional)\"";

        public CreditsCommand()
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

            string creditsUsername = (arguments.Count == 1) ? arguments[0].ToLowerInvariant() : args.Command.ChatMessage.Username.ToLowerInvariant();
            long creditsCount = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User creditsUser = DataHelper.GetUserNoOpen(creditsUsername, context);

                if (creditsUser == null)
                {
                    QueueMessage($"User does not exist in database!");
                    return;
                }

                creditsCount = creditsUser.Stats.Credits;
            }

            string creditsName = DataHelper.GetCreditsName();

            (int, int) leaderBoard = CreditsHelper.GetPositionOnLeaderboard(creditsUsername);

            QueueMessage($"{creditsUsername} has {creditsCount} {creditsName.Pluralize(false, creditsCount)} and is rank {leaderBoard.Item1}/{leaderBoard.Item2} on the leaderboard!");
        }
    }
}
