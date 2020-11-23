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
using TRBot.Connection;
using TRBot.Data;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine that displays the periodic message at a given interval. 
    /// </summary>
    public class PeriodicMessageRoutine : BaseRoutine
    {
        private DateTime CurMsgTime = default;

        public PeriodicMessageRoutine()
        {
            Identifier = RoutineConstants.PERIODIC_MSG_ROUTINE_ID;
        }

        public override void Initialize()
        {
            base.Initialize();

            CurMsgTime = DateTime.UtcNow;
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            TimeSpan msgDiff = currentTimeUTC - CurMsgTime;

            using BotDBContext context = DatabaseManager.OpenContext();

            long periodicMessageTime = DataHelper.GetSettingIntNoOpen(SettingsConstants.PERIODIC_MSG_TIME, context, 1800000L);
            string periodicMessage = DataHelper.GetSettingStringNoOpen(SettingsConstants.PERIODIC_MESSAGE, context, "This is your friendly Twitch Plays bot :D ! I hope you're enjoying the stream!");

            if (msgDiff.TotalMilliseconds >= periodicMessageTime)
            {
                if (DataContainer.MessageHandler.ClientService.IsConnected == true)
                {
                    if (string.IsNullOrEmpty(periodicMessage) == false)
                    {
                        DataContainer.MessageHandler.QueueMessage(periodicMessage);
                    }
                    
                    CurMsgTime = currentTimeUTC;
                }
            }
        }
    }
}
