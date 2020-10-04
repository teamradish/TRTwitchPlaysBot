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
using System.Text;
using System.IO;
using System.Linq;
using TRBot.Common;
using TRBot.Utilities;

namespace TRBot.Data
{
    /// <summary>
    /// A container for common data.
    /// </summary>
    public class DataContainer
    {
        public BotMessageHandler MessageHandler { get; private set; } = null;
        public DataReloader DataReloader { get; private set; } = null;

        public DataContainer()
        {

        }

        public void SetMessageHandler(BotMessageHandler msgHandler)
        {
            MessageHandler = msgHandler;
        }

        public void SetDataReloader(DataReloader dataReloader)
        {
            DataReloader = dataReloader;
        }
    }
}
