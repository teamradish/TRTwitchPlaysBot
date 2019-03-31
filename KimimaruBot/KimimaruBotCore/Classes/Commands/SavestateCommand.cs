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
            if (InputGlobals.InputMap.ContainsKey(saveStateStr) == false)
            {
                BotProgram.QueueMessage("Invalid state number.");
                return;
            }

            VJoyController.Joystick.PressButton(saveStateStr);
            VJoyController.Joystick.UpdateJoystickEfficient();

            //Track the time of the savestate
            DateTime curTime = DateTime.UtcNow;

            //Add a new savestate log
            GameLog newStateLog = new GameLog();
            newStateLog.User = e.Command.ChatMessage.Username;

            string date = curTime.ToShortDateString();
            string time = curTime.ToLongTimeString();
            newStateLog.DateTimeString = $"{date} at {time}";

            newStateLog.User = e.Command.ChatMessage.Username;
            newStateLog.LogMessage = string.Empty;

            //Add the message if one was specified
            if (args.Count > 1)
            {
                string message = e.Command.ArgumentsAsString.Remove(0, stateNumStr.Length + 1);
                newStateLog.LogMessage = message;
            }

            //Add or replace the log and save the bot data
            BotProgram.BotData.SavestateLogs[stateNum] = newStateLog;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Saved state {stateNum}!");

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
