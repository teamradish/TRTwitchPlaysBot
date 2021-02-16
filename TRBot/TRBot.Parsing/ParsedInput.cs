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

namespace TRBot.Parsing
{
    /// <summary>
    /// Represents a parsed input.
    /// </summary>
    public struct ParsedInput
    {
        public string Name;
        public bool Hold;
        public bool Release;
        public double Percent;
        public int Duration;
        public InputDurationTypes DurationType;
        public int ControllerPort;
        public string Error;

        /// <summary>
        /// Returns a default Input.
        /// </summary>
        public static ParsedInput Default(in int defaultInputDur) => new ParsedInput(string.Empty, false, false, Parser.PARSER_DEFAULT_PERCENT, defaultInputDur, InputDurationTypes.Milliseconds, 0, string.Empty);
        
        public ParsedInput(string nme, in bool hld, in bool relse, in double percnt, in int dur, InputDurationTypes durType, in int contPort, in string err)
        {
            this.Name = nme;
            this.Hold = hld;
            this.Release = relse;
            this.Percent = percnt;
            this.Duration = dur;
            this.DurationType = durType;
            this.ControllerPort = contPort;
            this.Error = string.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParsedInput inp)
            {
                return (this == inp);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                hash = (hash * 37) + ((Name == null) ? 0 : Name.GetHashCode());
                hash = (hash * 37) + Hold.GetHashCode();
                hash = (hash * 37) + Release.GetHashCode();
                hash = (hash * 37) + Percent.GetHashCode();
                hash = (hash * 37) + Duration.GetHashCode();
                hash = (hash * 37) + DurationType.GetHashCode();
                hash = (hash * 37) + ControllerPort.GetHashCode();
                hash = (hash * 37) + ((Error == null) ? 0 : Error.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(ParsedInput a, ParsedInput b)
        {
            return (a.Hold == b.Hold && a.Release == b.Release && a.Percent == b.Percent
                    && a.DurationType == b.DurationType && a.DurationType == b.DurationType
                    && a.Name == b.Name && a.ControllerPort == b.ControllerPort && a.Error == b.Error);
        }

        public static bool operator !=(ParsedInput a, ParsedInput b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"\"{Name}\" {Duration}{DurationType} | H:{Hold} | R:{Release} | P:{Percent} | CPort:{ControllerPort} | Err: \"{Error}\"";
        }
    }
}