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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Routines
{
    /// <summary>
    /// Constants for bot routines.
    /// </summary>
    public static class RoutineConstants
    {
        //These routine names are for routines that are commonly added at runtime
        public const string GROUP_BET_ROUTINE_NAME = "groupbet";
        public const string RECONNECT_ROUTINE_NAME = "reconnect";
        public const string PERIODIC_INPUT_ROUTINE_NAME = "periodicinput";
        public const string DEMOCRACY_ROUTINE_NAME = "democracy";
        public const string INPUT_MODE_VOTE_ROUTINE_NAME = "inputmodevote";
    }
}
