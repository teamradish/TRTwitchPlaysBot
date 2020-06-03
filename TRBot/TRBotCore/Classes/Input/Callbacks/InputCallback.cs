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

namespace TRBot
{
    /// <summary>
    /// A callback invoked when a certain input is encountered.
    /// </summary>
    public class InputCallback
    {
        public delegate void InputCB();

        public readonly string Input = string.Empty;
        public readonly InputCBInvocation InvocationType = InputCBInvocation.None;
        public readonly InputCB Callback = null;

        public InputCallback(string input, in InputCBInvocation invocation, InputCB callback)
        {
            Input = input;
            InvocationType = invocation;
            Callback = callback;
        }

        public static InputCB GetCallbackForCBType(in InputCBTypes cbType)
        {
            switch (cbType)
            {
                case InputCBTypes.SavestateLog1: return SavestateLog1;
                case InputCBTypes.SavestateLog2: return SavestateLog2;
                case InputCBTypes.SavestateLog3: return SavestateLog3;
                case InputCBTypes.SavestateLog4: return SavestateLog4;
                case InputCBTypes.SavestateLog5: return SavestateLog5;
                case InputCBTypes.SavestateLog6: return SavestateLog6;
                default: return null;
            }
        }

        public static void SavestateLog1()
        {
            SavestateLog(1);
        }

        public static void SavestateLog2()
        {
            SavestateLog(2);
        }

        public static void SavestateLog3()
        {
            SavestateLog(3);
        }

        public static void SavestateLog4()
        {
            SavestateLog(4);
        }

        public static void SavestateLog5()
        {
            SavestateLog(5);
        }

        public static void SavestateLog6()
        {
            SavestateLog(6);
        }

        public static void SavestateLog(in int stateNum)
        {
            //Track the time of the savestate
            DateTime curTime = DateTime.UtcNow;

            //Add a new savestate log
            GameLog newStateLog = new GameLog();
            newStateLog.User = string.Empty;//user.Name;

            string date = curTime.ToShortDateString();
            string time = curTime.ToLongTimeString();
            newStateLog.DateTimeString = $"{date} at {time}";

            newStateLog.LogMessage = string.Empty;

            //Add or replace the log and save the bot data
            BotProgram.BotData.SavestateLogs[stateNum] = newStateLog;
            BotProgram.SaveBotData();
        }
    }
}
