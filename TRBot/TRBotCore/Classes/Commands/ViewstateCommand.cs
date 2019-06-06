using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Views a savestate's log.
    /// </summary>
    public class ViewstateCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count < 1)
            {
                BotProgram.QueueMessage("Usage: state #");
                return;
            }

            string stateNumStr = args[0];

            if (int.TryParse(stateNumStr, out int stateNum) == false)
            {
                BotProgram.QueueMessage("Invalid state number.");
                return;
            }

            string saveStateStr = $"savestate{stateNum}";
            if (InputGlobals.CurrentConsole.ButtonInputMap.ContainsKey(saveStateStr) == false)
            {
                BotProgram.QueueMessage("Invalid state number.");
                return;
            }

            //Get the state log
            GameLog stateLog = null;

            if (BotProgram.BotData.SavestateLogs.TryGetValue(stateNum, out stateLog) == false)
            {
                BotProgram.QueueMessage("This state has not yet been saved!");
                return;
            }

            string message = string.Empty;

            //Show the log message if one was input
            if (string.IsNullOrEmpty(stateLog.LogMessage) == false)
            {
                message = $"State {stateNum}, \"{stateLog.LogMessage}\", saved by {stateLog.User} on {stateLog.DateTimeString} (UTC)";
            }
            else
            {
                message = $"State {stateNum}, saved by {stateLog.User} on {stateLog.DateTimeString} (UTC)";
            }

            BotProgram.QueueMessage(message);
        }
    }
}
