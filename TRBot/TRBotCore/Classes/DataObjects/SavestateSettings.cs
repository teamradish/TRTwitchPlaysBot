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

namespace TRBot
{
    /// <summary>
    /// Holds settings for savestates.
    /// </summary>
    public class SavestateSettings
    {
        /// <summary>
        /// The type of savestates to use.
        /// </summary>
        public SavestateTypes SavestateType = SavestateTypes.Quick;

        /// <summary>
        /// The current save slot; used for <see cref="SavestateTypes.Slot"/>.
        /// <para>Make sure this value matches the savestate slot on your game for accuracy.</para>
        /// </summary>
        public int CurrentSaveSlot = 1;
    }
}
