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
using TRBot.Parsing;

namespace TRBot.Connection
{
    //Kimimaru: These classes are abstractions on top of the service APIs
    //It's not currently known how much information is common among services, so these may need to be adjusted
    //or further abstracted in some manner

    #region Event Data Objects

    public abstract class EvtBaseMsgData
    {
        public string UserID { get; protected set; } = string.Empty;
        public string Username { get; protected set; } = string.Empty;
        public string DisplayName { get; protected set; } = string.Empty;

        protected EvtBaseMsgData(string userId, string username, string displayName)
        {
            UserID = userId;
            Username = username;
            DisplayName = displayName;
        }
    }

    public class EvtUserMsgData : EvtBaseMsgData
    {
        public string Channel { get; protected set; } = string.Empty;
        public string Message { get; protected set; } = string.Empty;

        public EvtUserMsgData(string userId, string username, string displayName, string channel, string message)
         : base (userId, username, displayName)
        {
            Channel = channel;
            Message = message;
        }
    }

    public class EvtWhisperMsgData : EvtBaseMsgData
    {
        public string MessageId { get; protected set; } = string.Empty;
        public string ThreadId { get; protected set; } = string.Empty;
        public string Message { get; protected set; } = string.Empty;

        public EvtWhisperMsgData(string userId, string username, string displayName,
            string messageId, string threadId, string message)
            : base (userId, username, displayName)
        {
            MessageId = messageId;
            ThreadId = threadId;
            Message = message;
        }
    }

    public class EvtHostedData
    {
        public string Channel { get; protected set; } = string.Empty;
        public string HostedByChannel { get; protected set; } = string.Empty;
        public int Viewers { get; protected set; } = 0;
        public bool AutoHosted { get; protected set; } = false;

        public EvtHostedData(string channel, string hostedByChannel, in int viewers,
            in bool autoHosted)
        {
            Channel = channel;
            HostedByChannel = hostedByChannel;
            Viewers = viewers;
            AutoHosted = autoHosted;
        }
    }

    public abstract class EvtBaseSubscriptionData
    {
        public string UserId { get; } = string.Empty;
        public string DisplayName { get; } = string.Empty;
        public string Channel { get; protected set; } = string.Empty;

        protected EvtBaseSubscriptionData(string userId, string displayName, string channel)
        {
            UserId = userId;
            DisplayName = displayName;
            Channel = channel;
        }
    }

    public class EvtSubscriptionData : EvtBaseSubscriptionData
    {
        public EvtSubscriptionData(string userId, string displayName, string channel)
            : base(userId, displayName, channel)
        {
            
        }
    }

    public class EvtReSubscriptionData : EvtBaseSubscriptionData
    {
        public int Months { get; protected set; } = 0;

        public EvtReSubscriptionData(string userId, string displayName, string channel, in int months)
            : base(userId, displayName, channel)
        {
            Months = months;
        }
    }

    public class EvtChatCommandData
    {
        public List<string> ArgumentsAsList { get; protected set; } = null;
        public string ArgumentsAsString { get; } = string.Empty;
        public EvtUserMsgData ChatMessage { get; protected set; } = null;
        public char CommandIdentifier { get; } = '\0';
        public string CommandText { get; } = string.Empty;

        public EvtChatCommandData(List<string> argsAsList, string argsAsStr, EvtUserMsgData usrMsgData,
            char cmdIdentifier, string cmdText)
        {
            ArgumentsAsList = argsAsList;
            ArgumentsAsString = argsAsStr;
            ChatMessage = usrMsgData;
            CommandIdentifier = cmdIdentifier;
            CommandText = cmdText;
        }
    }

    public class EvtErrorData
    {
        public string Message { get; protected set; } = string.Empty;

        public EvtErrorData(string message)
        {
            Message = message;
        }
    }

    #endregion

    #region Event Args Objects

    public class EvtUserMessageArgs : EventArgs
    {
        //public User UserData = null;
        public EvtUserMsgData UsrMessage = null;
    }

    public class EvtUserInputArgs : EventArgs
    {
        //public User UserData = null;
        public EvtUserMsgData UsrMessage = null;
        public ParsedInputSequence ValidInputSeq = default;
    }

    public class EvtWhisperMessageArgs : EventArgs
    {
        public EvtWhisperMsgData WhsprMessage = null;
    }

    public class EvtOnHostedArgs : EventArgs
    {
        public EvtHostedData HostedData = null;
    }

    public class EvtOnSubscriptionArgs : EventArgs
    {
        //public User UserData = null;
        public EvtSubscriptionData SubscriptionData = null;
    }

    public class EvtOnReSubscriptionArgs : EventArgs
    {
        //public User UserData = null;
        public EvtReSubscriptionData ReSubscriptionData = null;
    }

    public class EvtChatCommandArgs : EventArgs
    {
        //public User UserData = null;
        public EvtChatCommandData Command = null;
    }

    public class EvtJoinedChannelArgs : EventArgs
    {
        public string BotUsername = string.Empty;
        public string Channel = string.Empty;
    }

    public class EvtConnectedArgs : EventArgs
    {
        public string BotUsername = string.Empty;
        public string AutoJoinChannel = string.Empty;
    }

    public class EvtConnectionErrorArgs : EventArgs
    {
        public EvtErrorData Error;
        public string BotUsername;
    }

    public class EvtReconnectedArgs : EventArgs
    {
        
    }

    public class EvtDisconnectedArgs : EventArgs
    {

    }

    #endregion
}
