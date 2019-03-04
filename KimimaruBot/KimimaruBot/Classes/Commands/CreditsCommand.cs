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
        public static Dictionary<string, long> UserCredits = new Dictionary<string, long>();
        public static readonly string FileName = $"{Globals.DataPath}UserCredits.txt";

        public CreditsCommand()
        {

        }

        public static void SaveDict()
        {
            string creditJSON = JsonConvert.SerializeObject(UserCredits, Formatting.Indented);

            File.WriteAllText(FileName, creditJSON);
        }

        public override void Initialize(CommandHandler commandHandler)
        {
            string text = string.Empty;
            text = File.ReadAllText(FileName);

            UserCredits = JsonConvert.DeserializeObject<Dictionary<string,long>>(text);
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

                if (UserCredits.ContainsKey(userLower) == false)
                {
                    if (sameName == true)
                    {
                        UserCredits.Add(userLower, 0);

                        SaveDict();
                    }
                    else
                    {
                        BotProgram.QueueMessage($"{userName} is not in the database.");
                        return;
                    }
                }

                BotProgram.QueueMessage($"{userName} has {UserCredits[userLower]} credit(s)!");
            }
            else
            {
                //Compare credits
                string name1 = arguments[0];
                string name2 = arguments[1];

                string name1Lower = name1.ToLower();
                string name2Lower = name2.ToLower();

                if (UserCredits.ContainsKey(name1Lower) == false)
                {
                    BotProgram.QueueMessage($"{name1} is not in the database!");
                    return;
                }
                if (UserCredits.ContainsKey(name2Lower) == false)
                {
                    BotProgram.QueueMessage($"{name2} is not in the database!");
                    return;
                }

                long credits1 = UserCredits[name1Lower];
                long credits2 = UserCredits[name2Lower];

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
