/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using TRBot.Utilities;

namespace TRBot.Misc
{
    /// <summary>
    /// Handles throttling a certain number of messages in a given interval.
    /// </summary>
    public class BotMessagePerIntervalThrottler : BotMessageThrottler
    {
        private DateTime CurMsgTime = default;
        private long CurMsgCount = 0L;

        public BotMessagePerIntervalThrottler(in MessageThrottleData msgThrottleData)
        {
            SetData(msgThrottleData);

            CurMsgTime = DateTime.UtcNow;
        }

        public override void Update(in DateTime nowUTC, BotMessageHandler botMsgHandler)
        {
            TimeSpan diff = nowUTC - CurMsgTime;

            //Reset the count if we surpassed the interval
            if (diff.TotalMilliseconds >= MsgThrottleData.MessageTime)
            {
                CurMsgCount = 0L;
                CurMsgTime = nowUTC;
            }

            //Send all the messages in the queue until we hit our limit
            while (botMsgHandler.ClientMessageCount > 0 && CurMsgCount < MsgThrottleData.MessageCount)
            {
                //End if sending the message fails
                if (botMsgHandler.SendNextQueuedMessage() == false)
                {
                    break;
                }

                CurMsgCount++;
            }
        }
    }
}
