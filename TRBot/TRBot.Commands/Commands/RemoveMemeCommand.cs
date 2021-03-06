﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Permissions;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Removes a meme.
    /// </summary>
    public sealed class RemoveMemeCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"memename\"";

        public RemoveMemeCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count < 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string userName = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.REMOVE_MEME_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to remove memes.");
                    return;
                }
            }

            string memeName = args.Command.ArgumentsAsString;
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Meme meme = context.Memes.FirstOrDefault(m => m.MemeName == memeName);
                
                //Remove the meme if found
                if (meme != null)
                {
                    context.Memes.Remove(meme);
                    context.SaveChanges();

                    QueueMessage($"Removed meme \"{memeName}\"!");
                }
            }
        }
    }
}
