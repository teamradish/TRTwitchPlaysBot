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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Displays the current time in a given timezone.
    /// </summary>
    public class TimeCommand : BaseCommand
    {
        public TimeCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string arguments = args.Command.ArgumentsAsString;

            DateTime curTime = DateTime.UtcNow;
            string timeZoneCode = "UTC";

            //If we have an argument, try to parse for a different time zone
            if (string.IsNullOrEmpty(arguments) == false)
            {
                //Set the code
                timeZoneCode = arguments;

                //Try to parse based on the time zone code
                try
                {
                    curTime = TimeZoneInfo.ConvertTimeFromUtc(curTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneCode));
                }
                catch
                {
                    string message = "Unable to parse time zone ID. Time zone names are case-sensitive.";

                    //Send them to a different reference on each OS, as the codes differ
                    if (TRBotOSPlatform.CurrentOS == TRBotOSPlatform.OS.Windows)
                    {
                        message += " Here's a list of time zone names (\"Name of Time Zone\" column): https://support.microsoft.com/en-us/help/973627/microsoft-time-zone-index-values.";
                    }
                    else
                    {
                        message += " Here's a list of time zone names (\"TZ database name\" column): https://en.wikipedia.org/wiki/List_of_tz_database_time_zones.";
                    }

                    QueueMessage(message);
                    return;
                }
            }

            QueueMessage($"The current time is {curTime.ToLongTimeString()} on {curTime.ToShortDateString()} ({timeZoneCode})!");
        }
    }
}
