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

        [TestCase(50, 50)]
        [TestCase(30.0001, 30)]
        [TestCase(90.1, 90.1)]
        [TestCase(64.15, 64.15)]
        [TestCase(45.4321, 45.4328)]
        [TestCase(76.23451332, 76.23483627)]
        public void TestPercentageEquality(double percentage1, double percentage2)
        {
            ParsedInput a = new ParsedInput("test", false, false, percentage1, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            ParsedInput b = new ParsedInput("test", false, false, percentage2, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            Assert.AreEqual(a == b, true);
        }

        [TestCase(0, 100)]
        [TestCase(97.1, 97.2)]
        [TestCase(97.54, 97.55)]
        [TestCase(46.549, 46.550)]
        [TestCase(29.549, 9297.551)]
        [TestCase(30.001, 30.002)]
        [TestCase(45.4321, 45.4528)]
        [TestCase(33.333, 33.334)]
        public void TestPercentageInequality(double percentage1, double percentage2)
        {
            ParsedInput a = new ParsedInput("test", false, false, percentage1, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            ParsedInput b = new ParsedInput("test", false, false, percentage2, 200, InputDurationTypes.Milliseconds,
                0, string.Empty);

            Assert.AreEqual(a == b, false);
        }
    }
}