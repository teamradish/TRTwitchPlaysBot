using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;

namespace TRBot
{
    public sealed class PeriodicMessageRoutine : BaseRoutine
    {
        private DateTime CurMsgTime;

        public override void Initialize(in TwitchClient client)
        {
            CurMsgTime = DateTime.Now;
        }

        public override void CleanUp(in TwitchClient client)
        {
            
        }

        public override void UpdateRoutine(in TwitchClient client, in DateTime currentTime)
        {
            TimeSpan msgDiff = currentTime - CurMsgTime;

            if (msgDiff.TotalMinutes >= BotProgram.BotSettings.MessageTime)
            {
                if (client.IsConnected == true)
                {
                    BotProgram.QueueMessage($"Hi! I'm {BotProgram.BotName} :D ! Use {Globals.CommandIdentifier}help to display a list of commands!");
                    CurMsgTime = currentTime;
                }
            }
        }
    }
}
