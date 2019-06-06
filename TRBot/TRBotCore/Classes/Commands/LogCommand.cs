using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Adds a log for the current game.
    /// </summary>
    public sealed class LogCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string logMessage = e.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(logMessage) == true)
            {
                BotProgram.QueueMessage("Please enter a message for the log.");
                return;
            }

            DateTime curTime = DateTime.UtcNow;

            //Add a new log
            GameLog newLog = new GameLog();
            newLog.LogMessage = logMessage;
            newLog.User = e.Command.ChatMessage.Username;

            string date = curTime.ToShortDateString();
            string time = curTime.ToLongTimeString();
            newLog.DateTimeString = $"{date} at {time}";

            BotProgram.BotData.Logs.Add(newLog);
            BotProgram.SaveBotData();

            BotProgram.QueueMessage("Successfully logged message!");
        }
    }
}
