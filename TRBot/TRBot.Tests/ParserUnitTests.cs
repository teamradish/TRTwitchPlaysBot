using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using TRBot.Parsing;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Tests
{
    [TestFixture]
    public class ParserUnitTests
    {
        /* For some tests, we need to build dictionaries and other structures ahead of time.
           Find a way to supply ALL the necessary data ahead of time so each test can focus on just one thing.
        */

        /*
           ALSO, these tests are VERY BAD! We need to test ONLY specific functions. There should be little
           to no preparation required for these tests.
        */

        [TestCase("a", true, 60000)]
        [TestCase("b", true, 60000)]
        [TestCase("x", true, 60000)]
        [TestCase("jump", true, 60000)]
        [TestCase("y65s", true, 60000)] 
        [TestCase("r5200ms", true, 5000)] 
        public void TestOneInput(string input, bool checkDur, int maxDur)
        {
            Parser parser = new Parser();
            string inputRegex = parser.BuildInputRegex(new string[] { input });

            ParsedInputSequence inputSequence = parser.ParseInputs(input, inputRegex, new ParserOptions(0, 200, checkDur, maxDur));

            Assert.AreEqual(inputSequence.ParsedInputResult, inputSequence.TotalDuration <= maxDur ? ParsedInputResults.Valid : ParsedInputResults.Invalid);
            Assert.AreEqual(inputSequence.Inputs.Count, 1);
        }

        [TestCase("a200ms", new int[] { 200 }, new string[] { "a" })]
        [TestCase("a1000ms", new int[] { 1000 }, new string[] { "a" })]
        [TestCase("b1s", new int[] { 1000 }, new string[] { "b" })]
        [TestCase("a500msb748ms", new int[] { 500, 748 }, new string[] { "a", "b" })]
        [TestCase("a3256ms#8sr150s", new int[] { 3256, 8000, 150000 }, new string[] { "a", "r", "#" })]
        [TestCase("_a17ms-x34msy1024ms+b512ms", new int[] { 17, 34, 1024, 512 }, new string[] { "a", "x", "y", "b" })]
        public void TestSubInputDuration(string input, int[] expectedDuration, string[] validInputs)
        {
            Parser parser = new Parser();
            string inputRegex = parser.BuildInputRegex(validInputs);

            ParsedInputSequence inputSequence = parser.ParseInputs(input, inputRegex, new ParserOptions(0, 200, false, 0));

            int expectedDurIndex = 0;
            for (int i = 0; i < inputSequence.Inputs.Count; i++)
            {
                for (int j = 0; j < inputSequence.Inputs[i].Count; j++)
                {
                    ParsedInput inp = inputSequence.Inputs[i][j];
                    
                    Assert.AreEqual(inp.duration, expectedDuration[expectedDurIndex]);
                    expectedDurIndex++;
                }
            }
        }

        [TestCase("a.b", 600, 200, new string[] { "a", ".", "b" })]
        [TestCase("a.b", 900, 300, new string[] { "a", ".", "b" })]
        [TestCase("a200ms#200msb200ms", 600, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1s", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000ms", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a2s", 2000, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a2000ms", 2000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000ms+b1000ms", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000msb1000ms", 2000, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a500msb200ms#1s", 1700, 200, new string[] { "a", "#", "b" })]
        [TestCase("_a500msb200ms#1s-a1300ms", 3000, 200, new string[] { "a", "#", "b" })]
        public void TestTotalDuration(string input, int expectedDur, int defaultDur, string[] validInputs)
        {
            Parser parser = new Parser();
            string inputRegex = parser.BuildInputRegex(validInputs);

            ParsedInputSequence inputSequence = parser.ParseInputs(input, inputRegex, new ParserOptions(0, defaultDur, false));

            Assert.AreEqual(inputSequence.TotalDuration, expectedDur);
        }

        [TestCase("a", 1, new string[] { "a" })]
        [TestCase("a+b", 2, new string[] { "a", "b" })]
        [TestCase("a+b.", 3, new string[] { "a", "b", "." })]
        [TestCase("a.b+a", 4, new string[] { "a", "b", "." })]
        [TestCase("a+bstart+a+b+x+y..b..a", 13, new string[] { "a", "b", "x", "y", "start", "." })]
        public void TestInputCount(string input, int expectedInputCount, string[] validInputs)
        {
            Parser parser = new Parser();
            string inputRegex = parser.BuildInputRegex(validInputs);

            ParsedInputSequence inputSequence = parser.ParseInputs(input, inputRegex, new ParserOptions(0, 200, false));

            int inputCount = 0;
            for (int i = 0; i < inputSequence.Inputs.Count; i++)
            {
                inputCount += inputSequence.Inputs[i].Count;
            }
            
            Assert.AreEqual(inputCount, expectedInputCount);
        }

        [TestCase("jump", "a", new string[] { "jump" }, new string[] { "a" })]
        [TestCase("slide", "_downb", new string[] { "slide" }, new string[] { "_downb" })]
        [TestCase("a.jump.x.pause", "a.a.x.start", new string[] { "jump", "pause" }, new string[] { "a", "start" })]
        public void TestSynonyms(string input, string expectedOutput, string[] synonyms, string[] associatedInputs)
        {
            Parser parser = new Parser();
            List<InputSynonym> inputSynonyms = new List<InputSynonym>(synonyms.Length);
            for (int i = 0; i < synonyms.Length; i++)
            {
                inputSynonyms.Add(new InputSynonym(synonyms[i], associatedInputs[i]));
            }

            string synonymsPopulate = parser.PopulateSynonyms(input, inputSynonyms);

            Assert.AreEqual(synonymsPopulate, expectedOutput);
        }

        [TestCase("#atwo", "aa", new string[] { "#atwo" }, new string[] { "aa" })]
        [TestCase("#mashr", "r34ms#34msr34ms#34msr34ms#34ms", new string[] { "#mashr" }, new string[] { "r34ms#34msr34ms#34msr34ms#34ms" })]
        [TestCase("#mash(a)", "[a34ms#34ms]*20", new string[] { "#mash(*)" }, new string[] { "[<0>34ms#34ms]*20" })]
        [TestCase("#mashalt(a,b)", "[a34ms#34msb34ms#34ms]*20", new string[] { "#mashalt(*,*)" }, new string[] { "[<0>34ms#34ms<1>34ms#34ms]*20" })]
        public void TestMacros(string input, string expectedOutput, string[] macroNames, string[] macroInputs)
        {
            Parser parser = new Parser();
            List<InputMacro> macros = new List<InputMacro>(macroNames.Length);

            for (int i = 0; i < macroNames.Length; i++)
            {
                macros.Add(new InputMacro(macroNames[i], macroInputs[i]));
            }

            string macroInput = parser.PopulateMacros(input, macros.AsQueryable());

            Assert.AreEqual(macroInput, expectedOutput);
        }

        [TestCase("[a]*2", "aa")]
        [TestCase("[a3s#17ms]*2", "a3s#17msa3s#17ms")]
        [TestCase("r[#17msa+b]*2b", "r#17msa+b#17msa+bb")]
        [TestCase("[a#17ms[b]*3x]*2", "a#17msbbbxa#17msbbbx")]
        public void TestExpandify(string input, string expectedOutput)
        {
            Parser parser = new Parser();
            string expandify = parser.Expandify(input);

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