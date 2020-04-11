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

        public override void Initialize(in TwitchClient client)
        {
            FullFilePath = Globals.GetDataFilePath(Globals.ChatBotResponseFilename);
            
            LastFileChangeTime = GetLastWriteTime();
            
            CurCheckTimestamp = DateTime.Now;
        }

        public override void UpdateRoutine(in TwitchClient client, in DateTime currentTime)
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
                    BotProgram.QueueMessage(text);
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
