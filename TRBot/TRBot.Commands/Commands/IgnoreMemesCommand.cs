/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
    /// Has the user see their ignore meme status or change it to ignore/acknowledge memes.
    /// </summary>
    public sealed class IgnoreMemesCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"true or false (optional)\"";
        
        public IgnoreMemesCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string name = args.Command.ChatMessage.Username;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(name, context);

                if (user == null)
                {
                    return;
                }

                //Display opt status
                if (arguments.Count == 0)
                {
                    string message = "You are currently ";

                    if (user.IsIgnoringMemes == true)
                    {
                        message += "ignoring ";
                    }
                    else
                    {
                        message += "acknowledging ";
                    }

                    message += "memes. Enter \"true\" or \"false\" as an argument to change your meme status.";

                    QueueMessage(message);

                    return;
                }
            }

            string ignoreMemeStr = arguments[0];

            if (bool.TryParse(ignoreMemeStr, out bool ignoreMemeStatus) == false)
            {
                QueueMessage("Invalid ignore meme argument.");
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(name, context);

                //Ignoring memes
                if (ignoreMemeStatus == true)
                {
                    if (user.IsIgnoringMemes == true)
                    {
                        QueueMessage("You are already ignoring memes!");
                        return;
                    }

                    QueueMessage("You're now ignoring memes!");
                }
                else
                {
                    if (user.IsIgnoringMemes == false)
                    {
                        QueueMessage("You are already acknowledging memes!");
                        return;
                    }

                    QueueMessage("You're now acknowledging memes!");
                }

                //Set status and save
                user.SetIgnoreMemes(ignoreMemeStatus);
                context.SaveChanges();
            }
        }
    }
}
