using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Displays how many vJoy controllers are intended to be available.
    /// </summary>
    public sealed class ControllerCountCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage($"There are {VJoyController.Joysticks.Length} controller(s) plugged in!");
        }
    }
}
