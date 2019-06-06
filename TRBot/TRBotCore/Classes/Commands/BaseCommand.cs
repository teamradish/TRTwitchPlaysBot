using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace TRBot
{
    /// <summary>
    /// Base class for a command
    /// </summary>
    public abstract class BaseCommand
    {
        public bool HiddenFromHelp = false;

        //Kimimaru: We might want to specify this in the settings file to make things more configurable
        public int AccessLevel { get; protected set; } = (int)AccessLevels.Levels.User;

        public BaseCommand()
        {
            
        }

        public virtual void Initialize(CommandHandler commandHandler)
        {

        }

        public abstract void ExecuteCommand(object sender, OnChatCommandReceivedArgs e);
    }
}
