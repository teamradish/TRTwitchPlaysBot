using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Stops all ongoing input commands.
    /// </summary>
    public sealed class StopAllCommand : BaseCommand
    {
        public StopAllCommand()
        {
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {
                
            }

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            BotProgram.QueueMessage("Stopped all running inputs!");
        }
    }
}
