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
    public sealed class AchievementInfoCommand : BaseCommand
    {
        public AchievementInfoCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            string args = e.Command.ArgumentsAsString.ToLower();
            if (BotProgram.BotData.Achievements.AchievementDict.TryGetValue(args, out Achievement achievement) == false)
            {
                BotProgram.QueueMessage("There are no achievements with that name!");
                return;
            }

            //string name = e.Command.ChatMessage.DisplayName;
            //string nameToLower = name.ToLower();

            //User user = BotProgram.GetUser(nameToLower);
            //if (user == null)
            //{
            //    return;
            //}
            string achStr = achievement.Description.ToString();
            if (achievement.CreditReward > 0)
            {
                achStr += " | " + achievement.CreditReward.ToString() + " credit reward.";
            }

            BotProgram.QueueMessage(achStr);
        }
    }
}
