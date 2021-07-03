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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRBot.Misc
{
    /// <summary>
    /// A Markov chain text generator. This can be used to create random sentences from sample data.
    /// </summary>
    public class MarkovChainTextGenerator
    {
        /// <summary>
        /// The number of words to use for each prefix.
        /// </summary>
        public int PrefixWordCount = 2;
        
        /// <summary>
        /// The maximum number of characters for the generated sentence.
        /// </summary>
        public int CharacterLimit = 500;

        /// <summary>
        /// The minimum number of prefixes required to generate a sentence.
        /// </summary>
        public int MinPrefixGenCount = 5;

        private Random Rand = new Random();

        public MarkovChainTextGenerator()
        {

        }

        /// <summary>
        /// Generates text given sample data.
        /// </summary>
        /// <param name="sampleData">Sample data for the text generator. This is often a bunch of words, which can either be structured or not structured.</param>
        /// <returns>A string generated from the sample data. If there isn't enough sample data, this returns an empty string.</returns>
        public string GenerateText(string sampleData)
        {
            //Not enough data
            if (string.IsNullOrEmpty(sampleData) == true)
            {
                return string.Empty;
            }

            //Split by space
            string[] splitData = sampleData.Split(' ', StringSplitOptions.None);

            //Not enough data
            if (splitData.Length < (PrefixWordCount + 1))
            {
                return string.Empty;
            }

            //Fetch our markov chain
            Dictionary<string, List<string>> markovChain = BuildPrefixSuffixDict(splitData);

            //Too few prefixes
            if (markovChain.Count < MinPrefixGenCount)
            {
                return string.Empty;
            }

            StringBuilder strBuilder = new StringBuilder(CharacterLimit);

            //Start with a random prefix
            int randPrefixIndex = Rand.Next(0, markovChain.Count);

            string prefix = markovChain.ElementAt(randPrefixIndex).Key;

            //Go through, appending a random suffix belonging to the prefix
            //Step one at a time, trimming the first word from the previous prefix
            //Continue until there's no suffix or the string will go over the character limit
            while (true)
            {
                //No chain here - break
                if (markovChain.TryGetValue(prefix, out List<string> suffixes) == false)
                {
                    break;
                }

                //Append the first prefix
                if (strBuilder.Length == 0)
                {
                    strBuilder.Append(prefix).Append(' ');
                }

                string randSuffix = suffixes[Rand.Next(0, suffixes.Count)];

                //TRBotLogger.Logger.Information($"For prefix \"{prefix}\" chose suffix \"{randSuffix}\"");

                int curTotalLength = strBuilder.Length + randSuffix.Length + 1;

                //The new string would go over the character limit, so exit
                if (curTotalLength > CharacterLimit)
                {
                    break;
                }

                strBuilder.Append(randSuffix).Append(' ');

                //Get the new prefix
                if (PrefixWordCount > 1)
                {
                    int spaceIndex = prefix.IndexOf(' ');

                    //Somehow there's no space in the prefix - no choice but to back out
                    if (spaceIndex < 0)
                    {
                        break;
                    }

                    //Exclude the first item in the prefix
                    //For example, if prefix = "The cat" and suffix = "is", the sentence would be "The cat is"
                    //The new prefix should then be "cat is"
                    prefix = prefix.Substring(spaceIndex + 1) + " " + randSuffix;
                }
                //If there's only one prefix, set the suffix chosen as the new prefix
                else
                {
                    prefix = randSuffix;
                }

                //TRBotLogger.Logger.Information($"NEW PREFIX = \"{prefix}\"");
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Builds the dictionary of prefixes and suffixes that follow for the text generator.
        /// </summary>
        /// <param name="sampleTerms">An array of words to build the dictionary with.</param>
        /// <returns>A dictionary in which the keys are prefixes and the values are lists of suffixes that follow the prefixes.</returns>
        private Dictionary<string, List<string>> BuildPrefixSuffixDict(string[] sampleTerms)
        {
            Dictionary<string, List<string>> chainDict = new Dictionary<string, List<string>>(sampleTerms.Length);

            StringBuilder strBuilder = new StringBuilder(PrefixWordCount * 16);

            for (int i = 0; i < sampleTerms.Length; i++)
            {
                int suffixIndex = i + PrefixWordCount;

                //No suffix available
                if (suffixIndex >= sampleTerms.Length)
                {
                    break;
                }

                strBuilder.Clear();

                //Get the prefix
                strBuilder.Append(sampleTerms[i]);

                for (int j = 1; j < PrefixWordCount; j++)
                {
                    strBuilder.Append(' ').Append(sampleTerms[i + j]);
                }

                string prefix = strBuilder.ToString();

                //Get the suffix
                string suffix = sampleTerms[i + PrefixWordCount];

                //Check if the prefix is in the dictionary and add it if not
                if (chainDict.TryGetValue(prefix, out List<string> suffixes) == false)
                {
                    suffixes = new List<string>();
                    chainDict.Add(prefix, suffixes);
                }
                //else
                //{
                //    TRBotLogger.Logger.Information($"Found prefix \"{prefix}\" - adding suffix \"{suffix}\"");
                //}
                
                //Don't add duplicate suffixes
                if (suffixes.Contains(suffix) == false)
                {
                    suffixes.Add(suffix);
                }
            }

            return chainDict;
        }
    }
}
