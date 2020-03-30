using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Displays or changes the virtual controller used.
    /// </summary>
    public sealed class VirtualControllerCommand : BaseCommand
    {
        private StringBuilder StrBuilder = null;
        
        public VirtualControllerCommand()
        {
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            //See the virtual controller
            if (args.Count == 0)
            {
                BotProgram.QueueMessage($"The current virtual controller is {InputGlobals.CurVControllerType}. To set the virtual controller, add one as an argument: {GetValidControllerStr()}");
                return;
            }

            string vControllerStr = args[0];

            if (Enum.TryParse<InputGlobals.VControllerTypes>(vControllerStr, true, out InputGlobals.VControllerTypes vCType) == false)
            {
                BotProgram.QueueMessage($"Please enter a valid virtual controller: {GetValidControllerStr()}");
                return;
            }

            if (vCType == InputGlobals.CurVControllerType)
            {
                BotProgram.QueueMessage($"The current virtual controller is already {InputGlobals.CurVControllerType}!");
                return;
            }
            
            if (InputGlobals.IsVControllerSupported(vCType) == false)
            {
                BotProgram.QueueMessage($"{vCType} virtual controllers are not supported on your operating system.");
                return;
            }
            
            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {
                
            }
            
            //Change virtual controller
            InputGlobals.SetVirtualController(vCType);
            
            //Resume inputs
            InputHandler.ResumeRunningInputs();
            
            BotProgram.QueueMessage($"Set virtual controller to {InputGlobals.CurVControllerType} and reset all running inputs!");
        }
        
        private string GetValidControllerStr()
        {
            string[] names = EnumUtility.GetNames<InputGlobals.VControllerTypes>.EnumNames;

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
