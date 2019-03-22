using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace KimimaruBot
{
    public sealed class CreditsCommand : BaseCommand
    {
        public static readonly string FileName = Globals.GetDataFilePath("UserCredits.txt");

        public CreditsCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> arguments = e.Command.ArgumentsAsList;

            if (arguments == null || (arguments.Count <= 1 || arguments.Count > 2))
            {
                bool sameName = true;
                string userName = e.Command.ChatMessage.DisplayName;

                if (arguments != null && arguments.Count > 0)
                {
                    //Allow seeing other users' credits count
                    if (userName != arguments[0])
                    {
                        userName = arguments[0];
                        sameName = false;
                    }
                }

                string userLower = userName.ToLower();

                if (BotProgram.BotData.Users.ContainsKey(userLower) == false)
                {
                    if (sameName == true)
                    {
                        //Kimimaru: comment out for now - unsure if commands would happen before messages
                        //UserCredits.Add(userLower, 0);
                        //
                        //SaveDict();
                    }
                    else
                    {
                        BotProgram.QueueMessage($"{userName} is not in the database.");
                        return;
                    }
                }

                BotProgram.QueueMessage($"{userName} has {BotProgram.BotData.Users[userLower].Credits} credit(s)!");
            }
            else
            {
                //Compare credits
                string name1 = arguments[0];
                string name2 = arguments[1];

                string name1Lower = name1.ToLower();
                string name2Lower = name2.ToLower();

                if (BotProgram.BotData.Users.ContainsKey(name1Lower) == false)
                {
                    BotProgram.QueueMessage($"{name1} is not in the database!");
                    return;
                }
                if (BotProgram.BotData.Users.ContainsKey(name2Lower) == false)
                {
                    BotProgram.QueueMessage($"{name2} is not in the database!");
                    return;
                }

                long credits1 = BotProgram.BotData.Users[name1Lower].Credits;
                long credits2 = BotProgram.BotData.Users[name2Lower].Credits;

                string message = string.Empty;
                if (credits1 < credits2)
                {
                    long diff = credits2 - credits1;
                    message = $"{name1} has {diff} fewer credit(s) than {name2}!";
                }
                else if (credits1 > credits2)
                {
                    long diff = credits1 - credits2;
                    message = $"{name1} has {diff} more credit(s) than {name2}!";
                }
                else
                {
                    message = $"{name1} and {name2} have an equal number of credits!";
                }

                BotProgram.QueueMessage(message);
            }
        }
    }
}
