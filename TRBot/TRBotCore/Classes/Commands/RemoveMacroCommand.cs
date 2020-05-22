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
    /// Removes a macro that exists.
    /// </summary>
    public sealed class RemoveMacroCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}removemacro usage: \"#macroname\"");
                return;
            }

            string macroName = args[0].ToLowerInvariant();
            
            if (BotProgram.BotData.Macros.ContainsKey(macroName) == true)
            {
                char macroFirstChar = macroName[1];

                //Remove from the parser macro list
                List<string> parserMacroList = BotProgram.BotData.ParserMacroLookup[macroFirstChar];
                parserMacroList.Remove(macroName);
                if (parserMacroList.Count == 0)
                {
                    BotProgram.BotData.ParserMacroLookup.Remove(macroFirstChar);
                }

                BotProgram.BotData.Macros.TryRemove(macroName, out string removedMacro);
                BotProgram.SaveBotData();

                BotProgram.QueueMessage($"Removed macro {macroName}.");
            }
            else
            {
                BotProgram.QueueMessage($"Macro \"{macroName}\" could not be found.");
            }
        }
    }
}
