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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using TRBot.Consoles;

namespace TRBot.Tests
{
    [TestFixture]
    public class InputAxisUnitTests
    {
        [TestCase(
            0, 0d, .9001d, 100d,
            0, 0d, .9d, 100d
        )]
        [TestCase(
            2, 0d, 2d, 99.999d,
            2, 0d, 1d, 99.999d
        )]
        public void TestAxesEqual(int axisV1, double minAxisV1, double maxAxisV1, double maxPercentV1,
            int axisV2, double minAxisV2, double maxAxisV2, double maxPercentV2)
        {
            InputAxis axis1 = new InputAxis(axisV1, minAxisV1, maxAxisV1, maxPercentV1);
            InputAxis axis2 = new InputAxis(axisV2, minAxisV2, maxAxisV2, maxPercentV2);

            Assert.AreEqual(axis1 == axis2, true);
        }
    }
}