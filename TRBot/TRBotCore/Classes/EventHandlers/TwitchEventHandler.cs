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
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using static TRBot.EventDelegates;

namespace TRBot
{
    /// <summary>
    /// Helps handle events from Twitch.
    /// </summary>
    public class TwitchEventHandler : IEventHandler
    {
        public event UserSentMessage UserSentMessageEvent = null;

        public event UserMadeInput UserMadeInputEvent = null;

        public event UserNewlySubscribed UserNewlySubscribedEvent = null;

        public event UserReSubscribed UserReSubscribedEvent = null;

        public event OnWhisperReceived WhisperReceivedEvent = null;

        public event ChatCommandReceived ChatCommandReceivedEvent = null;

        public event OnJoinedChannel OnJoinedChannelEvent = null;

        public event ChannelBeingHosted ChannelHostedEvent = null;

        public event OnConnected OnConnectedEvent = null;

        public event OnConnectionError OnConnectionErrorEvent = null;

        public event OnReconnected OnReconnectedEvent = null;

        public event OnDisconnected OnDisconnectedEvent = null;

        private TwitchClient twitchClient = null;

        public TwitchEventHandler(TwitchClient client)
        {
            twitchClient = client;
        }

        public void Initialize()
        {
            twitchClient.OnMessageReceived -= OnMessageReceived;
            twitchClient.OnMessageReceived += OnMessageReceived;

            twitchClient.OnNewSubscriber -= OnNewSubscriber;
            twitchClient.OnNewSubscriber += OnNewSubscriber;
            
            twitchClient.OnReSubscriber -= OnReSubscriber;
            twitchClient.OnReSubscriber += OnReSubscriber;

            twitchClient.OnWhisperReceived -= OnWhisperReceived;
            twitchClient.OnWhisperReceived += OnWhisperReceived;

            twitchClient.OnChatCommandReceived -= OnChatCommandReceived;
            twitchClient.OnChatCommandReceived += OnChatCommandReceived;

            twitchClient.OnJoinedChannel -= OnJoinedChannel;
            twitchClient.OnJoinedChannel += OnJoinedChannel;

            twitchClient.OnBeingHosted -= OnChannelHosted;
            twitchClient.OnBeingHosted += OnChannelHosted;

            twitchClient.OnConnected -= OnConnected;
            twitchClient.OnConnected += OnConnected;

            twitchClient.OnConnectionError -= OnConnectionError;
            twitchClient.OnConnectionError += OnConnectionError;

            twitchClient.OnReconnected -= OnReconnected;
            twitchClient.OnReconnected += OnReconnected;

            twitchClient.OnDisconnected -= OnDisconnected;
            twitchClient.OnDisconnected += OnDisconnected;
        }

        public void CleanUp()
        {
            twitchClient.OnMessageReceived -= OnMessageReceived;
            twitchClient.OnNewSubscriber -= OnNewSubscriber;
            twitchClient.OnReSubscriber -= OnReSubscriber;
            twitchClient.OnWhisperReceived -= OnWhisperReceived;
            twitchClient.OnChatCommandReceived -= OnChatCommandReceived;
            twitchClient.OnJoinedChannel -= OnJoinedChannel;
            twitchClient.OnBeingHosted -= OnChannelHosted;
            twitchClient.OnConnected -= OnConnected;
            twitchClient.OnConnectionError -= OnConnectionError;
            twitchClient.OnReconnected -= OnReconnected;
            twitchClient.OnDisconnected -= OnDisconnected;

            UserSentMessageEvent = null;
            UserMadeInputEvent = null;
            UserNewlySubscribedEvent = null;
            UserReSubscribedEvent = null;
        }

        //Break up much of the message handling by sending events
        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.ChatMessage.DisplayName, false);

            UserSentMessageEvent?.Invoke(user, e);

