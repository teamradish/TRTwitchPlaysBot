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
    /// Represents a millisecond duration component in the parser.
    /// </summary>
    public class MillisecondParserComponent : GenericParserComponent
    {
        public const string MS_SYMBOL = "ms";
        public const string MS_DUR_GROUP_NAME = "milsec";
        public const string DUR_NUM_GROUP_NAME = "dur";

        public MillisecondParserComponent()
            : base(@"(?<" + MS_DUR_GROUP_NAME + @">(?<" + DUR_NUM_GROUP_NAME + @">\d+)" + MS_SYMBOL + @")?")
        {
            
        }
    }
}