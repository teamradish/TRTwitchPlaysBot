using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Stops all ongoing input commands.
    /// </summary>
    public sealed class StopAllCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {
                
            }

            //Reset the controller
            VJoyController.Joystick.Reset();

            InputHandler.ResumeRunningInputs();

            BotProgram.QueueMessage("Stopped all running inputs!");
        }
    }
}
