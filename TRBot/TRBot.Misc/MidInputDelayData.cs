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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRBot.Parsing;

namespace TRBot.Misc
{
    /// <summary>
    /// Contains data regarding mid-input delays.
    /// </summary>
    public struct MidInputDelayData
    {
        public List<List<ParsedInput>> NewInputs;
        public int NewTotalDuration;
        public bool Success;
        public string Message;

        public MidInputDelayData(List<List<ParsedInput>> newInputs, in int newTotalDuration, in bool success, string message)
        {
            NewInputs = newInputs;
            NewTotalDuration = newTotalDuration;
            Success = success;
            Message = message;
        }

        public override bool Equals(object obj)
        {
            if (obj is MidInputDelayData postInpDelayData)
            {
                return (this == postInpDelayData);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7;
                hash = (hash * 37) + ((NewInputs == null) ? 0 : NewInputs.GetHashCode());
                hash = (hash * 37) + NewTotalDuration.GetHashCode();
                hash = (hash * 37) + Success.GetHashCode();
                hash = (hash * 37) + ((Message == null) ? 0 : Message.GetHashCode());
                return hash;
            }
        }

        public static bool operator==(MidInputDelayData a, MidInputDelayData b)
        {
            return (a.NewInputs == b.NewInputs && a.NewTotalDuration == b.NewTotalDuration
                && a.Success == b.Success && a.Message == b.Message);
        }

        public static bool operator!=(MidInputDelayData a, MidInputDelayData b)
        {
            return !(a == b);
        }
    }
}
