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
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class TransferCommand : BaseCommand
    {
        public TransferCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args == null || args.Count != 2)
            {
                BotProgram.QueueMessage("Please specify a single user followed by the amount of credits you wish to transfer!");
                return;
            }

            string giver = e.Command.ChatMessage.DisplayName;
            string giverToLower = giver.ToLower();

            string receiver = args[0];
            string receiverToLower = receiver.ToLower();

            if (giverToLower == receiverToLower)
            {
                BotProgram.QueueMessage("You cannot transfer points to yourself!");
                return;
            }

            //If the user transferring points isn't in the database, add them
            User giverUser = BotProgram.GetOrAddUser(giverToLower);
            User receiverUser = BotProgram.GetUser(receiverToLower);

            if (receiverUser == null)
            {
                BotProgram.QueueMessage($"{receiver} is not in the database!");
                return;
            }

            long transferAmount = -1L;
            bool success = long.TryParse(args[1], out transferAmount);
            if (success == false || transferAmount <= 0)
            {
                BotProgram.QueueMessage("Please specify a positive whole number of credits greater than 0!");
                return;
            }

            if (giverUser.Credits < transferAmount)
            {
                BotProgram.QueueMessage("The transfer amount is greater than your credits!");
                return;
            }

            giverUser.Credits -= transferAmount;
            receiverUser.Credits += transferAmount;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"{giver} has transferred {transferAmount} points to {receiver} :D !");
        }
    }
}
