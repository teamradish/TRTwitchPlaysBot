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

namespace TRBot.Parsing
{
    /// <summary>
    /// Available options regarding parsing dynamic macros.
    /// </summary>
    public enum DynamicMacroArgOptions
    {
        /// <summary>
        /// Fill arguments when parsing dynamic macros. 
        /// </summary>
        FillArgs = 0,

        /// <summary>
        /// Don't fill arguments when parsing dynamic macros.
        /// This is used primarily for validating the output of dynamic macros.
        /// To parse inputs from macros, use another option. 
        /// </summary>
        DontFillArgs = 1
    }
}