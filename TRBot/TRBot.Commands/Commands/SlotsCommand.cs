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
using TRBot.Data;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Plays a game of slots.
    /// </summary>
    /// <remarks>This uses an implementation loosely similar to real slots, based on data from here: https://wizardofodds.com/games/slots/appendix/2/ </remarks>
    public sealed class SlotsCommand : BaseCommand
    {
        /// <summary>
        /// Internal names for the slot panels.
        /// </summary>
        private enum SlotInternalNames
        {
            Blank,
            Cherry,
            Plum,
            Watermelon,
            Orange,
            Lemon,
            Bar
        }

        /// <summary>
        /// Slot results.
        /// </summary>
        private enum SlotResults
        {
            Nothing,
            Standard,
            Jackpot
        }

        /// <summary>
        /// Maps a slot name to an emote. The value is the setting name controlling the emote.
        /// </summary>
        private readonly Dictionary<SlotInternalNames, string> SlotToEmoteMap = new Dictionary<SlotInternalNames, string>(7)
        {
            { SlotInternalNames.Blank, SettingsConstants.SLOTS_BLANK_EMOTE },
            { SlotInternalNames.Cherry, SettingsConstants.SLOTS_CHERRY_EMOTE },
            { SlotInternalNames.Plum, SettingsConstants.SLOTS_PLUM_EMOTE },
            { SlotInternalNames.Watermelon, SettingsConstants.SLOTS_WATERMELON_EMOTE },
            { SlotInternalNames.Orange, SettingsConstants.SLOTS_ORANGE_EMOTE },
            { SlotInternalNames.Lemon, SettingsConstants.SLOTS_LEMON_EMOTE },
            { SlotInternalNames.Bar, SettingsConstants.SLOTS_BAR_EMOTE },
        };
        
        private readonly Dictionary<SlotInternalNames, double> SlotRewardModifiers = new Dictionary<SlotInternalNames, double>(7)
        {
            { SlotInternalNames.Blank, 0d },
            { SlotInternalNames.Lemon, 2d },
            { SlotInternalNames.Orange, 4d },
            { SlotInternalNames.Watermelon, 10d },
            { SlotInternalNames.Plum, 25d },
            { SlotInternalNames.Cherry, 100d },
            { SlotInternalNames.Bar, 500d },
        };

        private const string INFO_ARG = "info";

        /// <summary>
        /// The weight table specifying the weights of each type of slot for each reel.
        /// </summary>
        private Dictionary<int, List<ReelWeight>> WeightTable = null;

        /// <summary>
        /// A cached list used when determining which slot to choose.
        /// </summary>
        private readonly List<double> WeightCache = new List<double>();

        private double HighestRewardMod = 0d;
        private Random Rand = new Random();

        private string UsageMessage = "Usage: \"buy-in (int)\" or \"info\"";

        public SlotsCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            BuildWeightTable();

            //Cache so we know which one is the jackpot
            HighestRewardMod = SlotRewardModifiers.Values.Max();
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            if (arguments.Count != 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            //Start off getting the credits name
            string creditsName = DataHelper.GetCreditsName();
            string userName = args.Command.ChatMessage.Username;
            long curUserCredits = 0;

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Get the user
                User user = DataHelper.GetUserNoOpen(userName, context);

                if (user == null)
                {
                    QueueMessage("You're somehow not in the database!");
                    return;
                }

                //Can't play without the ability
                if (user.HasEnabledAbility(PermissionConstants.SLOTS_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to play the slots!");
                    return;
                }

                //Check for opt-out
                if (user.IsOptedOut == true)
                {
                    QueueMessage("You cannot play the slots while opted out of stats.");
                    return;
                }

                curUserCredits = user.Stats.Credits;
            }

            string buyInStr = arguments[0];

            //If the user asks for info, print the infom essage
            if (buyInStr == INFO_ARG)
            {
                PrintInfoMessage();
                return;
            }

            //Validate argument
            if (long.TryParse(buyInStr, out long buyInAmount) == false)
            {
                QueueMessage(UsageMessage);
                return;
            }

            if (buyInAmount <= 0)
            {
                QueueMessage("Buy-in amount must be greater than 0!");
                return;
            }

            if (buyInAmount > curUserCredits)
            {
                QueueMessage($"Buy-in amount is greater than {creditsName.Pluralize(0)}!");
                return;
            }

            //Roll the slots!
            StringBuilder strBuilder = new StringBuilder(128);
            strBuilder.Append('(').Append(' ');

            SlotInternalNames[] slotsChosen = new SlotInternalNames[WeightTable.Count];
            int i = 0;

            foreach (KeyValuePair<int, List<ReelWeight>> weight in WeightTable)
            {
                SlotInternalNames chosenSlot = ChooseSlot(weight.Value);
                slotsChosen[i] = chosenSlot;

                string emote = DataHelper.GetSettingString(SlotToEmoteMap[chosenSlot], string.Empty);

                strBuilder.Append(emote).Append(' ').Append('|').Append(' ');

                i++;
            }

            strBuilder.Remove(strBuilder.Length - 3, 3);
            strBuilder.Append(' ').Append(')').Append(" = ");

            //Evaluate the reward based on what we got
            double rewardModifier = EvaluateReward(slotsChosen, out SlotResults slotResult);

            //Intentionally floor the reward - slots are like that :P
            long reward = (long)(buyInAmount * rewardModifier); 

            //Change the message based on the result
            switch (slotResult)
            {
                case SlotResults.Nothing:
                    strBuilder.Append(userName).Append(" didn't win BibleThump Better luck next time!");
                    break;
                case SlotResults.Standard:
                    strBuilder.Append(userName).Append(" won ").Append(reward).Append(' ').Append(creditsName.Pluralize(reward)).Append(", nice! SeemsGood");
                    break;
                case SlotResults.Jackpot:
                    strBuilder.Append(userName).Append(" hit the JACKPOT and won ").Append(reward).Append(' ').Append(creditsName.Pluralize(reward)).Append("!! Congratulations!! PogChamp PogChamp PogChamp");
                    break;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Modify credits
                User user = DataHelper.GetUserNoOpen(userName, context);

                //Adjust credits and save
                user.Stats.Credits -= buyInAmount;
                user.Stats.Credits += reward;

                context.SaveChanges();
            }

            QueueMessage(strBuilder.ToString());
        }

        private double EvaluateReward(SlotInternalNames[] slotsChosen, out SlotResults slotResult)
        {
            //For this implementation, reward the user only if all of the slots match
            SlotInternalNames firstSlot = slotsChosen[0];

            for (int i = 1; i < slotsChosen.Length; i++)
            {
                //Slots don't match, so no reward
                if (slotsChosen[i] != firstSlot)
                {
                    slotResult = SlotResults.Nothing;
                    return 0d;
                }
            }

            //Get the reward modifier for this slot
            double modifier = SlotRewardModifiers[firstSlot];
            
            //If the user didn't actually win anything, show it as a loss
            if (modifier <= 0d)
            {
                slotResult = SlotResults.Nothing;
            }
            //If the user got the highest reward modifier, they hit the jackpot!
            else if (modifier == HighestRewardMod)
            {
                slotResult = SlotResults.Jackpot;
            }
            //Standard result
            else
            {
                slotResult = SlotResults.Standard;
            }

            return modifier;
        }

        private SlotInternalNames ChooseSlot(List<ReelWeight> weights)
        {
            double maxWeight = 0;

            for (int i = 0; i < weights.Count; i++)
            {
                maxWeight += weights[i].WeightVal;
            }

            WeightCache.Clear();
            
            //Get the weights from 0 to 1
            for (int i = 0; i < weights.Count; i++)
            {
                WeightCache.Add(weights[i].WeightVal / maxWeight);
            }

            int index = Helpers.ChoosePercentage(Rand.NextDouble(), WeightCache);

            return weights[index].Slot;
        }

        private void BuildWeightTable()
        {
            if (WeightTable == null)
            {
                WeightTable = new Dictionary<int, List<ReelWeight>>(3);
            }

            WeightTable.Clear();

            WeightTable.Add(0, GetReel1Weights());
            WeightTable.Add(1, GetReel2Weights());
            WeightTable.Add(2, GetReel3Weights());
        }

        private List<ReelWeight> GetReel1Weights()
        {
            List<ReelWeight> reel1 = new List<ReelWeight>(7);
            reel1.Add(new ReelWeight(SlotInternalNames.Blank, 8));
            reel1.Add(new ReelWeight(SlotInternalNames.Cherry, 5));
            reel1.Add(new ReelWeight(SlotInternalNames.Plum, 6));
            reel1.Add(new ReelWeight(SlotInternalNames.Watermelon, 6));
            reel1.Add(new ReelWeight(SlotInternalNames.Orange, 7));
            reel1.Add(new ReelWeight(SlotInternalNames.Lemon, 8));
            reel1.Add(new ReelWeight(SlotInternalNames.Bar, 4));
            return reel1;
        }

        private List<ReelWeight> GetReel2Weights()
        {
            List<ReelWeight> reel2 = new List<ReelWeight>(7);
            reel2.Add(new ReelWeight(SlotInternalNames.Blank, 10));
            reel2.Add(new ReelWeight(SlotInternalNames.Cherry, 4));
            reel2.Add(new ReelWeight(SlotInternalNames.Plum, 4));
            reel2.Add(new ReelWeight(SlotInternalNames.Watermelon, 5));
            reel2.Add(new ReelWeight(SlotInternalNames.Orange, 5));
            reel2.Add(new ReelWeight(SlotInternalNames.Lemon, 6));
            reel2.Add(new ReelWeight(SlotInternalNames.Bar, 3));
            return reel2;
        }

        private List<ReelWeight> GetReel3Weights()
        {
            List<ReelWeight> reel3 = new List<ReelWeight>(7);
            reel3.Add(new ReelWeight(SlotInternalNames.Blank, 12));
            reel3.Add(new ReelWeight(SlotInternalNames.Cherry, 2));
            reel3.Add(new ReelWeight(SlotInternalNames.Plum, 3));
            reel3.Add(new ReelWeight(SlotInternalNames.Watermelon, 4));
            reel3.Add(new ReelWeight(SlotInternalNames.Orange, 6));
            reel3.Add(new ReelWeight(SlotInternalNames.Lemon, 6));
            reel3.Add(new ReelWeight(SlotInternalNames.Bar, 1));
            return reel3;
        }

        private void PrintInfoMessage()
        {
            int count = WeightTable.Count;

            StringBuilder stringBuilder = new StringBuilder(128);

            foreach (KeyValuePair<SlotInternalNames, double> kvPair in SlotRewardModifiers)
            {
                string emoteName = DataHelper.GetSettingString(SlotToEmoteMap[kvPair.Key], string.Empty);

                stringBuilder.Append(emoteName).Append(' ').Append('x').Append(count).Append(" = ");
                stringBuilder.Append(kvPair.Value).Append('x').Append(" buy-in | ");
            }

            stringBuilder.Remove(stringBuilder.Length - 3, 3);

            string infoMessage = stringBuilder.ToString();

            QueueMessage(infoMessage);
        }

        private struct ReelWeight
        {
            public SlotInternalNames Slot;
            public int WeightVal;

            public ReelWeight(in SlotInternalNames slot, in int weightVal)
            {
                Slot = slot;
                WeightVal = weightVal;
            }

            public override bool Equals(object obj)
            {
                if (obj is ReelWeight reelWeight)
                {
                    return (this == reelWeight);
                }

                return false;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 37;
                    hash = (hash * 59) + Slot.GetHashCode();
                    hash = (hash * 59) + WeightVal.GetHashCode();
                    return hash;
                } 
            }

            public static bool operator==(ReelWeight a, ReelWeight b)
            {
                return (a.Slot == b.Slot && a.WeightVal == b.WeightVal);
            }

            public static bool operator!=(ReelWeight a, ReelWeight b)
            {
                return !(a == b);
            }
        }
    }
}
