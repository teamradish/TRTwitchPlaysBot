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
    /// Base class for a command.
    /// </summary>
    public abstract class BaseCommand
    {
        public bool Enabled = true;
        public bool DisplayInHelp = true;
        public int Level = 0;
        public string ValueStr = string.Empty;

        protected CommandHandler CmdHandler = null;
        protected DataContainer DataContainer = null;

        public BaseCommand()
        {
            
        }

        public virtual void Initialize(CommandHandler cmdHandler, DataContainer dataContainer)
        {
            CmdHandler = cmdHandler;
            DataContainer = dataContainer;
        }

        public virtual void CleanUp()
        {
            
        }

        public abstract void ExecuteCommand(EvtChatCommandArgs args);

        protected void QueueMessage(string message)
        {
            DataContainer.MessageHandler.QueueMessage(message);
        }

        protected void QueueMessageSplit(string message, in int maxCharCount, string separator)
        {
            string sentMessage = Helpers.SplitStringWithinCharCount(message, maxCharCount, out List<string> textList);

            //If the text fits within the character limit, print it all out at once
            if (textList == null)
            {
                DataContainer.MessageHandler.QueueMessage(sentMessage);
            }
            else
            {
                //Otherwise, queue up the text in pieces
                for (int i = 0; i < textList.Count; i++)
                {
                    DataContainer.MessageHandler.QueueMessage(textList[i]);
                }
            }
        }
    }
}
