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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TRBotDataMigrationTool
{
    /// <summary>
    /// The types of input accessors.
    /// </summary>
    public enum InputAccessTypes
    {
        Default
    }

    /// <summary>
    /// Access data for defined inputs.
    /// </summary>
    public sealed class InputAccessData
    {
        public readonly Dictionary<string, InputAccessInfo> InputAccessDict = new Dictionary<string, InputAccessInfo>(4)
        {
            { "ss1", new InputAccessInfo((int)AccessLevels.Levels.Moderator, InputAccessTypes.Default, null) },
            { "ss2", new InputAccessInfo((int)AccessLevels.Levels.Moderator, InputAccessTypes.Default, null) }
        };

        //Throw in here for now
        //THIS IS A QUICK HACKY FIX AND WE SHOULD FIND A BETTER WAY TO DO THIS
        public static bool HasAccessToInput(in int userLevel, in InputAccessInfo accessInfo)
        {
            return userLevel >= accessInfo.AccessLevel;
        }
    }

    /// <summary>
    /// Holds information about the accessibility of an input.
    /// </summary>
    public struct InputAccessInfo
    {
        public int AccessLevel;
        public InputAccessTypes AccessType;
        public object AccessVal;

        public InputAccessInfo(in int accessLevel, in InputAccessTypes accessType, object accessVal)
        {
            AccessLevel = accessLevel;
            AccessType = accessType;
            AccessVal = accessVal;
        }

        public override bool Equals(object obj)
        {
            if (obj is InputAccessInfo inputAccessInfo)
            {
                return (this == inputAccessInfo);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 47;
                hash = (hash * 37) + AccessLevel.GetHashCode();
                hash = (hash * 37) + AccessType.GetHashCode();
                hash = (hash * 37) + ((AccessVal == null) ? 0 : AccessVal.GetHashCode());
                return hash;
            } 
        }

        public static bool operator==(InputAccessInfo a, InputAccessInfo b)
        {
            return (a.AccessLevel == b.AccessLevel && a.AccessType == b.AccessType && a.AccessVal == b.AccessVal);
        }

        public static bool operator!=(InputAccessInfo a, InputAccessInfo b)
        {
            return !(a == b);
        }
    }
}
