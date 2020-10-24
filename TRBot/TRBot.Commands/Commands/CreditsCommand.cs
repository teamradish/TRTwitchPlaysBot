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
using TRBot.Connection;
using TRBot.Data;
using TRBot.Permissions;

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

            using BotDBContext context = DatabaseManager.OpenContext();

            string creditsUsername = (arguments.Count == 1) ? arguments[0].ToLowerInvariant() : args.Command.ChatMessage.Username.ToLowerInvariant();
            User creditsUser = DataHelper.GetUserNoOpen(creditsUsername, context);

            if (creditsUser == null)
            {
                QueueMessage($"User does not exist in database!");
                return;
            }

            string creditsName = DataHelper.GetSettingStringNoOpen(SettingsConstants.CREDITS_NAME, context, "Credits");

            QueueMessage($"{creditsUsername} has {creditsUser.Stats.Credits} {creditsName}!");
        }
    }
}
