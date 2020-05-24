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
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Input synonym data.
    /// </summary>
    public sealed class InputSynonymData
    {
        /// <summary>
        /// The synonyms associated with certain inputs, per console.
        /// </summary>
        public readonly Dictionary<InputGlobals.InputConsoles, Dictionary<string, string>> SynonymDict
            = new Dictionary<InputGlobals.InputConsoles, Dictionary<string, string>>(9)
        {
            {
                InputGlobals.InputConsoles.NES,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.SNES,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.Genesis,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.N64,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.GC,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.PS2,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.GBA,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.Wii,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputGlobals.InputConsoles.PC,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            }
        };
    }
}
