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
    /// A command that prints out the max input duration and allows setting it for certain access levels.
    /// </summary>
    public class MaxInputDurationCommand : BaseCommand
    {
        private int SetAccessLevel = (int)AccessLevels.Levels.Moderator;

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count == 0)
            {
                BotProgram.MsgHandler.QueueMessage($"The max duration of an input sequence is {BotProgram.BotData.MaxInputDuration} milliseconds!");
                return;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            //Disallow setting the duration if the user doesn't have a sufficient access level
            if (user.Level < SetAccessLevel)
            {
                BotProgram.MsgHandler.QueueMessage(CommandHandler.INVALID_ACCESS_MESSAGE);
                return;
            }

            if (args.Count > 1)
            {
                BotProgram.MsgHandler.QueueMessage($"Usage: \"duration (ms)\"");
                return;
            }

            if (int.TryParse(args[0], out int newMaxDur) == false)
            {
                BotProgram.MsgHandler.QueueMessage("Please enter a valid number!");
                return;
            }

            if (newMaxDur < 0)
            {
                BotProgram.MsgHandler.QueueMessage("Cannot set a negative duration!");
                return;
            }

            if (newMaxDur == BotProgram.BotData.MaxInputDuration)
            {
                BotProgram.MsgHandler.QueueMessage("The duration is already this value!");
                return;
            }

            BotProgram.BotData.MaxInputDuration = newMaxDur;
            BotProgram.SaveBotData();

            BotProgram.MsgHandler.QueueMessage($"Set max input sequence duration to {newMaxDur} milliseconds!");
        }
    }
}
