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
using System.Threading;
using TRBot.Parsing;
using static TRBot.Connection.EventDelegates;

namespace TRBot.Connection
{
    /// <summary>
    /// Handles events from input through a terminal.
    /// </summary>
    public class TerminalEventHandler : IEventHandler
    {
        public event UserSentMessage UserSentMessageEvent = null;

        //public event UserMadeInput UserMadeInputEvent = null;

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

        private volatile bool StopConsoleThread = false;

        private char CommandIdentifier = '!';

        public TerminalEventHandler(char commandIdentifier)
        {
            CommandIdentifier = commandIdentifier;
        }

        public void Initialize()
        {
            WaitForMainInitialization();       
        }

        public void CleanUp()
        {
            StopConsoleThread = true;

            UserSentMessageEvent = null;
            UserNewlySubscribedEvent = null;
            UserReSubscribedEvent = null;
            WhisperReceivedEvent = null;
            ChatCommandReceivedEvent = null;
            OnJoinedChannelEvent = null;
            ChannelHostedEvent = null;
            OnConnectedEvent = null;
            OnConnectionErrorEvent = null;
            OnReconnectedEvent = null;
            OnDisconnectedEvent = null;
        }

        //NOTE: This would result in lots of code duplication if other streaming services were integrated
        //Is there a better way to do this?

        /*private void ProcessMsgAsInput(EvtUserMessageArgs e)
        {
            //User userData = e.UserData;

            //Ignore commands as inputs
            //if (e.UsrMessage.Message.StartsWith(Globals.CommandIdentifier) == true)
            //{
            //    return;
            //}

            //If there are no valid inputs, don't attempt to parse
            //if (InputGlobals.CurrentConsole.ValidInputs == null || InputGlobals.CurrentConsole.ValidInputs.Length == 0)
            //{
            //    return;
            //}

            //Parser.InputSequence inputSequence = default;
            //(bool, List<List<Parser.Input>>, bool, int) parsedVal = default;
            InputSequence inputSequence = default;

            try
            {
                TRBot.Parsing.Parser parser = new TRBot.Parsing.Parser();
                
                string regexStr = parser.BuildInputRegex(new string[] { "a", "b", "up", "down", "left", "right", "select", "start", "x", "y", "l", "r", "#" } );

                string parse_message = parser.Expandify(e.UsrMessage.Message);//parser.PopulateMacros(e.UsrMessage.Message, null, null));
                //parse_message = parser.PopulateSynonyms(parse_message, InputGlobals.InputSynonyms);
                inputSequence = parser.ParseInputs(parse_message, regexStr, new ParserOptions(0, 200, true, 60000));
                //parsedVal = Parser.Parse(parse_message);
                //Console.WriteLine(inputSequence.ToString());
                //Console.WriteLine("\nReverse Parsed: " + ReverseParser.ReverseParse(inputSequence));
                //Console.WriteLine("\nReverse Parsed Natural:\n" + ReverseParser.ReverseParseNatural(inputSequence));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Kimimaru: Sanitize parsing exceptions
                //Most of these are currently caused by differences in how C# and Python handle slicing strings (Substring() vs string[:])
                //One example that throws this that shouldn't is "#mash(w234"
                //BotProgram.MsgHandler.QueueMessage($"ERROR: {excMsg}");
                inputSequence.InputValidationType = InputValidationTypes.Invalid;
                //parsedVal.Item1 = false;
            }

            //Check for non-valid messages
            if (inputSequence.InputValidationType != InputValidationTypes.Valid)
            {
                //Display error message for invalid inputs
                if (inputSequence.InputValidationType == InputValidationTypes.Invalid)
                {
                    Console.WriteLine(inputSequence.Error);
                    //BotProgram.MsgHandler.QueueMessage(inputSequence.Error);
                }

                return;
            }

            //It's a valid message, so process it
                
            //Ignore if user is silenced
            //if (userData.Silenced == true)
            //{
            //    return;
            //}

            //Ignore based on user level and permissions
            //if (userData.Level < -1)//BotProgram.BotData.InputPermissions)
            //{
            //    BotProgram.MsgHandler.QueueMessage($"Inputs are restricted to levels {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above");
            //    return;
            //}

            #region Parser Post-Process Validation
            
            // All this validation is very slow
            // Find a way to speed it up, ideally without integrating it directly into the parser
            
            //Check if the user has permission to perform all the inputs they attempted
            //Also validate that the controller ports they're inputting for are valid
            //ParserPostProcess.InputValidation inputValidation = ParserPostProcess.CheckInputPermissionsAndPorts(userData.Level, inputSequence.Inputs,
            //    BotProgram.BotData.InputAccess.InputAccessDict);

            //If the input isn't valid, exit
            //if (inputValidation.IsValid == false)
            //{
            //    if (string.IsNullOrEmpty(inputValidation.Message) == false)
            //    {
            //        BotProgram.MsgHandler.QueueMessage(inputValidation.Message);
            //    }
            //    return;
            //}

            //Lastly, check for invalid button combos given the current console
            //if (BotProgram.BotData.InvalidBtnCombos.InvalidCombos.TryGetValue((int)InputGlobals.CurrentConsoleVal, out List<string> invalidCombos) == true)
            //{
            //    bool buttonCombosValidated = ParserPostProcess.ValidateButtonCombos(inputSequence.Inputs, invalidCombos);

            //    if (buttonCombosValidated == false)
            //    {
            //        string msg = "Invalid input: buttons ({0}) are not allowed to be pressed at the same time.";
            //        string combos = string.Empty;
            //        
            //        for (int i = 0; i < invalidCombos.Count; i++)
            //        {
            //            combos += "\"" + invalidCombos[i] + "\"";
            //            
            //            if (i < (invalidCombos.Count - 1))
            //            {
            //                combos += ", ";
            //            }
            //        }
            //        
            //        msg = string.Format(msg, combos);
            //        BotProgram.MsgHandler.QueueMessage(msg);
            //        
            //        return;
            //    }
            //}

            #endregion

            if (true)//InputHandler.StopRunningInputs == false)
            {
                EvtUserInputArgs userInputArgs = new EvtUserInputArgs()
                {
                    //UserData = e.UserData,
                    UsrMessage = e.UsrMessage,
                    ValidInputSeq = inputSequence
                };

                //Invoke input event
                UserMadeInputEvent?.Invoke(userInputArgs);
            }
            else
            {
                //BotProgram.MsgHandler.QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
            }
        }*/

