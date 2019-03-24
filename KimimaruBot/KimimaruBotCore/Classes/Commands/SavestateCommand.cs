using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;
using System.Diagnostics;

namespace KimimaruBot
{
    /// <summary>
    /// Saves a savestate.
    /// </summary>
    public sealed class SavestateCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = 2;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"{Globals.CommandIdentifier}savestate usage: #");
                return;
            }

            string stateNumStr = args[0];

            if (int.TryParse(stateNumStr, out int stateNum) == false)
            {
                BotProgram.QueueMessage($"Invalid savestate number.");
                return;
            }

            string saveStateStr = $"savestate{stateNum}";
            if (InputGlobals.InputMap.ContainsKey(saveStateStr) == false)
            {
                BotProgram.QueueMessage($"Invalid savestate number.");
                return;
            }

            VJoyController.Joystick.PressButton(saveStateStr);
            VJoyController.Joystick.UpdateJoystickEfficient();

            //Wait a bit before releasing the input
            const float wait = 50f;
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < wait)
            {

            }

            VJoyController.Joystick.ReleaseButton(saveStateStr);
            VJoyController.Joystick.UpdateJoystickEfficient();
        }
    }
}
