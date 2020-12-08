using System.Collections.Generic;
using System.Collections.Concurrent;
using NUnit.Framework;
using TRBot;

namespace TRBot.Tests
{
    [TestFixture]
    public class ParserUnitTests
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
        public void TestOneInput(string input, bool checkDur, int maxDur)
        {
            string inputRegex = Parser.BuildInputRegex(new string[] { input });

            Parser.InputSequence inputSequence = Parser.ParseInputs(input, inputRegex, new Parser.ParserOptions(0, 200, checkDur, maxDur));

            Assert.AreEqual(inputSequence.InputValidationType, inputSequence.TotalDuration <= maxDur ? Parser.InputValidationTypes.Valid : Parser.InputValidationTypes.Invalid);
            Assert.AreEqual(inputSequence.Inputs.Count, 1);
        }

        [TestCase("a200ms", new int[] { 200 }, new string[] { "a" })]
        [TestCase("a1000ms", new int[] { 1000 }, new string[] { "a" })]
        [TestCase("b1s", new int[] { 1000 }, new string[] { "b" })]
        [TestCase("a500ms b748ms", new int[] { 500, 748 }, new string[] { "a", "b" })]
        [TestCase("a3256ms #8s r150s", new int[] { 3256, 8000, 150000 }, new string[] { "a", "r", "#" })]
        [TestCase("_a17ms -x34ms y1024ms+b512ms", new int[] { 17, 34, 1024, 512 }, new string[] { "a", "x", "y", "b" })]
        public void TestSubInputDuration(string inputStr, int[] expectedDuration, string[] validInputs)
        {
            string inputRegex = Parser.BuildInputRegex(validInputs);

            Parser.InputSequence inputSequence = Parser.ParseInputs(inputStr, inputRegex, new Parser.ParserOptions(0, 200, false, 0));

            int expectedDurIndex = 0;
            for (int i = 0; i < inputSequence.Inputs.Count; i++)
            {
                for (int j = 0; j < inputSequence.Inputs[i].Count; j++)
                {
                    Parser.Input input = inputSequence.Inputs[i][j];
                    
                    Assert.AreEqual(input.duration, expectedDuration[expectedDurIndex]);
                    expectedDurIndex++;
                }
            }
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
        public void TestInputCount(string input, int expectedInputCount, string[] validInputs)
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
        public void TestSynonyms(string input, string expectedOutput, string[] synonyms, string[] associatedInputs)
        {
            Dictionary<string, string> synonymDict = BuildDictWithArrays(synonyms, associatedInputs);

            string synonymsPopulate = Parser.PopulateSynonyms(input, synonymDict);

            Assert.AreEqual(synonymsPopulate, expectedOutput);
        }

        [TestCase("#atwo", "aa", new string[] { "#atwo" }, new string[] { "aa" })]
        [TestCase("#mashr", "r34ms #34ms r34ms #34ms r34ms #34ms", new string[] { "#mashr" }, new string[] { "r34ms #34ms r34ms #34ms r34ms #34ms" })]
        [TestCase("#mash(a)", "[a34ms#34ms]*20", new string[] { "#mash(*)" }, new string[] { "[<0>34ms#34ms]*20" })]
        [TestCase("#mashalt(a,b)", "[a34ms#34msb34ms#34ms]*20", new string[] { "#mashalt(*,*)" }, new string[] { "[<0>34ms#34ms<1>34ms#34ms]*20" })]
        public void TestMacros(string input, string expectedOutput, string[] macroNames, string[] macroInputs)
        {
            ConcurrentDictionary<string, string> macroDict = BuildConcurrentDictWithArrays(macroNames, macroInputs);
            Dictionary<char, List<string>> macroLookup = new Dictionary<char, List<string>>();

            DataInit.PopulateMacrosToParserList(macroDict, macroLookup);
 
            string macroInput = Parser.PopulateMacros(input, macroDict, macroLookup);

            Assert.AreEqual(macroInput, expectedOutput);
        }

        [TestCase("[a]*2", "aa")]
        [TestCase("[a3s#17ms]*2", "a3s#17msa3s#17ms")]
        [TestCase("r[#17msa+b]*2b", "r#17msa+b#17msa+bb")]
        [TestCase("[a#17ms[b]*3x]*2", "a#17msbbbxa#17msbbbx")]
        public void TestExpandify(string input, string expectedOutput)
        {
            string expandify = Parser.Expandify(input);

            Assert.AreEqual(expandify, expectedOutput);
        }

        # region Utility

        private Dictionary<T, U> BuildDictWithArrays<T, U>(T[] array1, U[] array2)
        {
            Dictionary<T, U> dict = new Dictionary<T, U>(array1.Length);

            for (int i = 0; i < array1.Length; i++)
            {
                dict[array1[i]] = array2[i];
            }

            return dict;
        }

        private ConcurrentDictionary<T, U> BuildConcurrentDictWithArrays<T, U>(T[] array1, U[] array2)
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