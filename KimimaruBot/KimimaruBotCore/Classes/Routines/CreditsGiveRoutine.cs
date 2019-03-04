using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class CreditsGiveRoutine : BaseRoutine
    {
        private DateTime CurCreditsTime;

        private readonly Dictionary<string, bool> UsersTalked = new Dictionary<string, bool>();

        public CreditsGiveRoutine()
        {

        }

        public override void Initialize(in TwitchClient client)
        {
            CurCreditsTime = DateTime.Now;

            client.OnMessageReceived -= MessageReceived;
            client.OnMessageReceived += MessageReceived;
        }

        public override void CleanUp(in TwitchClient client)
        {
            client.OnMessageReceived -= MessageReceived;
        }

        public override void UpdateRoutine(in TwitchClient client, in DateTime currentTime)
        {
            TimeSpan creditsDiff = currentTime - CurCreditsTime;

            //Credits time
            if (creditsDiff.TotalMinutes >= BotProgram.BotSettings.CreditsTime)
            {
                string[] talkedNames = UsersTalked.Keys.ToArray();
                for (int i = 0; i < talkedNames.Length; i++)
                {
                    CreditsCommand.UserCredits[talkedNames[i]] += BotProgram.BotSettings.CreditsAmount;
                }

                CreditsCommand.SaveDict();
                UsersTalked.Clear();

                CurCreditsTime = currentTime;
            }
        }

        private void MessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string nameToLower = e.ChatMessage.DisplayName.ToLower();

            //Check if the user talked before
            if (UsersTalked.ContainsKey(nameToLower) == false)
            {
                //If so, check if they're in the credits database and add them for gaining credits
                if (CreditsCommand.UserCredits.ContainsKey(nameToLower) == true)
                {
                    UsersTalked.Add(nameToLower, true);
                }
            }
        }
    }
}
