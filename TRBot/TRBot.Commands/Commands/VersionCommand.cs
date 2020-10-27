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
using TRBot.Data;

namespace TRBot.Commands
{
    /// <summary>
    /// A simple command that states the TRBot version number.
    /// </summary>
    public class VersionCommand : MessageCommand
    {
        public VersionCommand()
        {
            
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string versionString = DataHelper.GetSettingString(SettingsConstants.DATA_VERSION_NUM, "Unknown??");
            ValueStr = $"This bot is running TRBot version {versionString}!";
            
            base.ExecuteCommand(args);
        }
    }
}
