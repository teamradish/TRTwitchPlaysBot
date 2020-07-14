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
    /// A command listing all the valid inputs for the current console.
    /// </summary>
    public sealed class ValidInputsCommand : BaseCommand
    {
        private StringBuilder StrBuilder = new StringBuilder(500);

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> validInputs = new List<string>(InputGlobals.CurrentConsole.ValidInputs);

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);
            Dictionary<string, int> inputAccess = BotProgram.BotData.InputAccess.InputAccessDict;

            //Show the input only if the user has access to use it
            for (int i = validInputs.Count - 1; i >= 0; i--)
            {
                string input = validInputs[i];

                if (inputAccess.TryGetValue(input, out int accessLvl) == false)
                {
                    continue;
                }

                //Check access level
                if (user.Level < accessLvl)
                {
                    validInputs.RemoveAt(i);
                }
            }

            if (validInputs == null || validInputs.Count == 0)
            {
                BotProgram.MsgHandler.QueueMessage($"Interesting! There are no valid inputs you can perform for the {InputGlobals.CurrentConsoleVal} console!");
                return;
            }

            //Add all remaining inputs
            StrBuilder.Clear();

            StrBuilder.Append("Valid inputs for ").Append(InputGlobals.CurrentConsoleVal.ToString()).Append(": ");

            for (int i = 0; i < validInputs.Count; i++)
            {   
                StrBuilder.Append(validInputs[i]).Append(", ");
            }

            //Remove trailing comma
            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            string validInputsStr = StrBuilder.ToString();

            BotProgram.MsgHandler.QueueMessage(validInputsStr);
        }
    }
}
