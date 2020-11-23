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
using TRBot.Data;

namespace TRBot.Routines
{
    /// <summary>
    /// The base class for bot routines.
    /// </summary>
    public abstract class BaseRoutine
    {
        /// <summary>
        /// The routine's identifier.
        /// </summary>
        public string Identifier = string.Empty;

        protected BotRoutineHandler RoutineHandler = null;
        protected DataContainer DataContainer = null; 

        public BaseRoutine()
        {

        }

        /// <summary>
        /// Sets data required for many routines to function.
        /// </summary>
        public void SetRequiredData(BotRoutineHandler routineHandler, DataContainer dataContainer)
        {
            RoutineHandler = routineHandler;
            DataContainer = dataContainer;
        }

        public virtual void Initialize()
        {

        }

        public virtual void CleanUp()
        {
            RoutineHandler = null;
            DataContainer = null;
        }

        public abstract void UpdateRoutine(in DateTime currentTimeUTC);
    }
}
