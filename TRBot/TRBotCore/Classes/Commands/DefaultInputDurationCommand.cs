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
    /// A command that prints out the default input duration and allows setting it for certain access levels.
    /// </summary>
    public class DefaultInputDurationCommand : BaseCommand
    {
        private int SetAccessLevel = (int)AccessLevels.Levels.Admin;

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count == 0)
            {
                BotProgram.QueueMessage($"The default duration of an input is {BotProgram.BotData.DefaultInputDuration} milliseconds!");
                return;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            //Disallow setting the duration if the user doesn't have a sufficient access level
            if (user.Level < SetAccessLevel)
            {
                BotProgram.QueueMessage(CommandHandler.INVALID_ACCESS_MESSAGE);
                return;
            }

            if (args.Count > 1)
            {
                BotProgram.QueueMessage($"Usage: \"duration (ms)\"");
                return;
            }

            if (int.TryParse(args[0], out int newDefaultDur) == false)
            {
                BotProgram.QueueMessage("Please enter a valid number!");
                return;
            }

            if (newDefaultDur < 0)
            {
                BotProgram.QueueMessage("Cannot set a negative duration!");
                return;
            }

            if (newDefaultDur == BotProgram.BotData.DefaultInputDuration)
            {
                BotProgram.QueueMessage("The duration is already this value!");
                return;
            }

            BotProgram.BotData.DefaultInputDuration = newDefaultDur;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Set default input duration to {newDefaultDur} milliseconds!");
        }
    }
}
