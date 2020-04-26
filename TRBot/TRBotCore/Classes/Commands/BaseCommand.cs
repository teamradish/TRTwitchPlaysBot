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
using TwitchLib;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace TRBot
{
    /// <summary>
    /// Base class for a command
    /// </summary>
    public abstract class BaseCommand
    {
        public bool HiddenFromHelp = false;

        //Kimimaru: We might want to specify this in the settings file to make things more configurable
        public int AccessLevel { get; protected set; } = (int)AccessLevels.Levels.User;

        public BaseCommand()
        {
            
        }

        public virtual void Initialize(CommandHandler commandHandler)
        {

        }

        public virtual void CleanUp()
        {
            
        }

        public abstract void ExecuteCommand(object sender, OnChatCommandReceivedArgs e);
    }
}
