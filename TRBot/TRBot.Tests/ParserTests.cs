using System.Collections.Generic;
using System.Collections.Concurrent;
using NUnit.Framework;
using TRBot;

namespace TRBot.Tests
{
    [TestFixture]
    public class ParserTests
    {
        /* For some tests, we need to build dictionaries and other structures ahead of time.
           Find a way to supply ALL the necessary data ahead of time so each test can focus on just one thing.
        */

        [TestCase("a", true, 60000)]
        [TestCase("b", true, 60000)]
        [TestCase("x", true, 60000)]
        [TestCase("jump", true, 60000)]
        [TestCase("y65s", true, 60000)] 
        [TestCase("r5200ms", true, 5000)] 
        public static void TestOneInput(string input, bool checkDur, int maxDur)
        {
            string inputRegex = Parser.BuildInputRegex(new string[] { input });

            Parser.InputSequence inputSequence = Parser.ParseInputs(input, inputRegex, new Parser.ParserOptions(0, 200, checkDur, maxDur));

            Assert.AreEqual(inputSequence.InputValidationType, inputSequence.TotalDuration <= maxDur ? Parser.InputValidationTypes.Valid : Parser.InputValidationTypes.Invalid);
            Assert.AreEqual(inputSequence.Inputs.Count, 1);
        }

        [TestCase("a . b", 600, 200, new string[] { "a", ".", "b" })]
        [TestCase("a . b", 900, 300, new string[] { "a", ".", "b" })]
        [TestCase("a200ms #200ms b200ms", 600, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1s", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000ms", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a2s", 2000, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a2000ms", 2000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000ms+b1000ms", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000ms b1000ms", 2000, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a500ms b200ms #1s", 1700, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a500ms b200ms #1s -a1300ms", 3000, 200, new string[] { "a", "#", "b" })]
        public void TestTotalDuration(string input, int expectedDur, int defaultDur, string[] validInputs)
        {
            string inputRegex = Parser.BuildInputRegex(validInputs);

            Parser.InputSequence inputSequence = Parser.ParseInputs(input, inputRegex, new Parser.ParserOptions(0, defaultDur, false));

            Assert.AreEqual(inputSequence.TotalDuration, expectedDur);
        }

        [TestCase("a", 1, new string[] { "a" })]
        [TestCase("a+b", 2, new string[] { "a", "b" })]
        [TestCase("a+b .", 3, new string[] { "a", "b", "." })]
        [TestCase("a . b+a", 4, new string[] { "a", "b", "." })]
        [TestCase("a+b start+a+b+x+y . . b . . a", 13, new string[] { "a", "b", "x", "y", "start", "." })]
        public static void TestInputCount(string input, int expectedInputCount, string[] validInputs)
        {
            string inputRegex = Parser.BuildInputRegex(validInputs);

            Parser.InputSequence inputSequence = Parser.ParseInputs(input, inputRegex, new Parser.ParserOptions(0, 200, false));

            int inputCount = 0;
            for (int i = 0; i < inputSequence.Inputs.Count; i++)
            {
                inputCount += inputSequence.Inputs[i].Count;
            }
            
            Assert.AreEqual(inputCount, expectedInputCount);
        }

        [TestCase("jump", "a", new string[] { "jump" }, new string[] { "a" })]
        [TestCase("slide", "_down b", new string[] { "slide" }, new string[] { "_down b" })]
        [TestCase("a . jump . x . pause", "a . a . x . start", new string[] { "jump", "pause" }, new string[] { "a", "start" })]
        public static void TestSynonyms(string input, string expectedOutput, string[] synonyms, string[] associatedInputs)
        {
            Dictionary<string, string> synonymDict = BuildDictWithArrays(synonyms, associatedInputs);

            string synonymsPopulate = Parser.PopulateSynonyms(input, synonymDict);

            Assert.AreEqual(synonymsPopulate, expectedOutput);
        }

        [TestCase("#atwo", "aa", new string[] { "#atwo" }, new string[] { "aa" })]
        [TestCase("#mashr", "r34ms #34ms r34ms #34ms r34ms #34ms", new string[] { "#mashr" }, new string[] { "r34ms #34ms r34ms #34ms r34ms #34ms" })]
        public static void TestMacros(string input, string expectedOutput, string[] macroNames, string[] macroInputs)
        {
            ConcurrentDictionary<string, string> macroDict = BuildConcurrentDictWithArrays(macroNames, macroInputs);
            Dictionary<char, List<string>> macroLookup = new Dictionary<char, List<string>>();

            DataInit.PopulateMacrosToParserList(macroDict, macroLookup);

            string macroInput = Parser.PopulateMacros(input, macroDict, macroLookup);

            Assert.AreEqual(macroInput, expectedOutput);
        }

        # region Utility

        private static Dictionary<T, U> BuildDictWithArrays<T, U>(T[] array1, U[] array2)
        {
            Dictionary<T, U> dict = new Dictionary<T, U>(array1.Length);

            for (int i = 0; i < array1.Length; i++)
            {
                dict[array1[i]] = array2[i];
            }

            return dict;
        }

        private static ConcurrentDictionary<T, U> BuildConcurrentDictWithArrays<T, U>(T[] array1, U[] array2)
        {
            ConcurrentDictionary<T, U> dict = new ConcurrentDictionary<T, U>(1, array1.Length);

            for (int i = 0; i < array1.Length; i++)
            {
                dict[array1[i]] = array2[i];
            }

            return dict;    
        }

        #endregion
    }
}