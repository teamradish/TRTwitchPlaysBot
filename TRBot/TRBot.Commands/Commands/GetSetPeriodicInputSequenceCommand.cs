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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Data;
using TRBot.Permissions;
using TRBot.Consoles;
using TRBot.Parsing;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that changes the periodic input sequence.
    /// </summary>
    public sealed class GetSetPeriodicInputSequenceCommand : BaseCommand
    {
        private string UsageMessage = $"Usage - no arguments (get value) or \"input sequence (string)\"";

        public GetSetPeriodicInputSequenceCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string userName = args.Command.ChatMessage.Username;
            string argInputSequence = args.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(argInputSequence) == true)
            {
                string periodicInputSequence = DataHelper.GetSettingString(SettingsConstants.PERIODIC_INPUT_VALUE, string.Empty);
                if (string.IsNullOrEmpty(periodicInputSequence) == true)
                {
                    QueueMessage("The current periodic input sequence is not defined! You can define one by providing an input sequence as an argument."); 
                }
                
                int botCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

                QueueMessageSplit($"The current periodic input sequence is: {periodicInputSequence}", botCharLimit, string.Empty); 
                return;
            }

            //Replace all non-space whitespace in the argument with space for readability
            //Some platforms will do this by default (Ex. Twitch), but we can't rely on them all being consistent here
            argInputSequence = Helpers.ReplaceAllWhitespaceWithSpace(argInputSequence);

            long globalInputPermLevel = DataHelper.GetSettingInt(SettingsConstants.GLOBAL_INPUT_LEVEL, 0L);
            
            //Consider the user's level so they cannot use this to perform inputs normally unavailable to them
            long userLevel = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                //Check for permissions
                if (user == null || user.HasEnabledAbility(PermissionConstants.SET_PERIODIC_INPUT_SEQUENCE_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to set the periodic input sequence!");
                    return;
                }

                //Check if the user is silenced
                if (user.HasEnabledAbility(PermissionConstants.SILENCED_ABILITY) == true)
                {
                    QueueMessage("Nice try, but you can't set the periodic input sequence while silenced!");
                    return;
                }
            
                //Ignore based on user level and permissions
                if (user.Level < globalInputPermLevel)
                {
                    QueueMessage($"Inputs are restricted to levels {(PermissionLevels)globalInputPermLevel} and above, so you can't set the periodic input sequence.");
                    return;
                }

                userLevel = user.Level;
            }

            //Parse the input sequence
            //Set up the console
            GameConsole usedConsole = null;

            int lastConsoleID = (int)DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);

                if (lastConsole != null)
                {
                    //Create a new console using data from the database
                    usedConsole = new GameConsole(lastConsole.Name, lastConsole.InputList, lastConsole.InvalidCombos);
                }
            }

            //If there are no valid inputs, don't attempt to parse
            if (usedConsole == null)
            {
                DataContainer.MessageHandler.QueueMessage("The current console does not point to valid data. Please set a different console to use, or if none are available, add one.");
                return;
            }

            if (usedConsole.ConsoleInputs.Count == 0)
            {
                DataContainer.MessageHandler.QueueMessage($"The current console, \"{usedConsole.Name}\", does not have any available inputs.");
                return;
            }

            int defaultDur = (int)DataHelper.GetUserOrGlobalDefaultInputDur(string.Empty);
            int maxDur = (int)DataHelper.GetUserOrGlobalMaxInputDur(string.Empty);
            int defaultPeriodicInpPort = (int)DataHelper.GetSettingInt(SettingsConstants.PERIODIC_INPUT_PORT, 0L);

            ParsedInputSequence inputSequence = default;

            try
            {
                string regexStr = usedConsole.InputRegex;

                string readyMessage = string.Empty;

                Parser parser = new Parser();

                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    //Get input synonyms for this console
                    IQueryable<InputSynonym> synonyms = context.InputSynonyms.Where(syn => syn.ConsoleID == lastConsoleID);

                    //Prepare the message for parsing
                    readyMessage = parser.PrepParse(argInputSequence, context.Macros, synonyms);
                }

                //Parse inputs to get our parsed input sequence
                inputSequence = parser.ParseInputs(readyMessage, regexStr, new ParserOptions(defaultPeriodicInpPort, defaultDur, true, maxDur));
            }
            catch (Exception exception)
            {
                string excMsg = exception.Message;

                //Handle parsing exceptions
                inputSequence.ParsedInputResult = ParsedInputResults.Invalid;

                DataContainer.MessageHandler.QueueMessage($"Couldn't parse periodic input sequence: {excMsg}");
                return;
            }

            //Check for non-valid messages
            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid)
            {
                string message = inputSequence.Error;
                if (string.IsNullOrEmpty(message) == true)
                {
                    message = "Input is invalid";
                }

                DataContainer.MessageHandler.QueueMessage($"Cannot set periodic input sequence: {message}");
                return;
            }

            /* Perform some post-processing ahead of time since this input will be carried out */
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User user = DataHelper.GetUserNoOpen(userName, context);

                //Check for restricted inputs on this user
                InputValidation inpValidation = ParserPostProcess.InputSequenceContainsRestrictedInputs(inputSequence,
                    user.GetRestrictedInputs());

                if (inpValidation.InputValidationType != InputValidationTypes.Valid)
                {
                    if (string.IsNullOrEmpty(inpValidation.Message) == false)
                    {
                        QueueMessage(inpValidation.Message);
                    }
                    return;
                }
            }

            //Check for level permissions and ports
            InputValidation validation = ParserPostProcess.ValidateInputLvlPermsAndPorts(userLevel, inputSequence,
                DataContainer.ControllerMngr, usedConsole.ConsoleInputs);
            if (validation.InputValidationType != InputValidationTypes.Valid)
            {
                if (string.IsNullOrEmpty(validation.Message) == false)
                {
                    QueueMessage(validation.Message);
                }

                return;
            }

            //Defer input combo validation to when it's about to be be executed
            //This way, the state of the virtual controllers now shouldn't
            //affect an otherwise valid combo from being accepted

            string newPeriodicInpSequence = ReverseParser.ReverseParse(inputSequence, usedConsole,
                new ReverseParser.ReverseParserOptions(ReverseParser.ShowPortTypes.ShowNonDefaultPorts,
                    defaultPeriodicInpPort, ReverseParser.ShowDurationTypes.ShowNonDefaultDurations, defaultDur));

            /*
             * Finally, after everything is good, set the input sequence!
            */
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings periodicInputSeqSetting = DataHelper.GetSettingNoOpen(SettingsConstants.PERIODIC_INPUT_VALUE, context);
                periodicInputSeqSetting.ValueStr = newPeriodicInpSequence;

                context.SaveChanges();
            }

            QueueMessage("Successfully set the periodic input sequence!");
        }
    }
}
