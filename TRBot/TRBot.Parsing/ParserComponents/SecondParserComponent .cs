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
    /// Represents a second duration component in the parser.
    /// </summary>
    public class SecondParserComponent : GenericParserComponent
    {
        public const string SEC_SYMBOL = "s";
        public const string SEC_DUR_GROUP_NAME = "sec";
        public const string DUR_NUM_GROUP_NAME = "dur";
        public const string DUR_DECIMAL_GROUP_NAME = "durdec";

        public SecondParserComponent()
            : base(@"(?<" + SEC_DUR_GROUP_NAME + @">(?<" + DUR_NUM_GROUP_NAME + @">\d+(?<" + DUR_DECIMAL_GROUP_NAME + @">\.\d{1,3})?)" + SEC_SYMBOL + @")?")
        {
            
        }
    }
}