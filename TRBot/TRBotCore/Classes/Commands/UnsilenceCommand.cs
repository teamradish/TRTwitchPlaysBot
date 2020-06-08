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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Unsilences a user, allowing them to perform inputs again.
    /// </summary>
    public sealed class UnsilenceCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"username\"");
                return;
            }

            string unsilencedName = args[0].ToLowerInvariant();
            string selfName = e.Command.ChatMessage.Username.ToLowerInvariant();
            if (unsilencedName == selfName)
            {
                BotProgram.MsgHandler.QueueMessage("Nice try.");
                return;
            }

            User user = BotProgram.GetUser(unsilencedName);

            if (user == null)
            {
                BotProgram.MsgHandler.QueueMessage($"User does not exist in database!");
                return;
            }

            if (user.Silenced == false)
            {
                BotProgram.MsgHandler.QueueMessage($"User {unsilencedName} is not silenced!");
                return;
            }

            //Make sure the user you're silencing is a lower level than you
            User selfUser = BotProgram.GetUser(selfName);
            if (selfUser.Level <= user.Level)
            {
                BotProgram.MsgHandler.QueueMessage($"Cannot unsilence a user at or above your access level!");
                return;
            }
            
            user.SetSilenced(false);
            BotProgram.BotData.SilencedUsers.Remove(unsilencedName);
            BotProgram.SaveBotData();

            BotProgram.MsgHandler.QueueMessage($"User {unsilencedName} has been unsilenced and can perform inputs once again.");
        }
    }
}
