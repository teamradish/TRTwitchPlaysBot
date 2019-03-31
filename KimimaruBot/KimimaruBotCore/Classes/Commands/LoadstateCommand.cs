using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class LoadstateCommand : BaseCommand
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
                BotProgram.QueueMessage("Usage: state #");
                return;
            }

            string stateNumStr = args[0];

            if (int.TryParse(stateNumStr, out int stateNum) == false)
            {
                BotProgram.QueueMessage($"Invalid state number.");
                return;
            }

            string loadStateStr = $"loadstate{stateNum}";
            if (InputGlobals.InputMap.ContainsKey(loadStateStr) == false)
            {
                BotProgram.QueueMessage($"Invalid state number.");
                return;
            }

            VJoyController.Joystick.PressButton(loadStateStr);
            VJoyController.Joystick.UpdateJoystickEfficient();

            BotProgram.QueueMessage($"Loaded state {stateNum}!");

            //Wait a bit before releasing the input
            const float wait = 50f;
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < wait)
            {

            }

            VJoyController.Joystick.ReleaseButton(loadStateStr);
            VJoyController.Joystick.UpdateJoystickEfficient();
        }
    }
}
