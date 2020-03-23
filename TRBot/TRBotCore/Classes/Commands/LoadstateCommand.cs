using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class LoadstateCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
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
            if (InputGlobals.CurrentConsole.ButtonInputMap.ContainsKey(loadStateStr) == false)
            {
                BotProgram.QueueMessage($"Invalid state number.");
                return;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            //Check if the user can perform this input
            ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissions(user.Level, loadStateStr, BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            if (inputValidation.IsValid == false)
            {
                if (string.IsNullOrEmpty(inputValidation.Message) == false)
                {
                    BotProgram.QueueMessage(inputValidation.Message);
                }

                return;
            }

            //Load states are always performed on the first controller
            IVirtualController joystick = BotProgram.ControllerMngr.GetController(0);
            joystick.PressButton(loadStateStr);
            joystick.UpdateController();

            BotProgram.QueueMessage($"Loaded state {stateNum}!");

            //Wait a bit before releasing the input
            const float wait = 50f;
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < wait)
            {

            }

            joystick.ReleaseButton(loadStateStr);
            joystick.UpdateController();
        }
    }
}
