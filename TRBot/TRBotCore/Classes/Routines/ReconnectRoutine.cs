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
using TwitchLib;
using TwitchLib.Client;

namespace TRBot
{
    /// <summary>
    /// Attempts to reconnect.
    /// </summary>
    public sealed class ReconnectRoutine : BaseRoutine
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

        public override void Initialize(in TwitchClient client)
        {

        }

        public override void CleanUp(in TwitchClient client)
        {

        }

        public override void UpdateRoutine(in TwitchClient client, in DateTime currentTime)
        {
            //If connected, simply return
            if (client.IsConnected == true)
            {
                InReconnection = false;
                CurReconnectionAttempts = 0;
                return;
            }

            //Check if we should attempt to reconnect
            if (client.IsConnected == false && InReconnection == false)
            {
                InReconnection = true;

                CurReconnectTimeStamp = currentTime;
            }

            if (InReconnection == true && CurReconnectionAttempts < MaxReconnectAttempts)
            {
                //Check the difference in time
                TimeSpan timeDiff = currentTime - CurReconnectTimeStamp;

                //See if it exceeds the threshold
                if (timeDiff.TotalMilliseconds < ReconnectTime)
                {
                    return;
                }

                InReconnection = false;
                CurReconnectionAttempts++;
                CurReconnectTimeStamp = currentTime;

                Console.WriteLine($"Attempting reconnect #{CurReconnectionAttempts}");

                if (CurReconnectionAttempts >= MaxReconnectAttempts)
                {
                    Console.WriteLine($"Exceeded max reconnection attempts of {MaxReconnectAttempts}");
                }

                //Double check yet again just to make sure the client isn't already connected before trying to reconnect
                if (client.IsConnected == false)
                {
                    //Attempt a reconnect
                    client.Connect();
                }
            }
        }
    }
}
