using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Displays how many vJoy controllers are intended to be available.
    /// </summary>
    public sealed class ControllerCountCommand : BaseCommand
    {
        public override void ExecuteCommand(OnChatCommandReceivedArgs e)
        {
            BotProgram.QueueMessage($"There are {BotProgram.BotData.JoystickCount} controller(s) plugged in!");
        }
    }
}
