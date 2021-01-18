/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using TRBot.Logging;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine that attempts to reconnect to the client service after detecting a disconnection.
    /// </summary>
    public class ReconnectRoutine : BaseRoutine
    {
        /// <summary>
        /// The amount of time to attempt another reconnect.
        /// </summary>
        private long ReconnectTime = 5000L;

        /// <summary>
        /// The max number of reconnect attempts.
        /// </summary>
        private readonly int MaxReconnectAttempts = 1000;

        private DateTime CurReconnectTimeStamp = default(DateTime);
        private int CurReconnectionAttempts = 0;

        private bool InReconnection = false;

        public ReconnectRoutine()
        {
            Identifier = RoutineConstants.RECONNECT_ROUTINE_ID;
        }

        public override void Initialize()
        {
            base.Initialize();

            ReconnectTime = DataHelper.GetSettingInt(SettingsConstants.RECONNECT_TIME, 5000L);

            DataContainer.DataReloader.SoftDataReloadedEvent -= OnReload;
            DataContainer.DataReloader.SoftDataReloadedEvent += OnReload;

            DataContainer.DataReloader.HardDataReloadedEvent -= OnReload;
            DataContainer.DataReloader.HardDataReloadedEvent += OnReload;
        }

        public override void CleanUp()
        {
            DataContainer.DataReloader.SoftDataReloadedEvent -= OnReload;
            DataContainer.DataReloader.HardDataReloadedEvent -= OnReload;

            base.CleanUp();
        }

        private void OnReload()
        {
            //Fetch the new reconnect time
            ReconnectTime = DataHelper.GetSettingInt(SettingsConstants.RECONNECT_TIME, 5000L);
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

                TRBotLogger.Logger.Information($"Attempting reconnect #{CurReconnectionAttempts} to channel.");

                if (CurReconnectionAttempts >= MaxReconnectAttempts)
                {
                    TRBotLogger.Logger.Error($"Exceeded max reconnection attempts of {MaxReconnectAttempts}. Please check your internet connection and restart the bot.");
                }

                //Double check yet again just to make sure the client isn't already connected before trying to reconnect
                if (DataContainer.MessageHandler.ClientService.IsConnected == false)
                {
                    //Attempt a reconnect
                    try
                    {
                        DataContainer.MessageHandler.ClientService.Connect();
                    }
                    catch (Exception e)
                    {
                        TRBotLogger.Logger.Error($"Unable to reconnect to client service: {e.Message}\n{e.StackTrace}");
                    }
                }
            }
        }
    }
}
