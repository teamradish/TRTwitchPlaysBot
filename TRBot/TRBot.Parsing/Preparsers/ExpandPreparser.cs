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
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TRBot.Parsing
{
    /// <summary>
    /// A pre-parser that expands repetitions in an input sequence.
    /// </summary>
    public class ExpandPreparser : IPreparser
    {
        public ExpandPreparser()
        {

        }

        /// <summary>
        /// Pre-parses a string to prepare it for the parser.
        /// </summary>
        /// <param name="message">The message to pre-parse.</param>
        /// <returns>A string containing the modified message.</returns>
        public string Preparse(string message)
        {
            string newMessage = message;
            StringBuilder strBuilder = null;

            const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

            //Find text within brackets with a number of repetitions
            //Get the innermost matches first

            //Uncomment these stopwatch lines to time it
            //Stopwatch sw = Stopwatch.StartNew();

            const string regex = @"\[([^\[\]]*\])\*(\d{1,3})";
            Match m = Regex.Match(newMessage, regex, regexOptions);
            
            while (m.Success == true)
            {
                //Initialize StringBuilder if not initialized
                if (strBuilder == null)
                {
                    strBuilder = new StringBuilder(message, message.Length * 3);
                }

                //Get the group
                Group containedGroup = m.Groups[1];
                string containedStr = containedGroup.Value;
                if (containedGroup.Success == false)
                {
                    return newMessage;
                }

                //Get the number of repetitions
                Group repetitionsGroup = m.Groups[2];
                string repetitionsStr = repetitionsGroup.Value;
                if (repetitionsGroup.Success == false || string.IsNullOrEmpty(repetitionsStr) == true)
                {
                    return newMessage;
                }

                if (int.TryParse(repetitionsStr, out int num) == false)
                {
                    return newMessage;
                }

                //Remove opening bracket
                strBuilder.Remove(containedGroup.Index - 1, 1);
                //Console.WriteLine("Opening removed: " + strBuilder.ToString());
                
                //If repetitions is 0 or lower, remove the input entirely
                if (num <= 0)
                {
                    strBuilder.Remove(containedGroup.Index - 1, containedGroup.Length + 1 + repetitionsStr.Length);
                    //Console.WriteLine("Zero Str: " + strBuilder.ToString());
                }
                else
                {
                    //Console.WriteLine("Str Length: " + strBuilder.ToString().Length);

                    int contentsStart = containedGroup.Index - 1;
                    int contentsLength = containedGroup.Length - 1;

                    //Remove closing bracket, symbol, and repetitions
                    int start = contentsStart + contentsLength;
                    int removeAmt = 1 + 1 + repetitionsStr.Length;

                    //Console.WriteLine("start: " + strt + " | removeAmt: " + removeAmt);

                    strBuilder.Remove(start, 1 + 1 + repetitionsStr.Length);
                    //Console.WriteLine("CB, S, R removed: " + strBuilder.ToString());

                    //Trim the end of the string so we can add just the text within the brackets
                    string containedStrTrimmed = containedStr.Substring(0, containedStr.Length - 1);

                    //Repeat sequence if greater than 1 to expand it out
                    //Ex. "[a500ms .]*2" should expand out to "a500ms . a500ms ."
                    if (num > 1)
                    {
                        int newStart = contentsStart + (contentsLength * 1);
                        //Console.WriteLine("NEW START: " + newStart + " | INSERTING " + "\"" + containedStr + "\"");

                        //Insert string - the StringBuilder can accurately allocate enough additional memory with this overload
                        strBuilder.Insert(newStart, containedStrTrimmed, num - 1);
                    }

                    //Console.WriteLine("INSERTED: " + strBuilder.ToString());
                }

                newMessage = strBuilder.ToString();

                //Find the next match in case this is a nested repetition
                m = Regex.Match(newMessage, regex, regexOptions);
            }

            //sw.Stop();
            //Console.WriteLine($"SW MS for Expandify: {sw.ElapsedMilliseconds}");

            return newMessage;
        }
    }
}