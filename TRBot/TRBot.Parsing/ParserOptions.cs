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
    /// Options for the parser.
    /// </summary>
    public struct ParserOptions
    {
        /// <summary>
        /// The default controller port to use for each parsed input.
        /// </summary>
        public int DefaultControllerPort;

        /// <summary>
        /// The default input duration to use for each parsed input.
        /// </summary>
        public int DefaultInputDur;

        /// <summary>
        /// Whether to cancel parsing early if a given maximum input duration has been exceeded.
        /// </summary>
        public bool CheckMaxDur;
        
        /// <summary>
        /// The maximum total input duration the parser will parse.
        /// </summary>
        public int MaxInputDur;

        public ParserOptions(in int defControllerPort, in int defaultInputDur, in bool checkMaxDur)
            : this(defControllerPort, defaultInputDur, checkMaxDur, 0)
        {
        }

        public ParserOptions(in int defControllerPort, in int defaultInputDur,
            in bool checkMaxDur, in int maxInputDur)
        {
            DefaultControllerPort = defControllerPort;
            DefaultInputDur = defaultInputDur;
            CheckMaxDur = checkMaxDur;
            MaxInputDur = maxInputDur;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParserOptions inpSeq)
            {
                return (this == inpSeq);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                hash = (hash * 37) + DefaultControllerPort.GetHashCode();
                hash = (hash * 37) + DefaultInputDur.GetHashCode();
                hash = (hash * 37) + CheckMaxDur.GetHashCode();
                hash = (hash * 37) + MaxInputDur.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ParserOptions a, ParserOptions b)
        {
            return (a.DefaultControllerPort == b.DefaultControllerPort && a.DefaultInputDur == b.DefaultInputDur
                    && a.CheckMaxDur == b.CheckMaxDur && a.MaxInputDur == b.MaxInputDur);
        }
        
        public static bool operator !=(ParserOptions a, ParserOptions b)
        {
            return !(a == b);
        }
    }
}