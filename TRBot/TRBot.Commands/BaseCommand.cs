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
using TRBot.Routines;

namespace TRBot.Commands
{
    /// <summary>
    /// Base class for a command.
    /// </summary>
    public abstract class BaseCommand
    {
        public bool Enabled = true;
        public bool DisplayInHelp = true;
        public long Level = 0;
        public string ValueStr = string.Empty;

        protected CommandHandler CmdHandler = null;
        protected DataContainer DataContainer = null;
        protected BotRoutineHandler RoutineHandler = null;

        public BaseCommand()
        {
            
        }

        /// <summary>
        /// Sets required data for many commands to function.
        /// </summary>
        public void SetRequiredData(CommandHandler cmdHandler, DataContainer dataContainer, BotRoutineHandler routineHandler)
        {
            CmdHandler = cmdHandler;
            DataContainer = dataContainer;
            RoutineHandler = routineHandler;
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void CleanUp()
        {
            
        }

        public abstract void ExecuteCommand(EvtChatCommandArgs args);

        protected void QueueMessage(string message)
        {
            DataContainer.MessageHandler.QueueMessage(message);
        }

        protected void QueueMessage(string message, in Serilog.Events.LogEventLevel logLevel)
        {
            DataContainer.MessageHandler.QueueMessage(message, logLevel);
        }

        protected void QueueMessageSplit(string message, in int maxCharCount, string separator)
        {
            DataContainer.MessageHandler.QueueMessageSplit(message, maxCharCount, separator);
        }

        protected void QueueMessageSplit(string message, in Serilog.Events.LogEventLevel logLevel,
            in int maxCharCount, string separator)
        {
            DataContainer.MessageHandler.QueueMessageSplit(message, logLevel, maxCharCount, separator);
        }
    }
}
