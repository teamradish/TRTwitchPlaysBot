using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Shows the contents of a macro, provided it exists.
    /// </summary>
    public sealed class ShowCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}show usage: \"macroname\"");
                return;
            }

            string macroName = args[0].ToLowerInvariant();

            if (BotProgram.BotData.Macros.TryGetValue(macroName, out string macroVal) == false)
            {
                BotProgram.QueueMessage($"{macroName} not found.");
                return;
            }

            BotProgram.QueueMessage($"{macroName} = {macroVal}");
        }
    }
}
