/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Logging;

namespace TRBot.Commands
{
    /// <summary>
    /// Simulates a user.
    /// </summary>
    public sealed class UserSimulateCommand : BaseCommand
    {
        /// <summary>
        /// The number of words to use for each prefix.
        /// </summary>
        private const int PREFIX_COUNT = 2;

        /// <summary>
        /// The minimum number of prefixes required to generate a sentence.
        /// </summary>
        private const int MIN_PREFIX_GEN_COUNT = 5;

        private MarkovChainTextGenerator TextGenerator = new MarkovChainTextGenerator();

        private string UsageMessage = "Usage: \"username (optional)\"";

        private string ManageCmdName = null;
        private string ManageCmdUsage = string.Empty;

        public UserSimulateCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            TextGenerator = new MarkovChainTextGenerator();
            TextGenerator.PrefixWordCount = PREFIX_COUNT;
            TextGenerator.MinPrefixGenCount = MIN_PREFIX_GEN_COUNT;
        }

        public override void CleanUp()
        {
            ManageCmdName = null;
            ManageCmdUsage = string.Empty;

            TextGenerator = null;

            base.CleanUp();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            CacheManageCommandMessage();

            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            string thisUserName = args.Command.ChatMessage.Username;
            string simulatedUserName = thisUserName;

            if (arguments.Count > 0)
            {
                simulatedUserName = arguments[0].ToLowerInvariant();
            }

            //Check if the user has enough credits
            long creditCost = DataHelper.GetSettingInt(SettingsConstants.USER_SIMULATE_CREDIT_COST, 1000L);

            long thisUserCredits = 0L;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);

                if (thisUser == null || thisUser.HasEnabledAbility(PermissionConstants.SIMULATE_ABILITY) == false)
                {
                    QueueMessage("You do not have the ability to use simulate!");
                    return;
                }

                //It's possible to simulate other users while opted out of simulate data
                //However, if simulate costs credits, the user can't use it if opted out of bot stats
                if (creditCost > 0L && thisUser.IsOptedOut == true)
                {
                    QueueMessage("You're not opted into bot stats, so you cannot use simulate!");
                    return;
                }

                //Fetch available credits
                thisUserCredits = thisUser.Stats.Credits;
            }

            string simulateData = string.Empty;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                User simulatedUser = DataHelper.GetUserNoOpen(simulatedUserName, context);

                if (simulatedUser == null)
                {
                    QueueMessage($"User not found in database.");
                    return;
                }

                if (simulatedUser.IsOptedOut == true || simulatedUser.IsOptedIntoSimulate == false)
                {
                    QueueMessage($"\"{simulatedUserName}\" cannot be simulated because they're not opted into bot or simulate data! {ManageCmdUsage}");
                    return;
                }

                simulateData = simulatedUser.Stats.SimulateHistory;
            }

            string creditsName = DataHelper.GetCreditsName();

            if (creditCost > 0L && thisUserCredits < creditCost)
            {
                QueueMessage($"You need at least {creditCost} {creditsName.Pluralize(creditCost)} to use simulate!");
                return;
            }

            //Generate a sentence no longer than the bot character limit
            int botCharLimit = (int)DataHelper.GetSettingInt(SettingsConstants.BOT_MSG_CHAR_LIMIT, 500L);

            TextGenerator.CharacterLimit = botCharLimit;

            string simulateSentence = TextGenerator.GenerateText(simulateData);

            if (string.IsNullOrEmpty(simulateSentence) == true)
            {
                QueueMessage("No simulation could be generated for this user. There may not be enough data available. Simply talk in chat to generate simulation data.");
                return;
            }

            QueueMessage(simulateSentence);

            //Subtract credits if there is a cost
            if (creditCost > 0L)
            {
                long remainingCredits = 0L;

                //Subtract the credits from this user
                using (BotDBContext context = DatabaseManager.OpenContext())
                {
                    User thisUser = DataHelper.GetUserNoOpen(thisUserName, context);

                    thisUser.Stats.Credits -= creditCost;

                    context.SaveChanges();

                    remainingCredits = thisUser.Stats.Credits;
                }

                QueueMessage($"Spent {creditCost} {creditsName.Pluralize(creditCost)} to simulate! You now have {remainingCredits} {creditsName.Pluralize(remainingCredits)} remaining!");
            }
        }
        
        private void CacheManageCommandMessage()
        {
            //Check specifically for null, which means the message hasn't been cached at all
            //We can't do it in Initialize because the other command may not have been added to the command handler yet
            if (ManageCmdName != null)
            {
                return;
            }

            //Get the name of the command that manages simulate data
            if (CmdHandler.GetCommand<UserSimulateManageCommand>(out ManageCmdName) == false)
            {
                ManageCmdName = string.Empty;
                return;
            }

            //Set the message to tell users how to opt in
            ManageCmdUsage = $"To change your simulate opt status, use the \"{ManageCmdName}\" command.";
        }
    }
}
