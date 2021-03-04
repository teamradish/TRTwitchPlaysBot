/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using TRBot.Parsing;

namespace TRBot.Tests
{
    [TestFixture]
    public class ParsedInputUnitTests
    {
        [TestCase]
        public void TestEquality()
        {
            ParsedInput a = new ParsedInput("test", false, false, 100d, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            ParsedInput b = new ParsedInput("test", false, false, 100d, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            Assert.AreEqual(a == b, true);
        }

        [TestCase]
        public void TestInequality()
        {
            ParsedInput a = new ParsedInput("test", false, false, 100d, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            ParsedInput b = new ParsedInput("test", false, false, 100.1d, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            Assert.AreEqual(a != b, true);
        }
    }
}