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
    /// A routine that displays the periodic message at a given interval. 
    /// </summary>
    public class PeriodicMessageRoutine : BaseRoutine
    {
        /// <summary>
        /// The separator for each individual periodic message.
        /// </summary>
        public const char PERIODIC_MSG_SEPARATOR = '|';

        private DateTime CurMsgTime = default;
        private int PeriodicMessageIndex = 0;

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

            long periodicMessageTime = DataHelper.GetSettingInt(SettingsConstants.PERIODIC_MSG_TIME, 1800000L);

            //If the time is surpassed, output the message
            if (msgDiff.TotalMilliseconds < periodicMessageTime)
            {
                return;
            }
                
            //If the service isn't connected, we can't send the message
            if (DataContainer.MessageHandler.ClientService.IsConnected == false)
            {
                return;
            }
            
            string periodicMessageRotation = DataHelper.GetSettingString(SettingsConstants.PERIODIC_MESSAGE_ROTATION, SettingsConstants.PERIODIC_MESSAGE);

            //Print the message if it's not empty
            if (string.IsNullOrEmpty(periodicMessageRotation) == false)
            {
                //Split the string and get the current index to output
                string[] periodicMsgs = periodicMessageRotation.Split(PERIODIC_MSG_SEPARATOR, StringSplitOptions.TrimEntries);

                //Make sure we don't go out of bounds
                if (PeriodicMessageIndex >= periodicMsgs.Length)
                {
                    PeriodicMessageIndex = 0;
                }

                string curPeriodicMsg = periodicMsgs[PeriodicMessageIndex];

                PeriodicMessageIndex++;

                //Check if this message has a database entry
                string databaseMsg = DataHelper.GetSettingString(curPeriodicMsg, string.Empty);

                //If the database string exists, use it, otherwise use the string itself
                string outputMsg = string.IsNullOrEmpty(databaseMsg) == false ? databaseMsg : curPeriodicMsg;

                DataContainer.MessageHandler.QueueMessage(outputMsg);
            }
                    
            CurMsgTime = currentTimeUTC;
        }
    }
}
