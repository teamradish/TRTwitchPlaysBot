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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;

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

            using BotDBContext context = DatabaseManager.OpenContext();

            string creditsName = DataHelper.GetCreditsNameNoOpen(context);

            string giverName = args.Command.ChatMessage.Username;
            User giverUser = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

            if (giverUser.HasEnabledAbility(PermissionConstants.TRANSFER_ABILITY) == false)
            {
                QueueMessage($"You do not have the ability to transfer {creditsName}!");
                return;
            }

            if (giverUser.IsOptedOut == true)
            {
                QueueMessage("You cannot transfer while opted out of stats.");
                return;
            }

            string receiverName = arguments[0];
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

            if (transferAmount > giverUser.Stats.Credits)
            {
                QueueMessage($"Transfer amount is greater than {creditsName}!");
                return;
            }

            giverUser.Stats.Credits -= transferAmount;
            receiverUser.Stats.Credits += transferAmount;

            context.SaveChanges();
                
            QueueMessage($"{giverUser.Name} has transferred {transferAmount} {creditsName} to {receiverUser.Name} :D !");
        }
    }
}
