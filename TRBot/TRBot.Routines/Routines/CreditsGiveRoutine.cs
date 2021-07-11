/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using TRBot.Connection;
using TRBot.Data;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine that grants users credits for participating in chat after some time.
    /// </summary>
    public class CreditsGiveRoutine : BaseRoutine
    {
        private DateTime CurCreditsTime;

        private readonly Dictionary<string, bool> UsersTalked = new Dictionary<string, bool>();

        public CreditsGiveRoutine()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            DataContainer.MessageHandler.ClientService.EventHandler.UserSentMessageEvent -= MessageReceived;
            DataContainer.MessageHandler.ClientService.EventHandler.UserSentMessageEvent += MessageReceived;

            CurCreditsTime = DateTime.UtcNow;
        }

        public override void CleanUp()
        {
            DataContainer.MessageHandler.ClientService.EventHandler.UserSentMessageEvent -= MessageReceived;

            base.CleanUp();
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            TimeSpan creditsDiff = currentTimeUTC - CurCreditsTime;

            long creditsTimeMS = DataHelper.GetSettingInt(SettingsConstants.CREDITS_GIVE_TIME, -1L);

            //Don't do anything if the credits time is less than 0
            if (creditsTimeMS < 0L)
            {
                CurCreditsTime = currentTimeUTC;
                return;
            }

            long creditsGiveAmount = DataHelper.GetSettingInt(SettingsConstants.CREDITS_GIVE_AMOUNT, 100L);

            //Check if we surpassed the time
            if (creditsDiff.TotalMilliseconds >= creditsTimeMS)
            {
                string[] talkedNames = UsersTalked.Keys.ToArray();
                for (int i = 0; i < talkedNames.Length; i++)
                {
                    using (BotDBContext context = DatabaseManager.OpenContext())
                    {
                        //Add to each user's credits and save
                        User user = DataHelper.GetUserNoOpen(talkedNames[i], context);
                        user.Stats.Credits += creditsGiveAmount;

                        context.SaveChanges();
                    }
                }

                UsersTalked.Clear();

                CurCreditsTime = currentTimeUTC;
            }
        }

        private void MessageReceived(EvtUserMessageArgs e)
        {
            string nameToLower = e.UsrMessage.Username.ToLowerInvariant();

            //Check if the user talked before
            if (UsersTalked.ContainsKey(nameToLower) == false)
            {
                long creditsTimeMS = DataHelper.GetSettingInt(SettingsConstants.CREDITS_GIVE_TIME, -1L);

                //Don't do anything if the credits time is less than 0
                if (creditsTimeMS < 0L)
                {
                    return;
                }

                //If so, check if they're in the database and not opted out, then add them for gaining credits
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User user = DataHelper.GetUserNoOpen(nameToLower, context);
                    if (user != null && user.IsOptedOut == false)
                    {
                        UsersTalked.Add(nameToLower, true);
                    }
                }
            }
        }
    }
}
