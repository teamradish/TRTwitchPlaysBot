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

namespace TRBot.Utilities
{
    /// <summary>
    /// Notifies subscribers when data should be reloaded.
    /// </summary>
    public class DataReloader
    {
        /// <summary>
        /// A delegate for data reloading.
        /// </summary>
        public delegate void OnDataReloaded();
        
        /// <summary>
        /// An event invoked when data has been reloaded.
        /// </summary>
        public event OnDataReloaded DataReloadedEvent = null;

        public DataReloader()
        {

        }

        public void CleanUp()
        {
            DataReloadedEvent = null;
        }

        /// <summary>
        /// Invokes a data reload.
        /// </summary>
        public void ReloadData()
        {
            DataReloadedEvent?.Invoke();
        }
    }
}
