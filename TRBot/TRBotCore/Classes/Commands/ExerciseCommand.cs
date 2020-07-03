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
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Handles input exercises for players.
    /// </summary>
    public sealed class ExerciseCommand : BaseCommand
    {
        private readonly ConcurrentDictionary<string, InputExercise> UserExercises = null;

        private const int MinInputs = 3;
        private const int MaxInputs = 8;
        private const int MinSubSequences = 2;
        private const int MaxSubSequences = 4;

        private const int MinSeconds = 1;
        private const int MaxSeconds = 26;

        private const int MinPercent = 1;
        private const int MaxPercent = 100;

        private const string GenerateNewArg = "new";
        private const int BaseCreditReward = 100;
        private const int CreditRewardMultiplier = 2;

        private readonly Random Rand = new Random();

        private readonly string[] InvalidExerciseInputs = new string[]
        {
            "ss1", "ss2", "ss3", "ss4", "ss5", "ss6", "ls1", "ls2", "ls3", "ls4", "ls5", "ls6",
            "."
        };

        public ExerciseCommand()
        {
            UserExercises = new ConcurrentDictionary<string, InputExercise>(Environment.ProcessorCount * 2, 32);
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;
            string userName = e.Command.ChatMessage.DisplayName.ToLowerInvariant();

            User user = BotProgram.GetUser(userName);
            if (user == null)
            {
                Console.WriteLine($"User {e.Command.ChatMessage.DisplayName} not in database.");
                return;
            }

            if (args.Count == 0)
            {
                ExecuteNoArgs(user);
                return;
            }

            //If "new" is specified, generate a new input sequence
            if (args.Count == 1 && args[0] == GenerateNewArg)
            {
                ExecuteNewArg(user);
                return;
            }

            //Make sure the user has an exercise - if not, output the same message
            if (UserExercises.ContainsKey(user.Name) == false)
            {
                ExecuteNoArgs(user);
                return;
            }

            //There's more than one argument and the user has an exercise; so it has to be the input
            //Let's validate it!
            ExecuteValidateInput(user, e.Command.ArgumentsAsString, InputGlobals.CurrentConsole);
        }

        private void ExecuteNoArgs(User user)
        {
            if (UserExercises.TryGetValue(user.Name, out InputExercise inputExercise) == true)
            {
                OutputInputExercise(inputExercise);
            }
            else
            {
                BotProgram.MsgHandler.QueueMessage($"No input exercise found. Generate a new one with \"{GenerateNewArg}\" as an argument to this command.");
            }
        }

        private void ExecuteNewArg(User user)
        {
            //Generate the exercise 
            Parser.InputSequence newSequence = GenerateExercise(user.Level, InputGlobals.CurrentConsole);

            //Give greater credit rewards for longer input sequences
            long creditReward = (newSequence.Inputs.Count * BaseCreditReward) * CreditRewardMultiplier;

            InputExercise inputExercise = new InputExercise(newSequence, creditReward);
            UserExercises[user.Name] = inputExercise;

            OutputInputExercise(inputExercise);            
        }

        private void OutputInputExercise(in InputExercise inputExercise)
        {
            //Put the credit reward in a different message if necessary
            //There's a chance the sentence may use most of Twitch's character limit
            string reverseSentence = ReverseParser.ReverseParseNatural(inputExercise.Sequence);
            
            string creditRewardMsg = inputExercise.CreditReward + " credit reward.";
            int totalMsgLength = reverseSentence.Length + creditRewardMsg.Length + 3;

            if (totalMsgLength > BotProgram.BotSettings.BotMessageCharLimit)
            {
                BotProgram.MsgHandler.QueueMessage(reverseSentence);
                BotProgram.MsgHandler.QueueMessage(creditRewardMsg);
            }
            else
            {
                BotProgram.MsgHandler.QueueMessage(reverseSentence + " | " + creditRewardMsg);
            }

            //Add another message for what to do
            BotProgram.MsgHandler.QueueMessage($"Put your input as an argument to this command. To generate a new exercise, pass \"{GenerateNewArg}\" as an argument.");
        }

        private void ExecuteValidateInput(User user, string userCommand, ConsoleBase currentConsole)
        {
            /* We don't need any parser post processing done here, as these inputs don't affect the game itself */
            
            Parser.InputSequence inputSequence = default;

            try
            {
                //Ignore max duration and synonyms
                string parse_message = Parser.Expandify(Parser.PopulateMacros(userCommand));
                inputSequence = Parser.ParseInputs(parse_message, 0, false, false);
            }
            catch
            {
                BotProgram.MsgHandler.QueueMessage("Sorry, I couldn't parse your input.");
                return;
            }

            if (inputSequence.InputValidationType != Parser.InputValidationTypes.Valid)
            {
                BotProgram.MsgHandler.QueueMessage("Sorry, I couldn't parse your input.");
                return;
            }

            InputExercise currentExercise = UserExercises[user.Name];
            List<List<Parser.Input>> exerciseInputs = currentExercise.Sequence.Inputs;

            List<List<Parser.Input>> userInputs = inputSequence.Inputs;

            //Compare input lengths - this has some downsides, but it's a quick check
            if (userInputs.Count != exerciseInputs.Count)
            {
                BotProgram.MsgHandler.QueueMessage("Incorrect input! Try again!");
                return;
            }

            for (int i = 0; i < exerciseInputs.Count; i++)
            {
                List<Parser.Input> exerciseSubInputs = exerciseInputs[i];
                List<Parser.Input> userSubInputs = userInputs[i];

                if (exerciseSubInputs.Count != userSubInputs.Count)
                {
                    BotProgram.MsgHandler.QueueMessage("Incorrect input! Try again!");
                    return;
                }

                //Now validate each input
                for (int j = 0; j < exerciseSubInputs.Count; j++)
                {
                    Parser.Input excInp = exerciseSubInputs[j];
                    Parser.Input userInp = userSubInputs[j];

                    //For simplicity for comparing, if the user put a wait input, use the same one all the time
                    if (currentConsole.IsWait(userInp) == true)
                    {
                        userInp.name = "#";
                    }

                    if (CompareInputs(excInp, userInp) == false)
                    {
                        BotProgram.MsgHandler.QueueMessage("Incorrect input! Try again!");
                        return;
                    }
                }
            }

            //No errors - the input should be correct!
            if (user.OptedOut == false)
            {
                long creditReward = currentExercise.CreditReward;
                user.AddCredits(creditReward);

                BotProgram.SaveBotData();
                BotProgram.MsgHandler.QueueMessage($"Correct input! Awesome job! You've earned your {creditReward} credits!");
            }
            else
            {
                BotProgram.MsgHandler.QueueMessage("Correct input! Awesome job!");
            }

            //Remove the entry
            UserExercises.TryRemove(user.Name, out InputExercise value);
        }

        private bool CompareInputs(in Parser.Input input1, in Parser.Input input2)
        {
            return (input1.hold == input2.hold
                    && input1.release == input2.release
                    && input1.name == input2.name
                    && input1.percent == input2.percent
                    && input1.duration == input2.duration
                    && input1.duration_type == input2.duration_type);
        }

        #region Exercise Generation

        private Parser.InputSequence GenerateExercise(in int userLevel, ConsoleBase currentConsole)
        {
            int numInputs = Rand.Next(MinInputs, MaxInputs);

            Parser.InputSequence inputSequence = default;
            inputSequence.InputValidationType = Parser.InputValidationTypes.Valid;
            List<List<Parser.Input>> exerciseInputs = new List<List<Parser.Input>>(numInputs);
            inputSequence.Inputs = exerciseInputs;

            List<string> heldInputs = new List<string>();

            for (int i = 0; i < numInputs; i++)
            {
                bool haveSubSequence = (Rand.Next(0, 2) == 0);
                int subSequences = (haveSubSequence == false) ? 1 : Rand.Next(MinSubSequences, MaxSubSequences);

                List<Parser.Input> subSequence = GenerateSubSequence(subSequences, heldInputs, userLevel, currentConsole);
                exerciseInputs.Add(subSequence);
            }

            return inputSequence;
        }

        private List<Parser.Input> GenerateSubSequence(in int numSubSequences, List<string> heldInputs, in int userLevel, ConsoleBase currentConsole)
        {
            List<Parser.Input> subSequence = new List<Parser.Input>(numSubSequences);

            //Trim inputs that shouldn't be chosen in exercises
            //NOTE: To improve performance we should trim only at the highest level, not here in the subsequences
            List<string> validInputs = TrimInvalidExerciseInputs(userLevel, currentConsole.ValidInputs);

            //If there's more than one subsequence, remove wait inputs since they're largely redundant in this case
            if (numSubSequences > 1)
            {
                validInputs.Remove("#");
                validInputs.Remove(".");
            }

            for (int i = 0; i < numSubSequences; i++)
            {
                //If we run out of valid inputs (Ex. consoles with very few buttons), get out of the loop
                if (validInputs.Count == 0)
                {
                    break;
                }
                
                Parser.Input input = Parser.Input.Default;

                int chosenInputIndex = Rand.Next(0, validInputs.Count);
                input.name = validInputs[chosenInputIndex];

                input.duration = Rand.Next(MinSeconds, MaxSeconds);
                bool useMilliseconds = (Rand.Next(0, 2) == 0);

                //If using milliseconds instead of seconds, multiply by 100 for more multiples of 100
                if (useMilliseconds == true)
                {
                    input.duration *= 100;
                    input.duration_type = Parser.ParseRegexMillisecondsInput;
                }
                else
                {
                    input.duration *= 1000;
                    input.duration_type = Parser.ParseRegexSecondsInput;
                }

                //Decide whether to hold or release this input if it's not a wait input
                if (currentConsole.IsWait(input) == false)
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
                }

                //Check for choosing a percent if the input is an axes
                if (currentConsole.IsAxis(input) == true 
                    || currentConsole.IsAbsoluteAxis(input) == true)
                {
                    bool usePercent = (Rand.Next(0, 2) == 0);

                    if (usePercent == true)
                    {
                        input.percent = Rand.Next(MinPercent, MaxPercent);
                    }
                }

                //Remove the input in the subsequence so it can't be chosen again, as it doesn't make sense
                //This prevents something like "a600ms+b400ms+a100ms" from happening
                validInputs.RemoveAt(chosenInputIndex);

                subSequence.Add(input);
            }

            return subSequence;
        }

        private List<string> TrimInvalidExerciseInputs(in int userLevel, string[] validInputs)
        {
            List<string> inputs = new List<string>(validInputs);
            for (int i = 0; i < InvalidExerciseInputs.Length; i++)
            {
                inputs.Remove(InvalidExerciseInputs[i]);
            }

            for (int i = inputs.Count - 1; i >= 0; i--)
            {
                int permLvl = 0;

                //NOTE: Try to decouple this dictionary from here
                if (BotProgram.BotData.InputAccess?.InputAccessDict?.TryGetValue(inputs[i], out permLvl) == false)
                {
                    continue;
                }

                //Remove the input if the user doesn't have access to use it
                if (userLevel < permLvl)
                {
                    inputs.RemoveAt(i);
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
            public Parser.InputSequence Sequence;
            public long CreditReward;

            public InputExercise(in Parser.InputSequence sequence, in long creditReward)
            {
                Sequence = sequence;
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
                    return hash;
                }
            }

            public static bool operator==(InputExercise a, InputExercise b)
            {
                return (a.CreditReward == b.CreditReward && a.Sequence == b.Sequence);
            }

            public static bool operator!=(InputExercise a, InputExercise b)
            {
                return !(a == b);
            }
        }
    }
}
