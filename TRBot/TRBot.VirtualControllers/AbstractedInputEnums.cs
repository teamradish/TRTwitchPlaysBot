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
using System.Collections.Generic;
using System.Text;

namespace TRBot.VirtualControllers
{
    /// <summary>
    /// The states for buttons.
    /// </summary>
    public enum ButtonStates
    {
        Released = 0,
        Pressed = 1
    }
    
    #region Button Vals

    /// <summary>
    /// Button values for virtual controllers.
    /// </summary>
    public enum GlobalButtonVals
    {
        BTN1   = 0,
        BTN2   = 1,
        BTN3   = 2,
        BTN4   = 3,
        BTN5   = 4,
        BTN6   = 5,
        BTN7   = 6,
        BTN8   = 7,
        BTN9   = 8,
        BTN10  = 9,
        BTN11  = 10,
        BTN12  = 11,
        BTN13  = 12,
        BTN14  = 13,
        BTN15  = 14,
        BTN16  = 15,
        BTN17  = 16,
        BTN18  = 17,
        BTN19  = 18,
        BTN20  = 19,
        BTN21  = 20,
        BTN22  = 21,
        BTN23  = 22,
        BTN24  = 23,
        BTN25  = 24,
        BTN26  = 25,
        BTN27  = 26,
        BTN28  = 27,
        BTN29  = 28,
        BTN30  = 29,
        BTN31  = 30,
        BTN32  = 31,
        BTN33  = 32,
        BTN34  = 33,
        BTN35  = 34,
        BTN36  = 35,
        BTN37  = 36,
        BTN38  = 37,
        BTN39  = 38,
        BTN40  = 39,
        BTN41  = 40,
        BTN42  = 41,
        BTN43  = 42,
        BTN44  = 43,
        BTN45  = 44,
        BTN46  = 45,
        BTN47  = 46,
        BTN48  = 47,
        BTN49  = 48,
        BTN50  = 49,
        BTN51  = 50,
        BTN52  = 51,
        BTN53  = 52,
        BTN54  = 53,
        BTN55  = 54,
        BTN56  = 55,
        BTN57  = 56,
        BTN58  = 57,
        BTN59  = 58,
        BTN60  = 59,
        BTN61  = 60,
        BTN62  = 61,
        BTN63  = 62,
        BTN64  = 63,
        BTN65  = 64,
        BTN66  = 65,
        BTN67  = 66,
        BTN68  = 67,
        BTN69  = 68,
        BTN70  = 69,
        BTN71  = 70,
        BTN72  = 71,
        BTN73  = 72,
        BTN74  = 73,
        BTN75  = 74,
        BTN76  = 75,
        BTN77  = 76,
        BTN78  = 77,
        BTN79  = 78,
        BTN80  = 79,
        BTN81  = 80,
        BTN82  = 81,
        BTN83  = 82,
        BTN84  = 83,
        BTN85  = 84,
        BTN86  = 85,
        BTN87  = 86,
        BTN88  = 87,
        BTN89  = 88,
        BTN90  = 89,
        BTN91  = 90,
        BTN92  = 91,
        BTN93  = 92,
        BTN94  = 93,
        BTN95  = 94,
        BTN96  = 95,
        BTN97  = 96,
        BTN98  = 97,
        BTN99  = 98,
        BTN100 = 99,
        BTN101 = 100,
        BTN102 = 101,
        BTN103 = 102,
        BTN104 = 103,
        BTN105 = 104,
        BTN106 = 105,
        BTN107 = 106,
        BTN108 = 107,
        BTN109 = 108,
        BTN110 = 109,
        BTN111 = 110,
        BTN112 = 111,
        BTN113 = 112,
        BTN114 = 113,
        BTN115 = 114,
        BTN116 = 115,
        BTN117 = 116,
        BTN118 = 117,
        BTN119 = 118,
        BTN120 = 119,
        BTN121 = 120,
        BTN122 = 121,
        BTN123 = 122,
        BTN124 = 123,
        BTN125 = 124,
        BTN126 = 125,
        BTN127 = 126,
        BTN128 = 127
    }

    #endregion

    #region Axis Vals

    /// <summary>
    /// Axis values for virtual controllers.
    /// </summary>
    public enum GlobalAxisVals
    {
        AXIS_X = 0,
        AXIS_Y = 1,
        AXIS_Z = 2,
        AXIS_RX = 3,
        AXIS_RY = 4,
        AXIS_RZ = 5,
        AXIS_M1 = 6,
        AXIS_M2 = 7
    }

    #endregion
}
