/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace TRBot
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
                    User user = BotProgram.GetOrAddUser(talkedNames[i]);
                    user.Credits += BotProgram.BotSettings.CreditsAmount;
                }

                BotProgram.SaveBotData();
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
                User user = BotProgram.GetUser(nameToLower);

                //If so, check if they're in the credits database and not opted out,
                //then add them for gaining credits
                if (user != null && user.OptedOut == false)
                {
                    UsersTalked.Add(nameToLower, true);
                }
            }
        }
    }
}
