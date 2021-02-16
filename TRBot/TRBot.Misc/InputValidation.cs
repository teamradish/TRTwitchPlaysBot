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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Misc
{
    /// <summary>
    /// Describes input validation.
    /// </summary>
    public struct InputValidation
    {
        public InputValidationTypes InputValidationType;
        public string Message;

        public InputValidation(in InputValidationTypes inputValidationType, string message)
        {
            InputValidationType = inputValidationType;
            Message = message;
        }

        public override bool Equals(object obj)
        {
            if (obj is InputValidation inpVald)
            {
                return (this == inpVald);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 37) + InputValidationType.GetHashCode();
                hash = (hash * 37) + ((Message == null) ? 0 : Message.GetHashCode());
                return hash;
            }
        }

        public static bool operator==(InputValidation a, InputValidation b)
        {
            return (a.InputValidationType == b.InputValidationType && a.Message == b.Message);
        }

        public static bool operator!=(InputValidation a, InputValidation b)
        {
            return !(a == b);
        }
    }
}
