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
using System.Collections;
using System.Collections.Generic;

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents a fully parsed input sequence.
    /// </summary>
    public struct ParsedInputSequence
    {
        public ParsedInputResults ParsedInputResult;
        public List<List<ParsedInput>> Inputs;
        public int TotalDuration;
        public string Error;

        public ParsedInputSequence(in ParsedInputResults parsedInputResult, List<List<ParsedInput>> inputs, in int totalDuration, string error)
        {
            ParsedInputResult = parsedInputResult;
            Inputs = inputs;
            TotalDuration = totalDuration;
            Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParsedInputSequence inpSeq)
            {
                return (this == inpSeq);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 37) + ParsedInputResult.GetHashCode();
                hash = (hash * 37) + ((Inputs == null) ? 0 : Inputs.GetHashCode());
                hash = (hash * 37) + TotalDuration.GetHashCode();
                hash = (hash * 37) + ((Error == null) ? 0 : Error.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(ParsedInputSequence a, ParsedInputSequence b)
        {
            return (a.ParsedInputResult == b.ParsedInputResult
                    && a.Inputs == b.Inputs && a.TotalDuration == b.TotalDuration && a.Error == b.Error);
        }

        public static bool operator !=(ParsedInputSequence a, ParsedInputSequence b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            int inputCount = (Inputs == null) ? 0 : Inputs.Count;
            return $"VType:{ParsedInputResult} | SubInputs:{inputCount} | Duration:{TotalDuration} | Err:{Error}";
        }
    }
}