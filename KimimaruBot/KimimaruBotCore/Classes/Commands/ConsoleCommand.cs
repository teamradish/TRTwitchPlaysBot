using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class ConsoleCommand : BaseCommand
    {
        private StringBuilder StrBuilder = null;

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = 3;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> amount = e.Command.ArgumentsAsList;

            //See the console
            if (amount.Count == 0)
            {
                BotProgram.QueueMessage($"The current console is {InputGlobals.CurrentConsole}. To set the console, add one as an argument: {GetValidConsoleStr()}");
                return;
            }

            if (amount.Count > 1)
            {
                BotProgram.QueueMessage($"Please enter a valid console: {GetValidConsoleStr()}");
                return;
            }

            string consoleStr = amount[0];

            if (Enum.TryParse<InputGlobals.InputConsoles>(consoleStr, true, out InputGlobals.InputConsoles console) == false)
            {
                BotProgram.QueueMessage($"Please enter a valid console: {GetValidConsoleStr()}");
                return;
            }

            if (console == InputGlobals.CurrentConsole)
            {
                BotProgram.QueueMessage($"The current console is already {InputGlobals.CurrentConsole}!");
                return;
            }

            //First stop all inputs completely while changing consoles - we don't want data from other inputs remaining
            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {

            }

            //Set console and buttons
            InputGlobals.CurrentConsole = console;
            VJoyController.Joystick.SetButtons(InputGlobals.CurrentConsole);

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            BotProgram.QueueMessage($"Set console to {InputGlobals.CurrentConsole} and reset all inputs!");
        }

        private string GetValidConsoleStr()
        {
            string[] names = EnumUtility.GetNames<InputGlobals.InputConsoles>.EnumNames;

            if (StrBuilder == null)
            {
                StrBuilder = new StringBuilder(500);
            }

            for (int i = 0; i < names.Length; i++)
            {
                StrBuilder.Append(names[i]).Append(',').Append(' ');
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            return StrBuilder.ToString();
        }
    }
}
