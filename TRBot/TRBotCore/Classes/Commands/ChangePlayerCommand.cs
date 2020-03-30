using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Changes which player, or controller port, a user is on.
    /// </summary>
    public sealed class ChangePlayerCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count == 0 || args.Count > 1)
            {
                BotProgram.QueueMessage("Usage: \"controllerPort (starting from 1)\"");
                return;
            }

            if (int.TryParse(args[0], out int portNum) == false)
            {
                BotProgram.QueueMessage("That is not a valid number!");
                return;
            }

            if (portNum <= 0 || portNum > InputGlobals.ControllerMngr.ControllerCount)
            {
                BotProgram.QueueMessage($"Please specify a number in the range of 1 through the current controller count ({InputGlobals.ControllerMngr.ControllerCount}).");
                return;
            }

            //Change to zero-based index for referencing
            int controllerNum = portNum - 1;

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            if (user.Team == controllerNum)
            {
                BotProgram.QueueMessage("You're already on this controller port!");
                return;
            }

            //Change team and save data
            user.Team = controllerNum;

            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Changed controller port to {portNum}!");
        }
    }
}
