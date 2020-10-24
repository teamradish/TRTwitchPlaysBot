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
using TRBot.Connection;
using TRBot.Data;

namespace TRBot.Routines
{
    /// <summary>
    /// Attempts to reconnect.
    /// </summary>
    public class ReconnectRoutine : BaseRoutine
    {
        /// <summary>
        /// The amount of time to attempt another reconnect.
        /// </summary>
        private readonly double ReconnectTime = 3000d;

        /// <summary>
        /// The max number of reconnect attempts.
        /// </summary>
        private readonly int MaxReconnectAttempts = 1000;

        private DateTime CurReconnectTimeStamp = default(DateTime);
        private int CurReconnectionAttempts = 0;

        private bool InReconnection = false;

        public ReconnectRoutine()
        {
            Identifier = "reconnect";
        }

        public override void Initialize(DataContainer dataContainer)
        {
            base.Initialize(dataContainer);
        }

        public override void CleanUp()
        {
            base.CleanUp();
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            //If connected, simply return
            if (DataContainer.MessageHandler.ClientService.IsConnected == true)
            {
                InReconnection = false;
                CurReconnectionAttempts = 0;
                return;
            }

            //Check if we should attempt to reconnect
            if (DataContainer.MessageHandler.ClientService.IsConnected == false && InReconnection == false)
            {
                InReconnection = true;

                CurReconnectTimeStamp = currentTimeUTC;
            }

            if (InReconnection == true && CurReconnectionAttempts < MaxReconnectAttempts)
            {
                //Check the difference in time
                TimeSpan timeDiff = currentTimeUTC - CurReconnectTimeStamp;

                //See if it exceeds the threshold
                if (timeDiff.TotalMilliseconds < ReconnectTime)
                {
                    return;
                }

                InReconnection = false;
                CurReconnectionAttempts++;
                CurReconnectTimeStamp = currentTimeUTC;

                Console.WriteLine($"Attempting reconnect #{CurReconnectionAttempts} to channel.");

                if (CurReconnectionAttempts >= MaxReconnectAttempts)
                {
                    Console.WriteLine($"Exceeded max reconnection attempts of {MaxReconnectAttempts}. Please check your internet connection and restart the bot.");
                }

                //Double check yet again just to make sure the client isn't already connected before trying to reconnect
                if (DataContainer.MessageHandler.ClientService.IsConnected == false)
                {
                    //Attempt a reconnect
                    DataContainer.MessageHandler.ClientService.Connect();
                }
            }
        }
    }
}
