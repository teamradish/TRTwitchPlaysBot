/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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

namespace TRBot.Routines
{
    /// <summary>
    /// A routine handling inputs for democracy input modes.
    /// </summary>
    public class InputModeVoteRoutine : BaseRoutine
    {
        /// <summary>
        /// The duration of the voting period.
        /// </summary>
        public long VotingDuration { get; private set; } = 0L;

        private DateTime StartVotingTime = default;
        
        /// <summary>
        /// A dictionary containing the users that voted and which input mode they voted for.
        /// </summary>
        private ConcurrentDictionary<string, InputModes> UsersVoted = new ConcurrentDictionary<string, InputModes>(Environment.ProcessorCount, 64);

        /// <summary>
        /// A dictionary containing the input modes and how many votes each one has.
        /// </summary>
        private ConcurrentDictionary<InputModes, long> VotesPerMode = new ConcurrentDictionary<InputModes, long>(Environment.ProcessorCount, 64);
        
        public bool TallyingCommenced { get; private set; } = false;

        public InputModeVoteRoutine(in long votingDuration)
        {
            Identifier = RoutineConstants.INPUT_MODE_VOTE_ROUTINE_ID;
            
            VotingDuration = votingDuration;
        }

        public override void Initialize()
        {
            base.Initialize();

            DateTime nowUTC = DateTime.UtcNow;
            StartVotingTime = nowUTC;
        }

        /// <summary>
        /// Returns the current votes per input mode in a new dictionary.
        /// </summary>
        /// <returns>A dictionary containing the current number of votes per input mode.</returns>
        public Dictionary<InputModes, long> GetVotesPerMode()
        {
            return new Dictionary<InputModes, long>(VotesPerMode);
        }

        public void AddModeVote(string userName, in InputModes vote)
        {
            //Add a vote for this user
            if (UsersVoted.TryGetValue(userName, out InputModes prevVotedMode) == false)
            {
                UsersVoted[userName] = vote;
            }
            //Change the user's vote
            else
            {
                //Voting for the same mode, so return since there's nothing to do
                if (prevVotedMode == vote)
                {
                    return;
                }

                //Subtract 1 from the previous mode's vote
                VotesPerMode[prevVotedMode] -= 1;
            }

            //Add this mode to the list if it's not present
            if (VotesPerMode.TryGetValue(vote, out long count) == false)
            {
                if (VotesPerMode.TryAdd(vote, 1L) == false)
                {
                    Console.WriteLine($"Unable to add vote. Input Mode: {vote} | Name: {userName}"); 
                    return;
                }
            }
            else
            {
                //Increment the vote by one
                count += 1;
                VotesPerMode[vote] = count;
            }            
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            //Get time difference and time remaining
            TimeSpan diff = currentTimeUTC - StartVotingTime;

            //Not enough time passed - return
            if (diff.TotalMilliseconds < VotingDuration)
            {
                return;
            }

            //Count the votes
            TallyingCommenced = true;

            InputModes chosenMode = default;
            long highestCount = -1;

            foreach (KeyValuePair<InputModes, long> kvPair in VotesPerMode)
            {
                long count = kvPair.Value;

                if (count > highestCount)
                {
                    chosenMode = kvPair.Key;
                    highestCount = count;
                }
            }

            //Compare the new mode with the current one
            InputModes curMode = (InputModes)DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE, (long)InputModes.Anarchy);

            long votingCooldown = DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE_CHANGE_COOLDOWN, 1000L * 60L * 15L);

            //Change the mode since it's different
            if (curMode != chosenMode)
            {
                //Save the new value in the database
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    Settings inpModeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.INPUT_MODE, context);
                    inpModeSetting.ValueInt = (long)chosenMode;

                    context.SaveChanges();
                }

                //If we set it to Anarchy, check if the Democracy routine is active and remove it if so
                if (chosenMode == InputModes.Anarchy)
                {
                    BaseRoutine democracyRoutine = RoutineHandler.FindRoutine(RoutineConstants.DEMOCRACY_ROUTINE_ID, out int indexFound);
                    if (democracyRoutine != null)
                    {
                        RoutineHandler.RemoveRoutine(indexFound);
                    }
                }
                //If we set it to Democracy, add the routine if it's not already active
                else if (chosenMode == InputModes.Democracy)
                {
                    DemocracyRoutine democracyRoutine = RoutineHandler.FindRoutine<DemocracyRoutine>();

                    if (democracyRoutine == null)
                    {
                        long votingTime = DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_VOTE_TIME, 10000L);

                        democracyRoutine = new DemocracyRoutine(votingTime);
                        RoutineHandler.AddRoutine(democracyRoutine);
                    }
                }
            }

            //Set the next voting time
            DateTime nextVoteAvailable = currentTimeUTC + TimeSpan.FromMilliseconds(votingCooldown);
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                Settings lastCompletionSetting = DataHelper.GetSettingNoOpen(SettingsConstants.INPUT_MODE_NEXT_VOTE_DATE, context);

                lastCompletionSetting.ValueStr = DataHelper.GetStrFromDateTime(nextVoteAvailable);
                context.SaveChanges();
            }

            string voteStr = "vote".Pluralize(false, highestCount);

            DataContainer.MessageHandler.QueueMessage($"Voting has ended and {chosenMode} came out on top with {highestCount} {voteStr}! The next round of voting to change the input mode can start again at {nextVoteAvailable}.");

            TallyingCommenced = false;
            StartVotingTime = currentTimeUTC;

            //Remove the routine
            RoutineHandler.RemoveRoutine(Identifier);
        }
    }
}
