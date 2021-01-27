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
using System.Collections;
using System.Collections.Generic;

namespace TRBot.Parsing
{
    /// <summary>
    /// A pre-parser that substitutes all input synonyms.
    /// </summary>
    public class InputSynonymPreparser : IPreparser
    {
        private IEnumerable<InputSynonym> InputSynonyms = null;

        public InputSynonymPreparser(IEnumerable<InputSynonym> inputSynonyms)
        {
            InputSynonyms = inputSynonyms;
        }

        /// <summary>
        /// Pre-parses a string to prepare it for the parser.
        /// </summary>
        /// <param name="message">The message to pre-parse.</param>
        /// <returns>A string containing the modified message.</returns>
        public string Preparse(string message)
        {
            if (InputSynonyms != null)
            {
                foreach (InputSynonym synonym in InputSynonyms)
                {
                    message = message.Replace(synonym.SynonymName, synonym.SynonymValue);
                }
            }

            return message;
        }
    }
}