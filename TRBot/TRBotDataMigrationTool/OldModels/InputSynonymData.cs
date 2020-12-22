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

namespace TRBotDataMigrationTool
{
    /// <summary>
    /// Input synonym data.
    /// </summary>
    public sealed class InputSynonymData
    {
        /// <summary>
        /// The synonyms associated with certain inputs, per console.
        /// </summary>
        public readonly Dictionary<InputConsoles, Dictionary<string, string>> SynonymDict
            = new Dictionary<InputConsoles, Dictionary<string, string>>(9)
        {
            {
                InputConsoles.NES,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.SNES,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.Genesis,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.N64,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.GC,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.PS2,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.GBA,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.Wii,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            },
            {
                InputConsoles.PC,
                    new Dictionary<string, string>(1)
                    {
                        { ".", "#" }
                    }
            }
        };
    }
}
