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
using System.IO;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Sets a message that can be displayed on the stream.
    /// </summary>
    public sealed class SetMessageCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Moderator;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            string msg = e.Command.ArgumentsAsString;

            if (msg == null)
            {
                msg = string.Empty;
            }

            BotProgram.BotData.GameMessage = msg;

            //Always save the message to a file so it updates on OBS
            /*For reading from this file on OBS:
              1. Create Text (GDI+)
              2. Check the box labeled "Read from file"
              3. Browse and select the file
             */
            if (Globals.SaveToTextFile(Globals.GameMessageFilename, BotProgram.BotData.GameMessage) == false)
            {
                BotProgram.QueueMessage($"Unable to save message to file");
            }

            BotProgram.SaveBotData();
        }
    }
}
