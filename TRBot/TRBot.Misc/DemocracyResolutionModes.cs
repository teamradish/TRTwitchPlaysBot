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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Misc
{
    /// <summary>
    /// The available resolution modes for democracy.
    /// </summary>
    public enum DemocracyResolutionModes
    {
        /// <summary>
        /// The most voted on input sequence will be executed.
        /// This must be exact. For instance, "r+b" is a different vote from "r b".
        /// </summary>
        ExactSequence = 0,

        /// <summary>
        /// The most voted on input name will be executed. Only the first input in each input sequence is considered.
        /// The duration used is the global default for inputs.
        /// For instance, "a32ms" and "a250ms" are considered for the same vote.
        /// </summary>
        SameName = 1,

        /// <summary>
        /// The most voted on input will be executed. Only the first input in each input sequence is considered.
        /// This must be exact.
        /// For instance, "x400ms" and "x399ms" are considered different votes.
        /// </summary>
        ExactInput = 2
    }
}
