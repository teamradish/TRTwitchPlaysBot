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
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Manages user achievements.
    /// </summary>
    public class AchievementManager
    {
        //Used for quicker lookup of achievements
        private Dictionary<AchievementTypes, Dictionary<string, Achievement>> Achievements = null;

        public void Initialize(EventHandler eventHandler)
        {
            eventHandler.UserSentMessageEvent -= OnUserSentMessage;
            eventHandler.UserSentMessageEvent += OnUserSentMessage;

            eventHandler.UserMadeInputEvent -= OnUserMadeInput;
            eventHandler.UserMadeInputEvent += OnUserMadeInput;

            eventHandler.UserNewlySubscribedEvent -= OnUserNewlySubscribed;
            eventHandler.UserNewlySubscribedEvent += OnUserNewlySubscribed;

            eventHandler.UserReSubscribedEvent -= OnUserReSubscribed;
            eventHandler.UserReSubscribedEvent += OnUserReSubscribed;

            //Initialize all achievements by type for quicker lookup and faster reference
            Dictionary<string, Achievement> achDict = BotProgram.BotData.Achievements.AchievementDict;

            Achievements = new Dictionary<AchievementTypes, Dictionary<string, Achievement>>(4);

            foreach (KeyValuePair<string, Achievement> achKVPair in BotProgram.BotData.Achievements.AchievementDict)
            {
                AchievementTypes achType = achKVPair.Value.AchType;

                if (Achievements.TryGetValue(achType, out Dictionary<string, Achievement> achs) == false)
                {
                    achs = new Dictionary<string, Achievement>(8);
                    Achievements.Add(achType, achs);
                }

                achs.Add(achKVPair.Key, achKVPair.Value);
            }
        }

        private void OnUserSentMessage(User user, OnMessageReceivedArgs e)
        {
            if (user.OptedOut == true) return;

            
        }

        private void OnUserMadeInput(User user, in Parser.InputSequence validInputSeq)
        {
            if (user.OptedOut == true) return;


        }

        private void OnUserNewlySubscribed(User user, OnNewSubscriberArgs e)
        {
            if (user.OptedOut == true) return;


        }

        private void OnUserReSubscribed(User user, OnReSubscriberArgs e)
        {
            if (user.OptedOut == true) return;


        }
    }
}
