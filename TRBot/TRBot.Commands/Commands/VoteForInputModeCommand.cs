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
using System.Text;
using TRBot.Connection;
using TRBot.VirtualControllers;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Misc;
using TRBot.Permissions;
using TRBot.Routines;

namespace TRBot.Commands
{
    /// <summary>
    /// Votes for a new input mode to take over.
    /// </summary>
    public sealed class VoteForInputModeCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"input mode (string/int)\"";
        private string CachedInputModesStr = string.Empty;
        
        public VoteForInputModeCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            //Show all input modes
            InputModes[] inputModeArr = EnumUtility.GetValues<InputModes>.EnumValues;

            for (int i = 0; i < inputModeArr.Length; i++)
            {
                InputModes inputMode = inputModeArr[i];

                CachedInputModesStr += inputMode.ToString();

                if (i < (inputModeArr.Length - 1))
                {
                    CachedInputModesStr += ", ";
                }
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long curInputMode = DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE, 0L);
            InputModes inpMode = (InputModes)curInputMode;

            //Check the number of votes
            if (arguments.Count == 0)
            {
                InputModeVoteRoutine inpModeVoteRoutine = RoutineHandler.FindRoutine<InputModeVoteRoutine>();

                if (inpModeVoteRoutine == null)
                {
                    QueueMessage($"There is no voting in progress for a new input mode. To start one up, pass a mode as an argument: {CachedInputModesStr}");
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder(128);

                    //Show the number of votes for each mode
                    Dictionary<InputModes, long> curVotes = inpModeVoteRoutine.GetVotesPerMode();
                    foreach (KeyValuePair<InputModes, long> kvPair in curVotes)
                    {
                        stringBuilder.Append(kvPair.Key.ToString()).Append(' ').Append('=');
                        stringBuilder.Append(' ').Append(kvPair.Value).Append(' ').Append('|').Append(' ');
                    }

                    stringBuilder.Remove(stringBuilder.Length - 3, 3);
                    stringBuilder.Append(". To vote for a new input mode, pass one as an argument: ").Append(CachedInputModesStr);

                    int charLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

                    QueueMessageSplit(stringBuilder.ToString(), charLimit, "| ");
                }

                return;
            }

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user has the ability to vote
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.VOTE_INPUT_MODE_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to vote for a new input mode!");
                    return;
                }
            }

            //Check if the cooldown is up
            DateTime nowUTC = DateTime.UtcNow;

            //Check the last completed time
            string lastComplete = DataHelper.GetSettingString(SettingsConstants.INPUT_MODE_NEXT_VOTE_DATE, DataHelper.GetStrFromDateTime(DateTime.UnixEpoch));
            if (DateTime.TryParse(lastComplete, out DateTime lastCompleteDate) == false)
            {
                lastCompleteDate = DateTime.UnixEpoch;
                Console.WriteLine($"Failed to parse DateTime: {DataHelper.GetStrFromDateTime(lastCompleteDate)}");
            }

            if (nowUTC < lastCompleteDate)
            {
                QueueMessage($"Input mode voting is on cooldown until {lastCompleteDate}");
                return;
            }

            string inputModeStr = arguments[0];

            //Parse
            if (EnumUtility.TryParseEnumValue(inputModeStr, out InputModes parsedInputMode) == false)
            {
                QueueMessage($"Please enter a valid input mode: {CachedInputModesStr}");
                return;
            }

            bool commencedNewVote = false;

            //Get the routine
            InputModeVoteRoutine inputModeVoteRoutine = RoutineHandler.FindRoutine<InputModeVoteRoutine>();
            
            //Add the routine if it doesn't exist
            if (inputModeVoteRoutine == null)
            {
                long voteDur = DataHelper.GetSettingInt(SettingsConstants.INPUT_MODE_VOTE_TIME, 60000L);
                inputModeVoteRoutine = new InputModeVoteRoutine(voteDur);
                RoutineHandler.AddRoutine(inputModeVoteRoutine);

                commencedNewVote = true;
            }

            string userName = args.Command.ChatMessage.Username.ToLowerInvariant();

            //Add the vote
            inputModeVoteRoutine.AddModeVote(userName, parsedInputMode);

            if (commencedNewVote == false)
            {
                QueueMessage($"{userName} voted for {parsedInputMode}!");
            }
            else
            {
                QueueMessage($"Voting for changing the input mode has begun! {userName} voted for {parsedInputMode}!");
            }
        }
    }
}
