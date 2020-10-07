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
using TRBot.Parsing;
using TRBot.VirtualControllers;

namespace TRBot.Consoles
{
    /// <summary>
    /// A custom console.
    /// </summary>
    public sealed class CustomConsole : GameConsole
    {
        public CustomConsole()
        {
            Name = "custom";

            Initialize();

            UpdateInputRegex();
        }

        private void Initialize()
        {
            SetConsoleInputs(new Dictionary<string, InputData>(22)
            {
                { "left", InputData.CreateButton("left", (int)GlobalButtonVals.BTN3) },         { "l", InputData.CreateButton("l", (int)GlobalButtonVals.BTN3) },
                { "right", InputData.CreateButton("right", (int)GlobalButtonVals.BTN4) },       { "r", InputData.CreateButton("r", (int)GlobalButtonVals.BTN4) },
                { "up", InputData.CreateButton("up", (int)GlobalButtonVals.BTN1) },             { "u", InputData.CreateButton("u", (int)GlobalButtonVals.BTN1) },
                { "down", InputData.CreateButton("down", (int)GlobalButtonVals.BTN2) },         { "d", InputData.CreateButton("d", (int)GlobalButtonVals.BTN2) },
                { "grab", InputData.CreateButton("grab", (int)GlobalButtonVals.BTN5) },         { "g", InputData.CreateButton("g", (int)GlobalButtonVals.BTN5) },
                { "select", InputData.CreateButton("select", (int)GlobalButtonVals.BTN6) },     { "s", InputData.CreateButton("s", (int)GlobalButtonVals.BTN6) },
                { "pause", InputData.CreateButton("pause", (int)GlobalButtonVals.BTN7) },       { "p", InputData.CreateButton("p", (int)GlobalButtonVals.BTN7) },
                { "restart", InputData.CreateButton("restart", (int)GlobalButtonVals.BTN8) },
                { "undo", InputData.CreateButton("undo", (int)GlobalButtonVals.BTN9) },
                { "back", InputData.CreateButton("back", (int)GlobalButtonVals.BTN10) },        { "b", InputData.CreateButton("b", (int)GlobalButtonVals.BTN10) },
                { "viewmap", InputData.CreateButton("viewmap", (int)GlobalButtonVals.BTN11) },  { "v", InputData.CreateButton("v", (int)GlobalButtonVals.BTN11) },
                { "#", InputData.CreateBlank("#") }
            });
            //InputAxesMap = new Dictionary<string, InputAxis>();

            //InputButtonMap = new Dictionary<string, InputButton>(21)
            //{
            //    { "left", new InputButton((int)GlobalButtonVals.BTN3) },        { "l", new InputButton((int)GlobalButtonVals.BTN3) },
            //    { "right", new InputButton((int)GlobalButtonVals.BTN4) },       { "r", new InputButton((int)GlobalButtonVals.BTN4) },
            //    { "up", new InputButton((int)GlobalButtonVals.BTN1) },          { "u", new InputButton((int)GlobalButtonVals.BTN1) },
            //    { "down", new InputButton((int)GlobalButtonVals.BTN2) },        { "d", new InputButton((int)GlobalButtonVals.BTN2) },
            //    { "grab", new InputButton((int)GlobalButtonVals.BTN5) },        { "g", new InputButton((int)GlobalButtonVals.BTN5) },
            //    { "select", new InputButton((int)GlobalButtonVals.BTN6) },      { "s", new InputButton((int)GlobalButtonVals.BTN6) },
            //    { "pause", new InputButton((int)GlobalButtonVals.BTN7) },       { "p", new InputButton((int)GlobalButtonVals.BTN7) }, { "start", new InputButton((int)GlobalButtonVals.BTN7) },
            //    { "restart", new InputButton((int)GlobalButtonVals.BTN8) },
            //    { "undo", new InputButton((int)GlobalButtonVals.BTN9) },
            //    { "back", new InputButton((int)GlobalButtonVals.BTN10) },       { "b", new InputButton((int)GlobalButtonVals.BTN10) },
            //    { "viewmap", new InputButton((int)GlobalButtonVals.BTN11) },    { "v", new InputButton((int)GlobalButtonVals.BTN11) },
            //};

            //ValidInputs = new List<string>(22)
            //{
            //    "up", "u", "down", "d", "left", "l", "right", "r", "grab", "g",
            //    "select", "s", "pause", "p", "start", "restart", "undo", "back", "b", "viewmap", "v", 
            //    "#"
            //};
        }
    }
}
