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

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
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
                BotProgram.BotData.Macros.Remove(macroName);
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
