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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class ListAchievementsCommand : BaseCommand
    {
        private string AchievementListStr = string.Empty;

        public ListAchievementsCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotData.Achievements.AchievementDict.Count == 0)
            {
                BotProgram.QueueMessage("There are no achievements :(");
                return;
            }

            if (string.IsNullOrEmpty(AchievementListStr) == true)
            {
                AchievementListStr = BuildAchievementListStr();
            }

            BotProgram.QueueMessage(AchievementListStr);
        }

        private string BuildAchievementListStr()
        {
            Dictionary<string, Achievement> achDict = BotProgram.BotData.Achievements.AchievementDict;

            //The capacity is a rough approximation of the average character length of an achievement name
            StringBuilder strBuilder = new StringBuilder(achDict.Count * 16);

            foreach (KeyValuePair<string, Achievement> achKV in achDict)
            {
                strBuilder.Append(achKV.Value.Name).Append(',').Append(' ');
            }

            strBuilder.Remove(strBuilder.Length - 2, 2);

            return strBuilder.ToString();
        }
    }
}
