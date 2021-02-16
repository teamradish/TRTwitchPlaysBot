﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Connection;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.Misc;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Logging;

namespace TRBot.Routines
{
    /// <summary>
    /// A routine handling inputs for democracy input modes.
    /// </summary>
    public class DemocracyRoutine : BaseRoutine
    {
        /// <summary>
        /// The duration of the voting period.
        /// </summary>
        public long VotingDuration { get; private set; } = 0L;

        private DateTime NextInputTime = default;
        
        /// <summary>
        /// Inputs sent by each user. These must be valid inputs run through post-process methods.
        /// The key is the input list and the value is the count.
        /// </summary>
        private ConcurrentDictionary<List<List<ParsedInput>>, long> AllInputs = new ConcurrentDictionary<List<List<ParsedInput>>, long>(Environment.ProcessorCount, 64, new InputListComparer());

        public DemocracyRoutine(in long votingDuration)
        {
            Identifier = RoutineConstants.DEMOCRACY_ROUTINE_ID;
            SetVoteDuration(votingDuration);
        }

        public void SetVoteDuration(in long votingDuration)
        {
            VotingDuration = votingDuration;
        }

        public override void Initialize()
        {
            base.Initialize();

            DateTime nowUTC = DateTime.UtcNow;
            NextInputTime = nowUTC;

            InputHandler.InputsHaltedEvent -= OnInputsHalted;
            InputHandler.InputsHaltedEvent += OnInputsHalted;

            DataContainer.DataReloader.SoftDataReloadedEvent -= OnDataReload;
            DataContainer.DataReloader.SoftDataReloadedEvent += OnDataReload;

            DataContainer.DataReloader.HardDataReloadedEvent -= OnDataReload;
            DataContainer.DataReloader.HardDataReloadedEvent += OnDataReload;
        }

        public override void CleanUp()
        {
            InputHandler.InputsHaltedEvent -= OnInputsHalted;
            DataContainer.DataReloader.SoftDataReloadedEvent -= OnDataReload;
            DataContainer.DataReloader.HardDataReloadedEvent -= OnDataReload;

            base.CleanUp();
        }

        private void OnInputsHalted()
        {
            //Clear all inputs when halted
            //Inputs are often halted when switching consoles, virtual controller count, and virtual controller type
            AllInputs.Clear();
        }

        private void OnDataReload()
        {
            //Update voting time to the new value
            VotingDuration = DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_VOTE_TIME, 10000L);
        }

        /// <summary>
        /// Adds an input sequence to be considered for voting.
        /// </summary>
        public void AddInputSequence(string userName, List<List<ParsedInput>> inputList)
        {
            if (inputList == null || inputList.Count == 0
                || inputList[0] == null || inputList[0].Count == 0)
            {
                TRBotLogger.Logger.Error($"Invalid input list added by {userName}!");
                return;
            }

            if (AllInputs.TryGetValue(inputList, out long count) == false)
            {
                AllInputs[inputList] = 1;
            }
            else
            {
                count += 1;
                AllInputs[inputList] = count;
            }

            //TRBotLogger.Logger.Information($"Added input sequence for user {userName}. Count: {AllInputs[inputList]} | Total: {AllInputs.Count}");
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            //Update the time if we have no inputs
            //This refreshes the duration so inputs consistently go through
            if (AllInputs.Count == 0)
            {
                NextInputTime = currentTimeUTC;
                return;
            }

            //Get time difference and time remaining
            TimeSpan diff = currentTimeUTC - NextInputTime;

            //Not enough time passed - return
            if (diff.TotalMilliseconds < VotingDuration)
            {
                return;
            }

            //Update next input time
            NextInputTime = currentTimeUTC;

            DemocracyResolutionModes resolutionMode = (DemocracyResolutionModes)DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_RESOLUTION_MODE, 0L);

