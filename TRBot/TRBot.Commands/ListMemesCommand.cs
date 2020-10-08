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
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.ParserData;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Lists all available memes.
    /// </summary>
    public sealed class ListMemesCommand : BaseCommand
    {
        public ListMemesCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            using BotDBContext context = DatabaseManager.OpenContext();

            int memeCount = context.Memes.Count();

            if (memeCount == 0)
            {
                QueueMessage("There are no memes!");
                return;
            }

            //The capacity is the estimated average number of characters for each meme multiplied by the number of memes
            StringBuilder strBuilder = new StringBuilder(memeCount * 20);

            foreach (Meme meme in context.Memes)
            {
                strBuilder.Append(meme.MemeName).Append(',').Append(' ');
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            int maxCharCount = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.BOT_MSG_CHAR_LIMIT, context, 500L);
            
            QueueMessageSplit(strBuilder.ToString(), maxCharCount, ", ");
        }
    }
}
