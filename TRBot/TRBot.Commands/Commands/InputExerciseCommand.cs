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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.Misc;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Handles input exercises for players.
    /// </summary>
    public sealed class InputExerciseCommand : BaseCommand
    {
        private const int MIN_INPUTS = 3;
        private const int MAX_INPUTS = 8;
        private const int MIN_SUB_SEQUENCES = 2;
        private const int MAX_SUB_SEQUENCES = 4;

        private const int MIN_SECONDS_VAL = 1;
        private const int MAX_SECONDS_VAL = 26;

        private const int MIN_PERCENT_VAL = 1;
        private const int MAX_PERCENT_VAL = 100;

        private const int BASE_CREDIT_REWARD = 100;
        private const int CREDIT_REWARD_MULTIPLIER = 3;
        private const double HARD_EXERCISE_MULTIPLIER = 1.3d;

        private const int MAX_PORTS = 8;

        private const string GENERATE_NEW_ARG = "new";
        private const string EASY_DIFFICULTY_ARG = "easy";
        private const string HARD_DIFFICULTY_ARG = "hard";

        private const string NO_EXERCISE_FOUND_MSG = "No input exercise found. Generate a new one with \"" + GENERATE_NEW_ARG + "\" as an argument to this command, optionally with a difficulty level: \"" + EASY_DIFFICULTY_ARG + "\" or \"" + HARD_DIFFICULTY_ARG + "\".";

        private const char SPLIT_CHAR = ',';

        private ConcurrentDictionary<string, InputExercise> UserExercises = null;
        private string[] InvalidExerciseInputNames = Array.Empty<string>();
        private readonly Random Rand = new Random();

        private string UsageMessage = "Usage: (\"new\" \"difficulty - easy (default) or hard\") for new exercise, or \"input sequence\" to input exercise";

        public InputExerciseCommand()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            if (UserExercises == null)
            {
                UserExercises = new ConcurrentDictionary<string, InputExercise>(Environment.ProcessorCount * 2, 32);
            }

            //Split inputs by what's in the value string
            if (string.IsNullOrEmpty(ValueStr) == false)
            {
                InvalidExerciseInputNames = ValueStr.Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                
                //Trim whitespace
                for (int i = 0; i < InvalidExerciseInputNames.Length; i++)
                {
                    InvalidExerciseInputNames[i] = InvalidExerciseInputNames[i].Trim();
                }
            }
        }

        public override void CleanUp()
        {
            if (UserExercises != null)
            {
                UserExercises.Clear();
                UserExercises = null;
            }

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;
            string userName = args.Command.ChatMessage.Username.ToLowerInvariant();

            using BotDBContext context = DatabaseManager.OpenContext();

            User user = DataHelper.GetUserNoOpen(userName, context);
            if (user == null)
            {
                QueueMessage("Huh, looks like you're not in the database!");
                return;
            }

            if (user.HasEnabledAbility(PermissionConstants.INPUT_EXERCISE_ABILITY) == false)
            {
                QueueMessage("You do not have the ability to use input exercises!");
                return;
            }

            //Get the last console used
            int lastConsoleID = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.LAST_CONSOLE, context, 1L);
            GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);
            GameConsole usedConsole = null;

            if (lastConsole != null)
            {
                //Create a new console using data from the database
                usedConsole = new GameConsole(lastConsole.Name, lastConsole.InputList, lastConsole.InvalidCombos);
            }

            //If there are no valid inputs, don't attempt to generate or solve
            if (usedConsole == null)
            {
                QueueMessage($"The current console does not point to valid data, making it impossible to solve or generate a new exercise. Please set a different console to use, or if none are available, add one.");
                return;
            }

            if (usedConsole.ConsoleInputs.Count == 0)
            {
                QueueMessage($"The current console, \"{usedConsole.Name}\", does not have any available inputs. Cannot determine solve or generate an exercise.");
                return;
            }

            string creditsName = DataHelper.GetCreditsNameNoOpen(context);
            int botCharLimit = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.BOT_MSG_CHAR_LIMIT, context, 500L);
            int defaultInputDur = (int)DataHelper.GetSettingIntNoOpen(SettingsConstants.DEFAULT_INPUT_DURATION, context, 200L);
            IQueryable<InputMacro> macros = context.Macros;

            ReverseParser.ReverseParserOptions parseOptions = new ReverseParser.ReverseParserOptions(ReverseParser.ShowPortTypes.None, (int)user.ControllerPort);

            //Handle no arguments
            if (arguments.Count == 0)
            {
                if (UserExercises.TryGetValue(user.Name, out InputExercise inputExercise) == true)
                {
                    OutputInputExercise(inputExercise, usedConsole, botCharLimit, creditsName, inputExercise.ParseOptions);
                }
                else
                {
                    QueueMessage(NO_EXERCISE_FOUND_MSG);
                }
                return;
            }

            //If "new" is specified, generate a new input sequence
            if ((arguments.Count == 1 || arguments.Count == 2) && arguments[0].ToLowerInvariant() == GENERATE_NEW_ARG)
            {
                //Check for a difficulty argument
                if (arguments.Count == 2)
                {
                    string difString = arguments[1].ToLowerInvariant();

                    //Check difficulty level
                    if (difString == EASY_DIFFICULTY_ARG)
                    {
                        parseOptions.ShowPortType = ReverseParser.ShowPortTypes.None;
                    }
                    else if (difString == HARD_DIFFICULTY_ARG)
                    {
                        parseOptions.ShowPortType = ReverseParser.ShowPortTypes.ShowNonDefaultPorts;
                    }
                    else
                    {
                        QueueMessage($"Invalid difficulty level specified. Please choose either \"{EASY_DIFFICULTY_ARG}\" or \"{HARD_DIFFICULTY_ARG}\".");
                        return;
                    }
                }

                //Generate the exercise 
                ParsedInputSequence newSequence = GenerateExercise(user.Level, defaultInputDur, usedConsole, parseOptions);

                //Give greater credit rewards for longer input sequences
                long creditReward = (newSequence.Inputs.Count * BASE_CREDIT_REWARD) * CREDIT_REWARD_MULTIPLIER;

                //This parser setting is set when performing a hard exercise 
                if (parseOptions.ShowPortType == ReverseParser.ShowPortTypes.ShowNonDefaultPorts)
                {
                    creditReward = (long)Math.Ceiling(creditReward * HARD_EXERCISE_MULTIPLIER);
                }

                InputExercise inputExercise = new InputExercise(newSequence, parseOptions, creditReward);
                UserExercises[user.Name] = inputExercise;

                OutputInputExercise(inputExercise, usedConsole, botCharLimit, creditsName, parseOptions);      
                return;
            }

            //Make sure the user has an exercise - if not, output the same message
            if (UserExercises.TryGetValue(user.Name, out InputExercise exercise) == false)
            {
                QueueMessage(NO_EXERCISE_FOUND_MSG);
                return;
            }

            //There's more than one argument and the user has an exercise; so it has to be the input
            //Let's validate it!
            if (ExecuteValidateInput(user, args.Command.ArgumentsAsString, usedConsole,
                creditsName, defaultInputDur, macros, parseOptions) == true)
            {
                //Grant credits if the user isn't opted out
                if (user.IsOptedOut == false)
                {
                    long creditReward = exercise.CreditReward;
                    user.Stats.Credits += creditReward;
                    context.SaveChanges();

                    QueueMessage($"Correct input! Awesome job! You've earned your {creditReward} {creditsName.Pluralize(false, creditReward)}!");
                }
                else
                {
                    QueueMessage("Correct input! Awesome job!");
                }

                //Remove the entry
                UserExercises.TryRemove(user.Name, out InputExercise value);
            }
        }

        private void OutputInputExercise(in InputExercise inputExercise, GameConsole console,
            in int botCharLimit, string creditsName, in ReverseParser.ReverseParserOptions options)
        {
            //Get the input in natural language
            string reverseSentence = ReverseParser.ReverseParseNatural(inputExercise.Sequence, console, options);
            
            reverseSentence += " | " + inputExercise.CreditReward + " " + creditsName + " reward.";

            QueueMessageSplit(reverseSentence, botCharLimit, ", ");

            //Add another message telling what to do
            QueueMessage($"Put your input as an argument to this command. To generate a new exercise, pass \"{GENERATE_NEW_ARG}\" as an argument.");
        }

        private bool ExecuteValidateInput(User user, string userInput, GameConsole console, string creditsName,
            in int defaultInputDur, IQueryable<InputMacro> macros, in ReverseParser.ReverseParserOptions options)
        {
            /* We don't need any parser post processing done here, as these inputs don't affect the game itself */
            
            //Console.WriteLine("USER COMMAND: " + userCommand);

            ParsedInputSequence inputSequence = default;

            try
            {
                string regexStr = console.InputRegex;

                string readyMessage = string.Empty;

                Parser parser = new Parser();

                //Prepare the message for parsing
                //Ignore input synonyms and max duration
                readyMessage = parser.PrepParse(userInput, macros, null);

                //Parse inputs to get our parsed input sequence
                inputSequence = parser.ParseInputs(readyMessage, regexStr, new ParserOptions(0, defaultInputDur, false, 0));
            }
            catch (Exception e)
            {
                inputSequence.ParsedInputResult = ParsedInputResults.Invalid;

                QueueMessage($"Sorry, I couldn't parse your input: {e.Message}");
                return false;
            }

            //Console.WriteLine("RESULT: " + inputSequence.ParsedInputResult);

            if (inputSequence.ParsedInputResult != ParsedInputResults.Valid)
            {
                if (string.IsNullOrEmpty(inputSequence.Error) == true)
                {
                    QueueMessage("Sorry, I couldn't parse your input.");
                }
                else
                {
                    QueueMessage($"Sorry, I couldn't parse your input: {inputSequence.Error}");
                }
                return false;
            }

            InputExercise currentExercise = UserExercises[user.Name];
            //Console.WriteLine("Correct: " + ReverseParser.ReverseParse(currentExercise.Sequence));
            List<List<ParsedInput>> exerciseInputs = currentExercise.Sequence.Inputs;

            List<List<ParsedInput>> userInputs = inputSequence.Inputs;

            //Compare input lengths - this has some downsides, but it's a quick check
            if (userInputs.Count != exerciseInputs.Count)
            {
                //Console.WriteLine($"COUNT DISPARITY {userInputs.Count} vs {exerciseInputs.Count}");

                QueueMessage("Incorrect input! Try again!");
                return false;
            }

            for (int i = 0; i < exerciseInputs.Count; i++)
            {
                List<ParsedInput> exerciseSubInputs = exerciseInputs[i];
                List<ParsedInput> userSubInputs = userInputs[i];

                if (exerciseSubInputs.Count != userSubInputs.Count)
                {
                    //Console.WriteLine($"SUBINPUT COUNT DISPARITY AT {i}: {userSubInputs.Count} vs {exerciseSubInputs.Count}");

                    QueueMessage("Incorrect input! Try again!");
                    return false;
                }

                //Now validate each input
                for (int j = 0; j < exerciseSubInputs.Count; j++)
                {
                    ParsedInput excInp = exerciseSubInputs[j];
                    ParsedInput userInp = userSubInputs[j];

                    //For simplicity for comparing, if the user put a blank input, use the same one all the time
                    if (console.IsBlankInput(userInp) == true)
                    {
                        userInp.name = "#";
                    }

                    if (excInp != userInp)
                    {
                        //Console.WriteLine($"FAILED COMPARISON ON: {userInp.ToString()} ===== CORRECT: {excInp.ToString()}");

                        QueueMessage("Incorrect input! Try again!");
                        return false;
                    }
                }
            }

            //No errors - the input should be correct!
            return true;
        }

        #region Exercise Generation

        private ParsedInputSequence GenerateExercise(in long userLevel, in int defaultInputDur, GameConsole console,
            in ReverseParser.ReverseParserOptions options)
        {
            int numInputs = Rand.Next(MIN_INPUTS, MAX_INPUTS);

            ParsedInputSequence inputSequence = default;
            inputSequence.ParsedInputResult = ParsedInputResults.Valid;
            List<List<ParsedInput>> exerciseInputs = new List<List<ParsedInput>>(numInputs);
            inputSequence.Inputs = exerciseInputs;

            List<string> heldInputs = new List<string>();

            for (int i = 0; i < numInputs; i++)
            {
                bool haveSubSequence = (Rand.Next(0, 2) == 0);
                int subSequences = (haveSubSequence == false) ? 1 : Rand.Next(MIN_SUB_SEQUENCES, MAX_SUB_SEQUENCES);

                List<ParsedInput> subSequence = GenerateSubSequence(subSequences, heldInputs, defaultInputDur, userLevel,
                    console, options);
                exerciseInputs.Add(subSequence);
            }

            return inputSequence;
        }

        private List<ParsedInput> GenerateSubSequence(in int numSubSequences, List<string> heldInputs,
            in int defaultInputDur, in long userLevel, GameConsole console, in ReverseParser.ReverseParserOptions options)
        {
            List<ParsedInput> subSequence = new List<ParsedInput>(numSubSequences);

            //Trim inputs that shouldn't be chosen in exercises
            //NOTE: To improve performance we should trim only at the highest level, not here in the subsequences
            List<InputData> validInputs = TrimInvalidExerciseInputs(userLevel, console.ConsoleInputs);

            //If there's more than one subsequence, remove blank inputs since they're redundant in this case
            if (numSubSequences > 1)
            {
                for (int i = validInputs.Count - 1; i >= 0; i--)
                {
                    if (validInputs[i].InputType == InputTypes.Blank)
                    {
                        validInputs.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < numSubSequences; i++)
            {
                //If we run out of valid inputs (Ex. consoles with very few buttons), get out of the loop
                if (validInputs.Count == 0)
                {
                    break;
                }

                ParsedInput input = ParsedInput.Default(defaultInputDur);

                int chosenInputIndex = Rand.Next(0, validInputs.Count);
                input.name = validInputs[chosenInputIndex].Name;

                input.duration = Rand.Next(MIN_SECONDS_VAL, MAX_SECONDS_VAL);
                bool useMilliseconds = (Rand.Next(0, 2) == 0);

                //If using milliseconds instead of seconds, multiply by 100 for more multiples of 100
                if (useMilliseconds == true)
                {
                    input.duration *= 100;
                    input.duration_type = Parser.DEFAULT_PARSE_REGEX_MILLISECONDS_INPUT;
                }
                else
                {
                    input.duration *= 1000;
                    input.duration_type = Parser.DEFAULT_PARSE_REGEX_SECONDS_INPUT;
                }

                //Decide whether to hold or release this input if it's not a wait input
                if (console.IsBlankInput(input) == false)
                {
                    bool holdRelease = (Rand.Next(0, 2) == 0);
                    if (holdRelease == true)
                    {
                        //If already held, release this input
                        if (heldInputs.Contains(input.name) == true)
                        {
                            input.release = true;
                            heldInputs.Remove(input.name);
                        }
                        else
                        {
                            input.hold = true;
                            heldInputs.Add(input.name);
                        }
                    }

                    //Randomize port if we should
                    if (options.ShowPortType != ReverseParser.ShowPortTypes.None)
                    {
                        int randPort = Rand.Next(0, MAX_PORTS);
                        input.controllerPort = randPort;
                    }
                }

                //Check for choosing a percent if the input is an axes
                if (console.IsAxis(input) == true)
                {
                    bool usePercent = (Rand.Next(0, 2) == 0);

                    if (usePercent == true)
                    {
                        input.percent = Rand.Next(MIN_PERCENT_VAL, MAX_PERCENT_VAL);
                    }
                }

                //Remove the input in the subsequence so it can't be chosen again, as it doesn't make sense
                //This prevents something like "a600ms+b400ms+a100ms" from happening
                validInputs.RemoveAt(chosenInputIndex);

                subSequence.Add(input);
            }

            return subSequence;
        }

        private List<InputData> TrimInvalidExerciseInputs(in long userLevel, Dictionary<string, InputData> validInputs)
        {
            List<InputData> inputs = new List<InputData>(validInputs.Values);

            for (int i = inputs.Count - 1; i >= 0; i--)
            {
                InputData inp = inputs[i];

                //Remove the input if the user doesn't have access to use it
                if (userLevel < inputs[i].Level)
                {
                    inputs.RemoveAt(i);
                    continue;
                }

                //Also remove any inputs with prohibited names
                for (int j = 0; j < InvalidExerciseInputNames.Length; j++)
                {
                    string invalidInputName = InvalidExerciseInputNames[j];
                    if (inp.Name == invalidInputName)
                    {
                        inputs.RemoveAt(i);
                        break;
                    }
                }
            }

            return inputs;
        }

        #endregion

        /// <summary>
        /// Represents an input exercise containing an input sequence and a credit reward for matching it.
        /// </summary>
        private struct InputExercise
        {
            public ParsedInputSequence Sequence;
            public ReverseParser.ReverseParserOptions ParseOptions;
            public long CreditReward;

            public InputExercise(in ParsedInputSequence sequence, in ReverseParser.ReverseParserOptions parseOptions, in long creditReward)
            {
                Sequence = sequence;
                ParseOptions = parseOptions;
                CreditReward = creditReward;
            }

            public override bool Equals(object obj)
            {
                if (obj is InputExercise inpExc)
                {
                    return (this == inpExc);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 11;
                    hash = (hash * 37) + Sequence.GetHashCode();
                    hash = (hash * 37) + CreditReward.GetHashCode();
                    hash = (hash * 37) + ParseOptions.GetHashCode();
                    return hash;
                }
            }

            public static bool operator==(InputExercise a, InputExercise b)
            {
                return (a.CreditReward == b.CreditReward && a.Sequence == b.Sequence && a.ParseOptions == b.ParseOptions);
            }

            public static bool operator!=(InputExercise a, InputExercise b)
            {
                return !(a == b);
            }
        }
    }
}
