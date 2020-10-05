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
using TRBot.Common;
using TRBot.Utilities;
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// A command that notifies all data should be reloaded.
    /// </summary>
    public sealed class ReloadCommand : BaseCommand
    {
        private const string SOFT_RELOAD_ARG = "soft";
        private const string HARD_RELOAD_ARG = "hard";

        private string UsageMessage = $"Usage - Soft reload: \"{SOFT_RELOAD_ARG}\" or no args | Hard reload: \"{HARD_RELOAD_ARG}\"";

        public ReloadCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            //Soft reload with no arguments
            if (arguments.Count == 0)
            {
                SoftReload();
                return;
            }

            if (arguments.Count > 1)
            {
                DataContainer.MessageHandler.QueueMessage(UsageMessage);
                return;
            }

            string arg1 = arguments[0].ToLowerInvariant();

            //Soft reloads
            if (arg1 == SOFT_RELOAD_ARG)
            {
                SoftReload();
            }
            else if (arg1 == HARD_RELOAD_ARG)
            {
                HardReload();
            }
            else
            {
                DataContainer.MessageHandler.QueueMessage(UsageMessage);
            }
        }

        private void SoftReload()
        {
            DataContainer.DataReloader.ReloadDataSoft();
            DataContainer.MessageHandler.QueueMessage("Finished reloading of data!");
        }

        private void HardReload()
        {
            DataContainer.DataReloader.ReloadDataHard();

            DataContainer.MessageHandler.QueueMessage("Finished hard reloading data!");
        }
    }
}