            //Attempt to parse the message as an input
            ProcessMsgAsInput(user, e);
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.Subscriber.DisplayName, false);

            UserNewlySubscribedEvent?.Invoke(user, e);
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            User user = BotProgram.GetOrAddUser(e.ReSubscriber.DisplayName, false);

            UserReSubscribedEvent?.Invoke(user, e);
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            WhisperReceivedEvent?.Invoke(e);
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            ChatCommandReceivedEvent?.Invoke(e);
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            OnJoinedChannelEvent?.Invoke(e);
        }

        private void OnChannelHosted(object sender, OnBeingHostedArgs e)
        {
            ChannelHostedEvent?.Invoke(e);
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            OnConnectedEvent?.Invoke(e);
        }

        private void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            OnConnectionErrorEvent?.Invoke(e);
        }

        private void OnReconnected(object sender, OnReconnectedEventArgs e)
        {
            OnReconnectedEvent?.Invoke(e);
        }

        private void OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            OnDisconnectedEvent?.Invoke(e);
        }

        //NOTE: This would result in lots of code duplication if other streaming services were integrated
        //Is there a better way to do this?

        private void ProcessMsgAsInput(User userData, OnMessageReceivedArgs e)
        {
            //Don't process for inputs if a meme
            string possibleMeme = e.ChatMessage.Message.ToLower();
            if (BotProgram.BotData.Memes.TryGetValue(possibleMeme, out string meme) == true)
            {
                return;
            }

            //Ignore commands as inputs
            if (possibleMeme.StartsWith(Globals.CommandIdentifier) == true)
            {
                return;
            }

            //If there are no valid inputs, don't attempt to parse
            if (InputGlobals.CurrentConsole.ValidInputs == null || InputGlobals.CurrentConsole.ValidInputs.Length == 0)
            {
                return;
            }

            //Parser.InputSequence inputSequence = default;
            //(bool, List<List<Parser.Input>>, bool, int) parsedVal = default;
            Parser.InputSequence inputSequence = default;

            try
            {
                string parse_message = Parser.Expandify(Parser.PopulateMacros(e.ChatMessage.Message));
                inputSequence = Parser.ParseInputs(parse_message, true);
                //parsedVal = Parser.Parse(parse_message);
                //Console.WriteLine(inputSequence.ToString());
                //Console.WriteLine("\nReverse Parsed: " + ReverseParser.ReverseParse(inputSequence));
                //Console.WriteLine("\nReverse Parsed Natural:\n" + ReverseParser.ReverseParseNatural(inputSequence));
            }
            catch (Exception exception)
            {
                //Kimimaru: Sanitize parsing exceptions
                //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                //One example that throws this that shouldn't is "#mash(w234"
                //BotProgram.QueueMessage($"ERROR: {exception.Message}");
                inputSequence.InputValidationType = Parser.InputValidationTypes.Invalid;
                //parsedVal.Item1 = false;
            }

            //Check for non-valid messages
            if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
            {
                //Display error message for invalid inputs
                if (inputSequence.InputValidationType == Parser.InputValidationTypes.Invalid)
                {
                    BotProgram.QueueMessage(inputSequence.Error);
                }

                return;
            }

            //It's a valid message, so process it
                
            //Ignore if user is silenced
            if (userData.Silenced == true)
            {
                return;
            }

            //Ignore based on user level and permissions
            if (userData.Level < BotProgram.BotData.InputPermissions)
            {
                BotProgram.QueueMessage($"Inputs are restricted to levels {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above");
                return;
            }

            #region Parser Post-Process Validation
            
            /* All this validation is very slow
             * Find a way to speed it up, ideally without integrating it directly into the parser
             */
            
            //Check if the user has permission to perform all the inputs they attempted
            ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissions(userData.Level, inputSequence.Inputs,
                BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            if (inputValidation.IsValid == false)
            {
                if (string.IsNullOrEmpty(inputValidation.Message) == false)
                {
                    BotProgram.QueueMessage(inputValidation.Message);
                }
                return;
            }

            //Lastly, check for invalid button combos given the current console
            if (BotProgram.BotData.InvalidBtnCombos.InvalidCombos.TryGetValue((int)InputGlobals.CurrentConsoleVal, out List<string> invalidCombos) == true)
            {
                if (ParserPostProcess.ValidateButtonCombos(inputSequence.Inputs, invalidCombos, userData.Team) == false)
                {
                    string msg = "Invalid input: buttons ({0}) are not allowed to be pressed at the same time.";
                    string combos = string.Empty;
                    
                    for (int i = 0; i < invalidCombos.Count; i++)
                    {
                        combos += "\"" + invalidCombos[i] + "\"";
                        
                        if (i < (invalidCombos.Count - 1))
                        {
                            combos += ", ";
                        }
                    }
                    
                    msg = string.Format(msg, combos);
                    BotProgram.QueueMessage(msg);
                    
                    return;
                }
            }

            #endregion

            if (InputHandler.StopRunningInputs == false)
            {
                //Invoke input event
                UserMadeInputEvent?.Invoke(userData, inputSequence);
            }
            else
            {
                BotProgram.QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
            }
        }
    }
}
