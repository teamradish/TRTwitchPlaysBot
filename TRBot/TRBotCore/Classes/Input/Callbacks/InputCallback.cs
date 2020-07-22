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
        public delegate void InputCB(object cbValue);

        public readonly string Input = string.Empty;
        public readonly InputCBInvocation InvocationType = InputCBInvocation.None;
        public readonly object CBValue = null;
        public readonly InputCB Callback = null;

        public InputCallback(string input, in InputCBInvocation invocation, object cbValue, InputCB callback)
        {
            Input = input;
            InvocationType = invocation;
            CBValue = cbValue;
            Callback = callback;
        }

        public static InputCB GetCallbackForCBType(in InputCBTypes cbType)
        {
            switch (cbType)
            {
                case InputCBTypes.SavestateLog: return SavestateLog;
                case InputCBTypes.BotMessage: return BotMessage;
                case InputCBTypes.SavestateSlotLog: return SavestateSlotLog;
                case InputCBTypes.ChangeStateSlot: return ChangeStateSlot;
                default: return null;
            }
        }

        public static void SavestateLog(object cbValue)
        {
            int stateNum = 0;
            try
            {
                stateNum = (int)(long)cbValue;
            }
            catch (InvalidCastException)
            {
                //Console.WriteLine($"EXCEPTION: {e.Message}");
                return;
            }

            //Console.WriteLine($"Logged state {stateNum}");

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

        public static void BotMessage(object cbValue)
        {
            //Console.WriteLine("Send bot message");

            string strVal = cbValue as string;

            if (string.IsNullOrEmpty(strVal) == true)
            {
                return;
            } 

            BotProgram.MsgHandler.QueueMessage(strVal);
        }

        public static void SavestateSlotLog(object cbValue)
        {
            //Console.WriteLine("SavestateSlotLog with: " + BotProgram.BotData.SaveLoadStateSettings.CurrentSaveSlot);

            //Save a log in the current slot
            SavestateLog((long)BotProgram.BotData.SaveLoadStateSettings.CurrentSaveSlot);
        }

        public static void ChangeStateSlot(object cbValue)
        {
            int changeAmount = 0;
            try
            {
                changeAmount = (int)(long)cbValue;
            }
            catch (InvalidCastException)
            {
                //Console.WriteLine($"EXCEPTION: {e.Message}");
                return;
            }

            int slotVal = BotProgram.BotData.SaveLoadStateSettings.CurrentSaveSlot;
            slotVal = System.Math.Clamp(slotVal + changeAmount, 1, int.MaxValue);

            //Don't save bot data, for now at least, since slots may switch often
            BotProgram.BotData.SaveLoadStateSettings.CurrentSaveSlot = slotVal;
        }
    }
}
