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
    /// Represents a release component in the parser.
    /// </summary>
    public class ReleaseParserComponent : GenericParserComponent
    {
        public const string RELEASE_SYMBOL = @"-";
        public const string RELEASE_GROUP_NAME = "release";
        
        public ReleaseParserComponent()
            : base(@"(?<" + RELEASE_GROUP_NAME + @">\" + RELEASE_SYMBOL + @")?")
        {
            
        }
    }
}