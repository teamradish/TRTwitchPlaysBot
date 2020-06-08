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
using System.IO;
using TwitchLib;
using TwitchLib.Client;

namespace TRBot
{
    public sealed class ChatBotResponseRoutine : BaseRoutine
    {
        private const double CheckTime = 750d;
        private DateTime CurCheckTimestamp = default(DateTime);
        
        private DateTime LastFileChangeTime = default(DateTime);

        private string FullFilePath = string.Empty;

        public override void Initialize(IClientService clientService)
        {
            FullFilePath = Globals.GetDataFilePath(Globals.ChatBotResponseFilename);
            
            LastFileChangeTime = GetLastWriteTime();
            
            CurCheckTimestamp = DateTime.Now;
        }

        public override void UpdateRoutine(IClientService client, in DateTime currentTime)
        {
            //We're not using the chatbot, so return
            if (BotProgram.BotSettings.UseChatBot == false)
            {
                return;
            }
            
            TimeSpan msgDiff = currentTime - CurCheckTimestamp;

            if (msgDiff.TotalMilliseconds < CheckTime)
            {
                return;
            }
            
            CurCheckTimestamp = currentTime;
            
            DateTime lastWrite = GetLastWriteTime();
            
            if (lastWrite > LastFileChangeTime)
            {
                LastFileChangeTime = lastWrite;
                
                //Send the message
                string text = Globals.ReadFromTextFile(Globals.ChatBotResponseFilename);
                
                if (string.IsNullOrEmpty(text) == false)
                {
                    BotProgram.MsgHandler.QueueMessage(text);
                }
            }
        }
        
        private DateTime GetLastWriteTime()
        {
            if (File.Exists(FullFilePath) == true)
            {
                try
                {
                    return File.GetLastWriteTime(FullFilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to get last file change time: {e.Message}");
                    return DateTime.UnixEpoch;
                }
            }
            
            return DateTime.UnixEpoch;
        }
    }
}