        private /*async*/ void WaitForMainInitialization()
        {
            //Wait for the main program to initialize
            //while (false)//BotProgram.MsgHandler == null)
            //{
            //    await System.Threading.Tasks.Task.Delay(100);
            //}

            SetupStart();

            //Create a new thread to handle inputs through the console
            Thread consoleThread = new Thread(ReadWaitInputs);
            consoleThread.IsBackground = true;
            consoleThread.Start();
        }

        private void SetupStart()
        {
            EvtConnectedArgs conArgs = new EvtConnectedArgs
            {
                BotUsername = "terminalBot",
                AutoJoinChannel = string.Empty
            };

            OnConnectedEvent?.Invoke(conArgs);

            EvtJoinedChannelArgs joinedChannelArgs = new EvtJoinedChannelArgs
            {
                BotUsername = "terminalBot",
                Channel = string.Empty
            };

            OnJoinedChannelEvent?.Invoke(joinedChannelArgs);
        }

        private void ReadWaitInputs()
        {
            while (true)
            {
                if (StopConsoleThread == true)
                {
                    break;
                }

                //if (Console.KeyAvailable == false)
                //    continue;

                string line = Console.ReadLine();

                //User user = UserData;

                //Send message event
                EvtUserMessageArgs umArgs = new EvtUserMessageArgs()
                {
                    UsrMessage = new EvtUserMsgData("terminalUser", "terminalUser", "terminalUser",
                        string.Empty, line)
                };

                UserSentMessageEvent?.Invoke(umArgs);

                //Check for a command
                if (line.Length > 0 && line[0] == CommandIdentifier)
                {
                    //Build args list
                    List<string> argsList = new List<string>(line.Split(' '));
                    string argsAsStr = string.Empty;

                    //Remove the command itself and the space from the string
                    if (argsList.Count > 1)
                    {
                        argsAsStr = line.Remove(0, argsList[0].Length + 1);
                    }

                    //Remove command identifier
                    string cmdText = argsList[0].Remove(0, 1);
                    
                    //Now remove the first entry from the list, which is the command, retaining only the arguments
                    argsList.RemoveAt(0);

                    string userName = "terminalUser";

                    EvtChatCommandArgs chatcmdArgs = new EvtChatCommandArgs();
                    EvtUserMsgData msgData = new EvtUserMsgData(userName, userName, userName, string.Empty, line);

                    chatcmdArgs.Command = new EvtChatCommandData(argsList, argsAsStr, msgData, CommandIdentifier, cmdText);

                    ChatCommandReceivedEvent?.Invoke(chatcmdArgs);
                }
            }
        }
    }
}
