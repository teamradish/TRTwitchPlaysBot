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
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            //See the console
            if (args.Count == 0)
            {
                BotProgram.QueueMessage($"The current console is {InputGlobals.CurrentConsoleVal}. To set the console, add one as an argument: {GetValidConsoleStr()}");
                return;
            }

            string consoleStr = args[0];

            if (Enum.TryParse<InputGlobals.InputConsoles>(consoleStr, true, out InputGlobals.InputConsoles console) == false)
            {
                BotProgram.QueueMessage($"Please enter a valid console: {GetValidConsoleStr()}");
                return;
            }

            if (console == InputGlobals.CurrentConsoleVal)
            {
                BotProgram.QueueMessage($"The current console is already {InputGlobals.CurrentConsoleVal}!");
                return;
            }

            //First stop all inputs completely while changing consoles - we don't want data from other inputs remaining
            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {

            }

            //Set console and buttons
            InputGlobals.SetConsole(console, args);
            VJoyController.Joystick.SetButtons(InputGlobals.CurrentConsoleVal);

            //Resume inputs
            InputHandler.ResumeRunningInputs();

            BotProgram.QueueMessage($"Set console to {InputGlobals.CurrentConsoleVal} and reset all inputs!");
        }

        private string GetValidConsoleStr()
        {
            string[] names = EnumUtility.GetNames<InputGlobals.InputConsoles>.EnumNames;

            if (StrBuilder == null)
            {
                StrBuilder = new StringBuilder(500);
            }

            StrBuilder.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                StrBuilder.Append(names[i]).Append(',').Append(' ');
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            return StrBuilder.ToString();
        }
    }
}
