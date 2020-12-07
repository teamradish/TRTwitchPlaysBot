/* This file is part of TRBot.
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
        public string name;
        public bool hold;
        public bool release;
        public int percent;
        public int duration;
        public string duration_type;
        public int controllerPort;
        public string error;

        /// <summary>
        /// Returns a default Input.
        /// </summary>
        public static ParsedInput Default(in int defaultInputDur) => new ParsedInput(string.Empty, false, false, Parser.PARSER_DEFAULT_PERCENT, defaultInputDur, Parser.PARSER_DEFAULT_DUR_TYPE, 0, string.Empty);
        
        public ParsedInput(string nme, in bool hld, in bool relse, in int percnt, in int dur, string durType, in int contPort, in string err)
        {
            this.name = nme;
            this.hold = hld;
            this.release = relse;
            this.percent = percnt;
            this.duration = dur;
            this.duration_type = durType;
            this.controllerPort = contPort;
            this.error = string.Empty;
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
                hash = (hash * 37) + ((name == null) ? 0 : name.GetHashCode());
                hash = (hash * 37) + hold.GetHashCode();
                hash = (hash * 37) + release.GetHashCode();
                hash = (hash * 37) + percent.GetHashCode();
                hash = (hash * 37) + duration.GetHashCode();
                hash = (hash * 37) + ((duration_type == null) ? 0 : duration_type.GetHashCode());
                hash = (hash * 37) + controllerPort.GetHashCode();
                hash = (hash * 37) + ((error == null) ? 0 : error.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(ParsedInput a, ParsedInput b)
        {
            return (a.hold == b.hold && a.release == b.release && a.percent == b.percent
                    && a.duration_type == b.duration_type && a.duration_type == b.duration_type
                    && a.name == b.name && a.controllerPort == b.controllerPort && a.error == b.error);
        }

        public static bool operator !=(ParsedInput a, ParsedInput b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"\"{name}\" {duration}{duration_type} | H:{hold} | R:{release} | P:{percent} | CPort:{controllerPort} | Err:{error}";
        }
    }
}