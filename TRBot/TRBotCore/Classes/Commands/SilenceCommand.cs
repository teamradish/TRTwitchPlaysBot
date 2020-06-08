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
    /// Silences a user, preventing them from performing inputs.
    /// </summary>
    public sealed class SilenceCommand : BaseCommand
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

            string silencedName = args[0].ToLowerInvariant();
            string selfName = e.Command.ChatMessage.Username.ToLowerInvariant();
            if (silencedName == selfName)
            {
                BotProgram.MsgHandler.QueueMessage("No use in silencing yourself, silly!");
                return;
            }

            User user = BotProgram.GetUser(silencedName);

            if (user == null)
            {
                BotProgram.MsgHandler.QueueMessage($"User does not exist in database!");
                return;
            }

            if (user.Silenced == true)
            {
                BotProgram.MsgHandler.QueueMessage($"User {silencedName} is already silenced!");
                return;
            }

            //Make sure the user you're silencing is a lower level than you
            User selfUser = BotProgram.GetUser(selfName);
            if (selfUser.Level <= user.Level)
            {
                BotProgram.MsgHandler.QueueMessage($"Cannot silence a user at or above your access level!");
                return;
            }

            user.SetSilenced(true);
            BotProgram.BotData.SilencedUsers.Add(silencedName);
            BotProgram.SaveBotData();

            BotProgram.MsgHandler.QueueMessage($"User {silencedName} has been silenced and thus prevented from performing inputs.");
        }
    }
}
