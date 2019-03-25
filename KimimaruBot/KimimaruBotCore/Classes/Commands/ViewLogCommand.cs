using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Views a log that has been created.
    /// </summary>
    public sealed class ViewLogCommand : BaseCommand
    {
        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            int logNum = 0;

            //If no log number was specified, use the most recent one
            if (args.Count == 0)
            {
                logNum = BotProgram.BotData.Logs.Count - 1;
            }
            else if (args.Count == 1)
            {
                string num = args[0];
                if (int.TryParse(num, out logNum) == false)
                {
                    BotProgram.QueueMessage("Invalid log number!");
                    return;
                }

                //Go from back to front (Ex. "!viewlog 1" shows the most recent log)
                logNum = BotProgram.BotData.Logs.Count - logNum;
            }
            else
            {
                BotProgram.QueueMessage($"Usage: \"recent log number (optional)\"");
                return;
            }

            if (logNum < 0 || logNum >= BotProgram.BotData.Logs.Count)
            {
                BotProgram.QueueMessage($"No log found!");
                return;
            }

            GameLog log = BotProgram.BotData.Logs[logNum];

            BotProgram.QueueMessage($"{log.DateTimeString} (UTC) --> {log.User} : {log.LogMessage}");
        }
    }
}
