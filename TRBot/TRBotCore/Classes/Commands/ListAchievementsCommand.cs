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
        private List<string> AchievementCache = new List<string>(16);

        public ListAchievementsCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            CacheAchievementString();
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            if (BotProgram.BotData.Achievements.AchievementDict.Count == 0)
            {
                BotProgram.QueueMessage("There are no achievements :(");
                return;
            }

            for (int i = 0; i < AchievementCache.Count; i++)
            {
                BotProgram.QueueMessage(AchievementCache[i]);
            }
        }

        private void CacheAchievementString()
        {
            AchievementCache.Clear();

            string curString = string.Empty;

            //List all achievements
            int i = 0;
            foreach (KeyValuePair<string, Achievement> achKVPair in BotProgram.BotData.Achievements.AchievementDict)
            {
                string achName = achKVPair.Value.Name;

                int length = achName.Length + curString.Length;

                if (length >= Globals.TwitchCharacterLimit)
                {
                    AchievementCache.Add(curString);
                    curString = string.Empty;
                }

                curString += achName;

                if (i < (BotProgram.BotData.Achievements.AchievementDict.Count - 1))
                {
                    curString += ", ";
                }

                i++;
            }

            if (string.IsNullOrEmpty(curString) == false)
            {
                AchievementCache.Add(curString);
            }
        }
    }
}