            //Make sure the given resolution mode is valid
            if (resolutionMode < 0 || resolutionMode > DemocracyResolutionModes.ExactInput)
            {
                DataContainer.MessageHandler.QueueMessage($"Democracy resolution mode is invalid! Setting resolution mode to {DemocracyResolutionModes.ExactSequence}");
                
                resolutionMode = DemocracyResolutionModes.ExactSequence;

                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    Settings demResSetting = DataHelper.GetSettingNoOpen(SettingsConstants.DEMOCRACY_RESOLUTION_MODE, context);
                    if (demResSetting == null)
                    {
                        demResSetting = new Settings(SettingsConstants.DEMOCRACY_RESOLUTION_MODE, string.Empty, 0L);
                    }

                    demResSetting.ValueInt = (long)DemocracyResolutionModes.ExactSequence;

                    context.SaveChanges();
                }
            }

            //TRBotLogger.Logger.Information($"Resolution mode: {resolutionMode}");
            //TRBotLogger.Logger.Information($"Passed voting duration of {VotingDuration}");

            List<List<ParsedInput>> executedInputList = null;

            if (resolutionMode == DemocracyResolutionModes.SameName)
            {
                executedInputList = ResolveSameName();
            }
            else if (resolutionMode == DemocracyResolutionModes.ExactInput)
            {
                executedInputList = ResolveExactInput();
            }
            else
            {
                executedInputList = ResolveExactSequence();
            }

            //Clear all inputs in our main list now that they're in the dictionary by count
            AllInputs.Clear();

            //Something happened while resolving inputs, so return
            //We should have printed a detailed message beforehand
            if (executedInputList == null)
            {
                return;
            }

            //Get the current console
            int lastConsoleID = (int)DataHelper.GetSettingInt(SettingsConstants.LAST_CONSOLE, 1L);
            GameConsole usedConsole = null;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                GameConsole lastConsole = context.Consoles.FirstOrDefault(c => c.ID == lastConsoleID);

                if (lastConsole == null)
                {
                    DataContainer.MessageHandler.QueueMessage("The current console is somehow null! Cannot execute input.");
                    return;
                }

                usedConsole = new GameConsole(lastConsole.Name, lastConsole.InputList, lastConsole.InvalidCombos);
            }

            //Make sure inputs aren't stopped
            if (InputHandler.InputsHalted == true)
            {
                //We can't process inputs because they're currently stopped
                DataContainer.MessageHandler.QueueMessage("New inputs cannot be processed until all other inputs have stopped.");
                return;
            }

            /*************************
            * Execute the input now! *
            *************************/

            InputHandler.CarryOutInput(executedInputList, usedConsole, DataContainer.ControllerMngr);
        }

        private List<List<ParsedInput>> ResolveExactSequence()
        {
            //ExactSequence resolution: Choose the exact input sequence with the most votes

            List<List<ParsedInput>> chosenList = null;
            long maxCount = -1;

            foreach (KeyValuePair<List<List<ParsedInput>>, long> kvPair in AllInputs)
            {
                if (kvPair.Value > maxCount)
                {
                    chosenList = kvPair.Key;
                    maxCount = kvPair.Value;
                }
            }

            return chosenList;
        }

        private List<List<ParsedInput>> ResolveSameName()
        {
            //SameName resolution: The first input in each sequence is considered, and the end duration is the default
            //The first one found with the max value is chosen in the event of a tie
            
            //Find the highest count and take the first input name matching it
            string chosenInputName = string.Empty;
            long maxCount = -1;

            //Create a new dictionary to store the input names and their counts
            //We can't rely only on the count from the original dictionary since it
            //records only completely unique input sequences
            Dictionary<string, long> inputNames = new Dictionary<string, long>(AllInputs.Count);

            foreach (KeyValuePair<List<List<ParsedInput>>, long> kvPair in AllInputs)
            {
                string inputName = kvPair.Key[0][0].Name;

                //TRBotLogger.Logger.Information($"Iterating through input \"{inputName}\"");

                if (inputNames.TryGetValue(inputName, out long count) == false)
                {
                    //Set the count to the original count to account for identical entries
                    count = kvPair.Value;
                    inputNames.Add(inputName, count);

                    //TRBotLogger.Logger.Information($"Added input \"{inputName}\"");
                }
                else
                {
                    //Add the count from the dictionary
                    //This way we account for identical entries (Ex. three separate "a" inputs)
                    count += kvPair.Value;
                    inputNames[inputName] = count;

                    //TRBotLogger.Logger.Information($"Input name \"{inputName}\" now has count of {inputNames[inputName]}");
                }
                
                //Check for a greater count
                //If we surpass it, this is the new max
                if (count > maxCount)
                {
                    chosenInputName = inputName;
                    maxCount = count;

                    //TRBotLogger.Logger.Information($"Found greater: \"{chosenInputName}\" with count of {maxCount}");
                }
            }

            //Get global default input duration
            int defaultDur = (int)DataHelper.GetSettingInt(SettingsConstants.DEFAULT_INPUT_DURATION, 200L);

            ParsedInput pressedInput = ParsedInput.Default(defaultDur);
            pressedInput.Name = chosenInputName;

            List<List<ParsedInput>> executedInputList = new List<List<ParsedInput>>(1);
            executedInputList.Add(new List<ParsedInput>(1) { pressedInput });

            //TRBotLogger.Logger.Information($"Carrying out: {pressedInput.ToString()}");

            return executedInputList;
        }

        private List<List<ParsedInput>> ResolveExactInput()
        {
            //ExactInput resolution: The first input in each sequence is considered
            //The first one found with the max value is chosen in the event of a tie

            //Find the highest count
            ParsedInput chosenInput = default;
            long maxCount = -1;

            //Create a new dictionary to store the first input and their counts
            //We can't rely only on the count from the original dictionary since it
            //records only completely unique input sequences
            Dictionary<ParsedInput, long> inputDict = new Dictionary<ParsedInput, long>(AllInputs.Count);

            foreach (KeyValuePair<List<List<ParsedInput>>, long> kvPair in AllInputs)
            {
                //Take only the first input
                ParsedInput input = kvPair.Key[0][0];

                //TRBotLogger.Logger.Information($"Iterating through input \"{input}\"");

                if (inputDict.TryGetValue(input, out long count) == false)
                {
                    //Set the count to the original count to account for identical entries
                    count = kvPair.Value;
                    inputDict.Add(input, count);

                    //TRBotLogger.Logger.Information($"Added input {input}");
                }
                else
                {
                    //Add the count from the dictionary
                    //This way we account for identical entries
                    count += kvPair.Value;
                    inputDict[input] = count;

                    //TRBotLogger.Logger.Information($"Input {input} now has count of {inputDict[input]}");
                }
                
                //Check for a greater count
                //If we surpass it, this is the new max
                if (count > maxCount)
                {
                    chosenInput = input;
                    maxCount = count;

                    //TRBotLogger.Logger.Information($"Found greater: {chosenInput} with count of {maxCount}");
                }
            }

            List<List<ParsedInput>> executedInputList = new List<List<ParsedInput>>(1);
            executedInputList.Add(new List<ParsedInput>(1) { chosenInput });

            //TRBotLogger.Logger.Information($"Carrying out: {chosenInput}");

            return executedInputList;
        }

        /// <summary>
        /// An <see cref="IEqualityComparer{T}" /> for an input sequence's input list. This compares the length and contents of the lists.
        /// </summary>
        private class InputListComparer : IEqualityComparer<List<List<ParsedInput>>>
        {
            public bool Equals(List<List<ParsedInput>> inputList1, List<List<ParsedInput>> inputList2)
            {
                //Both null - equal
                if (inputList1 == null && inputList2 == null)
                {
                    return true;
                }

                //One is null and the other isn't - not equal
                if ((inputList1 == null && inputList2 != null) 
                    || (inputList1 != null && inputList2 == null))
                {
                    return false;
                }

                //Check for count
                if (inputList1.Count != inputList2.Count)
                {
                    return false;
                }

                for (int i = 0; i < inputList1.Count; i++)
                {
                    List<ParsedInput> inpList1 = inputList1[i];
                    List<ParsedInput> inpList2 = inputList2[i];

                    //Check for count
                    if (inpList1.Count != inpList2.Count)
                    {
                        return false;
                    }

                    //Compare each individual parsed input in each list
                    for (int j = 0; j < inpList1.Count; j++)
                    {
                        ParsedInput inp1 = inpList1[j];
                        ParsedInput inp2 = inpList2[j];

                        if (inp1 != inp2)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            public int GetHashCode(List<List<ParsedInput>> inputList)
            {
                unchecked
                {
                    //Get the combined hash of each input
                    int hash = 0;
                    for (int i = 0; i < inputList.Count; i++)
                    {
                        List<ParsedInput> inpList = inputList[i];
                        for (int j = 0; j < inpList.Count; j++)
                        {
                            hash += inpList[j].GetHashCode();
                        }
                    }

                    return hash;
                }
            }
        }
    }
}
