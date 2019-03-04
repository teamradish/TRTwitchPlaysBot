/* Original Python parser for TPE written in Python by: Jdog, aka TwitchPlays_Everything
 * Converted to C# by: Kimimaru, aka Kimimaru4000
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace KimimaruBot
{
    /// <summary>
    /// The parser for input.
    /// </summary>
    public static class Parser
    {
        //SNES
        private static readonly string[] VALID_INPUTS = new string[]
        {
            "left", "right", "up", "down",
            "a", "b", "l", "r", "x", "y",
            "start", "select",
            "#", "."
        };

        //N64
        /*
         private static readonly string[] VALID_INPUTS = new string[]
         {
             "left", "right", "up", "down",
             "dleft", "dright", "dup", "ddown",
             "cleft," "cright", "cup", "cdown",
             "a", "b", "l", "r", "z",
             "start",
             "#", "."
         };
        */

        //Wii
        /*
         private static readonly string[] VALID_INPUTS = new string[]
         {
             "left", "right", "up", "down",
             "pleft", "pright", "pup", "pdown",
             "tleft," "tright", "tup", "tdown",
             "a", "b", "one", "two", "minus", "plus",
             "c", "z",
             "shake", "point",
             "#", "."
         };
        */

        //GC
        /*
         private static readonly string[] VALID_INPUTS = new string[]
         {
             "left", "right", "up", "down",
             "dleft", "dright", "dup", "ddown",
             "cleft," "cright", "cup", "cdown",
             "a", "b", "l", "r", "x", "y", "z",
             "start",
             "#", "."
         };
        */

        private const int DURATION_DEFAULT = 200;
        private const int DURATION_MAX = 60000;

        /// <summary>
        /// Contains input data.
        /// </summary>
        public class Input
        {
            public string name = string.Empty;
            public bool hold = false;
            public bool release = false;
            public int percent = 100;
            public int duration = DURATION_DEFAULT;
            public string duration_type = "ms";
            public int length = 0;
            public string error = string.Empty;

            public override string ToString()
            {
                return $"\"{name}\" for {duration}{duration_type} | IsHeld: {hold} | IsRelease: {release} | Percent: {percent} | Error: {error}";
            }

            public static readonly Dictionary<string, string> INPUT_SYNONYMS = new Dictionary<string, string>()
            {
                //{ "pause", "start" }
                { "kappa", "#" }
            };

            public string populate_synonyms(string message)
            {
                foreach (string synonym in INPUT_SYNONYMS.Keys)
                {
                    message = message.Replace(synonym, INPUT_SYNONYMS[synonym]);
                }

                return message;
            }

            //Returns Input object
            private static Input get_input(string message)
            {
                //Create a default input instance
                Input current_input = new Input();

                string regex = @"^[_-]";
                Match m = Regex.Match(message, regex);

                //If there's a match, trim the message
                if (m.Success == true)
                {
                    string c = message.Substring(m.Index, m.Length);
                    message = message.Substring(m.Length);

                    if (c == "_")
                    {
                        current_input.hold = true;
                        current_input.length += 1;
                    }
                    else if (c == "-")
                    {
                        current_input.release = true;
                        current_input.length += 1;
                    }
                }

                //Try to match one input, prioritizing the longest match
                int max = 0;
                string valid_input = string.Empty;

                foreach (string button in VALID_INPUTS)
                {
                    if (button == ".")
                        regex = @"\.";
                    else
                        regex = $"^{button}";

                    m = Regex.Match(message, regex);

                    if (m.Success == true)
                    {
                        int length = m.Length - m.Index;

                        if (length > max)
                        {
                            max = length;
                            current_input.name = message.Substring(m.Index, m.Length);
                        }
                    }
                }

                //If not a valid input, break parsing
                if (string.IsNullOrEmpty(current_input.name) == true)
                {
                    current_input.error = "ERR_INVALID_INPUT";

                    return current_input;
                }
                else
                    current_input.length += max;

                //Trim the input from the message
                message = message.Substring(max);

                //Try to match a percent
                regex = @"^\d+%";
                m = Regex.Match(message, regex);

                if (m.Success == true)
                {
                    current_input.percent = int.Parse(message.Substring(m.Index, m.Length - 1));
                    message = message.Substring(m.Length);
                    current_input.length += current_input.percent.ToString().Length + 1;

                    if (current_input.percent > 100)
                    {
                        current_input.error = "ERR_INVALID_PERCENTAGE";
                        return current_input;
                    }
                }

                //Try to match a duration
                regex = @"^\d+";
                m = Regex.Match(message, regex);

                if (m.Success == true)
                {
                    current_input.duration = int.Parse(message.Substring(m.Index, m.Length));
                    message = message.Substring(m.Length);
                    current_input.length += current_input.duration.ToString().Length;

                    //Determine the type of duration
                    regex = @"(s|ms)";
                    m = Regex.Match(message, regex);

                    if (m.Success == true)
                    {
                        current_input.duration_type = message.Substring(m.Index, m.Length);
                        message = message.Substring(m.Length);

                        if (current_input.duration_type == "s")
                        {
                            current_input.duration *= 1000;
                            current_input.length += 1;
                        }
                        else
                            current_input.length += 2;
                    }
                    else
                    {
                        current_input.error = "ERR_DURATION_TYPE_UNSPECIFIED";
                        return current_input;
                    }
                }

                if (current_input.name == "start" && current_input.duration >= 500)
                {
                    current_input.error = "ERR_START_BUTTON_DURATION_MAX_EXCEEDED";
                    return current_input;
                }

                return current_input;
            }

            //Returns list containing: [Valid, input_sequence]
            //Or: [Invalid, input that it failed on]
            public List<object> Parse(string message)
            {
                bool contains_start_input = false;
                message = message.Replace(" ", string.Empty).ToLower();
                List<Input> input_subsequence = new List<Input>();
                List<Input> input_sequence = new List<Input>();
                int duration_counter = 0;

                message = populate_synonyms(message);

                while (message.Length > 0)
                {
                    input_subsequence = new List<Input>();
                    int subduration_max = 0;
                    Input current_input = get_input(message);

                    /*
                     * if (current_input.name == "plus")
                     *     contains_start_input = true;
                     */

                    if (string.IsNullOrEmpty(current_input.error) == false)
                        return new List<object>() { false, current_input };

                    message = message.Substring(current_input.length);
                    input_subsequence.Add(current_input);

                    if (current_input.duration > subduration_max)
                        subduration_max = current_input.duration;

                    if (message.Length > 0)
                    {
                        while(message[0] == '+')
                        {
                            if (message.Length > 0)
                                message = message.Substring(1);
                            else
                                break;

                            current_input = get_input(message);

                            /*
                             * if (current_input.name == "plus")
                             *     contains_start_input = true;
                             */

                            if (string.IsNullOrEmpty(current_input.error) == false)
                                return new List<object>() { false, current_input };

                            message = message.Substring(current_input.length);
                            input_sequence.Add(current_input);

                            if (current_input.duration > subduration_max)
                                subduration_max = current_input.duration;

                            if (message.Length == 0)
                                break;
                        }
                    }

                    duration_counter += subduration_max;

                    if (duration_counter > DURATION_MAX)
                    {
                        current_input.error = "ERR_DURATION_MAX";
                        return new List<object>() { false, current_input };
                    }

                    input_sequence.AddRange(input_subsequence);
                }

                return new List<object>() { true, input_sequence, contains_start_input, duration_counter };
            }
        }
    }
}
