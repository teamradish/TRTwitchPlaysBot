/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents an input pre-parser.
    /// Pre-parsers prepare an input string for parsing.
    /// </summary>
    public interface IPreparser
    {
        /// <summary>
        /// Pre-parses a string to prepare it for the parser.
        /// </summary>
        /// <param name="message">The message to pre-parse.</param>
        /// <returns>A string containing the modified message.</returns>
        string Preparse(string message);
    }
}