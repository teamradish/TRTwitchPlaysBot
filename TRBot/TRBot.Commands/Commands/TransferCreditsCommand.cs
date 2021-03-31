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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Allows a user to transfer credits to another user.
    /// </summary>
    public class TransferCreditsCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"username (string)\" \"transfer amount (int)\"";

        public TransferCreditsCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 2)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string creditsName = DataHelper.GetCreditsName();
            string giverName = args.Command.ChatMessage.Username.ToLowerInvariant();
            long giverCredits = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User giverUser = DataHelper.GetUserNoOpen(giverName, context);
    
                if (giverUser.HasEnabledAbility(PermissionConstants.TRANSFER_ABILITY) == false)
                {
                    QueueMessage($"You do not have the ability to transfer !");
                    return;
                }
    
                if (giverUser.IsOptedOut == true)
                {
                    QueueMessage("You cannot transfer while opted out of stats.");
                    return;
                }

                giverCredits = giverUser.Stats.Credits;
            }

            string receiverName = arguments[0].ToLowerInvariant();
            if (giverName == receiverName)
            {
                QueueMessage($"You cannot transfer {creditsName.Pluralize(false, 0)} to yourself!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User receiverUser = DataHelper.GetUserNoOpen(receiverName, context);

                if (receiverUser == null)
                {
                    QueueMessage("This user does not exist in the database!");
                    return;
                }

                if (receiverUser.IsOptedOut == true)
                {
                    QueueMessage("This user is opted out of stats, so you can't transfer to them!");
                    return;
                }
            }

            string transferAmountStr = arguments[1];

            if (long.TryParse(transferAmountStr, out long transferAmount) == false)
            {
                QueueMessage("Please enter a valid transfer amount!");
                return;
            }

            if (transferAmount <= 0)
            {
                QueueMessage("Transfer amount must be greater than 0!");
                return;
            }

            if (transferAmount > giverCredits)
            {
                QueueMessage($"Transfer amount is greater than {creditsName.Pluralize(false, 0)}!");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User giverUser = DataHelper.GetUserNoOpen(giverName, context);
                User receiverUser = DataHelper.GetUserNoOpen(receiverName, context);

                //Transfer credits and save
                giverUser.Stats.Credits -= transferAmount;
                receiverUser.Stats.Credits += transferAmount;

                context.SaveChanges();
            }

            QueueMessage($"{giverName} has transferred {transferAmount} {creditsName.Pluralize(false, transferAmount)} to {receiverName} :D !");
        }
    }
}
