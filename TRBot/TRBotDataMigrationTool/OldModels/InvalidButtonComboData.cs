﻿/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
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
using System.Collections.Generic;
using System.Text;

namespace TRBotDataMigrationTool
{
    /// <summary>
    /// Data for invalid button combos.
    /// </summary>
    public sealed class InvalidButtonComboData
    {
        public readonly Dictionary<int, List<string>> InvalidCombos = new Dictionary<int, List<string>>(4)
        {
            { (int)InputConsoles.GBA, new List<string>() { "a", "b", "start", "select" } }
        };
    }
}
