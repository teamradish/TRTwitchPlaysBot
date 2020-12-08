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
    /// High fives others. Often used after accomplishing a task.
    /// </summary>
    public class HighFiveCommand : BaseCommand
    {
        public HighFiveCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments == null || arguments.Count <= 0)
            {
                QueueMessage("Choose one or more people to high five!");
                return;
            }

            StringBuilder stringBuilder = new StringBuilder(128);

            stringBuilder.Append(args.Command.ChatMessage.Username).Append(" high fives ");

            for (int i = 0; i < arguments.Count; i++)
            {
                int nextInd = i + 1;

                if (i > 0)
                {
                    stringBuilder.Append(',').Append(' ');
                    if (nextInd == arguments.Count)
                    {
                        stringBuilder.Append("and ");
                    }
                }

                stringBuilder.Append(arguments[i]);
            }

            stringBuilder.Append('!');

            int botCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            QueueMessageSplit(stringBuilder.ToString(), botCharLimit, ", ");
        }
    }
}
