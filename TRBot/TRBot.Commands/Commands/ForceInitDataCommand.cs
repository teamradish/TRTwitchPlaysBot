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
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Parsing;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// Force initializes missing default data in the database.
    /// </summary>
    public sealed class ForceInitDataCommand : BaseCommand
    {
        public ForceInitDataCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            QueueMessage("Checking to initialize default values for missing database entries.");

            int entriesAdded = 0;
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Tell it to force initialize defaults
                Settings forceInitSetting = DataHelper.GetSettingNoOpen(SettingsConstants.FORCE_INIT_DEFAULTS, context);
                if (forceInitSetting != null)
                {
                    forceInitSetting.ValueInt = 1;
                    context.SaveChanges();
                }
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                entriesAdded = DataHelper.InitDefaultData(context);

                if (entriesAdded > 0)
                {
                    context.SaveChanges();
                }
            }

            if (entriesAdded > 0)
            {
                QueueMessage($"Added {entriesAdded} additional entries to the database. Make sure to reload data if you're expecting any new commands.");
            }
            else
            {
                QueueMessage("No new entries added.");
            }
        }
    }
}
