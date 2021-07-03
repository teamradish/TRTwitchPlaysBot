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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents an input component in the parser.
    /// </summary>
    public class InputParserComponent : IParserComponent
    {
        public const string INPUT_GROUP_NAME = "input";

        public string ComponentRegex { get; private set; } = string.Empty;

        public InputParserComponent(IList<string> inputList)
        {
            const string startRegex = @"(?<" + INPUT_GROUP_NAME + @">(";
            const string endRegex = @"))";
            
            //If there are no inputs, simply combine the start and end regex
            if (inputList.Count == 0)
            {
                ComponentRegex = startRegex + endRegex;
                return;
            }

            IOrderedEnumerable<string> sorted = from str in inputList
                                                orderby str.Length descending
                                                select str;

            StringBuilder strBuilder = new StringBuilder(inputList.Count * 6);
            strBuilder.Append(startRegex);

            int lastIndex = inputList.Count - 1;
            
            //Build the input regex
            foreach (string inputStr in sorted)
            {
                strBuilder.Append(Regex.Escape(inputStr));
                strBuilder.Append('|');
            }

            strBuilder.Remove(strBuilder.Length - 1, 1);

            strBuilder.Append(endRegex);

            ComponentRegex = strBuilder.ToString();
        }
    }
}