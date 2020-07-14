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
    /// <summary>
    /// Displays the access level of a given command. 
    /// </summary>
    public sealed class CmdAccessLevelCommand : BaseCommand
    {
        private CommandHandler CmdHandler = null;

        public CmdAccessLevelCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            CmdHandler = commandHandler;
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count <= 0 || args.Count > 1)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"command name\"");
                return;
            }

            string cmdName = args[0];

            if (string.IsNullOrEmpty(cmdName) == true)
            {
                BotProgram.MsgHandler.QueueMessage("Usage: \"command name\"");
                return;
            }

            string cmdNameCopy = cmdName;

            //Accept the command name if it starts with the command identifier
            if (cmdName.StartsWith(Globals.CommandIdentifier) == true)
            {
                //Make sure the length is valid
                if (cmdName.Length <= 1)
                {
                    BotProgram.MsgHandler.QueueMessage("Invalid command.");
                    return;
                }

                //Trim the command identifier
                cmdNameCopy = cmdNameCopy.Substring(1, cmdNameCopy.Length - 1);
            }

            if (CmdHandler.CommandDict.TryGetValue(cmdNameCopy, out BaseCommand baseCmd) == false)
            {
                BotProgram.MsgHandler.QueueMessage("Invalid command.");
                return;
            }

            AccessLevels.Levels accessLevel = (AccessLevels.Levels)baseCmd.AccessLevel;

            BotProgram.MsgHandler.QueueMessage($"The permission level of the \"{cmdNameCopy}\" command is {accessLevel}.");
        }
    }
}
