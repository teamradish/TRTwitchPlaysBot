using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Sets a message that can be displayed on the stream.
    /// </summary>
    public sealed class SetMessageCommand : BaseCommand
    {
        public const string MessageFile = "GameMessage.txt";

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);

            AccessLevel = (int)AccessLevels.Levels.Whitelisted;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string msg = e.Command.ArgumentsAsString;

            if (msg == null)
            {
                msg = string.Empty;
            }

            BotProgram.BotData.GameMessage = msg;

            //Always save the message to a file so it updates on OBS
            /*For reading from this file on OBS:
              1. Create Text (GDI+)
              2. Check the box labeled "Read from file"
              3. Browse and select the file
             */
            if (Globals.SaveToTextFile(MessageFile, BotProgram.BotData.GameMessage) == false)
            {
                BotProgram.QueueMessage($"Unable to save message to file");
            }

            BotProgram.SaveBotData();
        }
    }
}
