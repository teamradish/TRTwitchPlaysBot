using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;
using System.Diagnostics;

namespace TRBot
{
    /// <summary>
    /// Saves a savestate.
    /// </summary>
    public sealed class SavestateCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
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
            if (InputGlobals.CurrentConsole.ButtonInputMap.ContainsKey(saveStateStr) == false)
            {
                BotProgram.QueueMessage("Invalid state number.");
                return;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            //Check if the user can perform this input
            ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissions(user.Level, saveStateStr, BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            if (inputValidation.IsValid == false)
            {
                if (string.IsNullOrEmpty(inputValidation.Message) == false)
                {
                    BotProgram.QueueMessage(inputValidation.Message);
                }

                return;
            }

            //Savestates are always performed on the first controller
            IVirtualController joystick = InputGlobals.ControllerMngr.GetController(0);
            joystick.PressButton(InputGlobals.CurrentConsole.ButtonInputMap[saveStateStr]);
            joystick.UpdateController();

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

            joystick.ReleaseButton(InputGlobals.CurrentConsole.ButtonInputMap[saveStateStr]);
            joystick.UpdateController();
        }
    }
}
