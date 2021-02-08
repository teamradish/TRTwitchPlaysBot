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
        /*
           ALSO, these tests are VERY BAD! We need to test ONLY specific functions. There should be little
           to no preparation required for these tests.
        */

        /*[TestCase("a", true, 60000)]
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
        }*/

        [TestCase("a", new string[] { "a" })]
        [TestCase("bx", new string[] { "a", "b", "x" })]
        public void TestBasic(string input, params string[] inputList)
        {
            StandardParser standardParser = Basic(inputList);

            ParsedInputSequence seq = standardParser.ParseInputs(input);
            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
            Assert.AreEqual(seq.Error, string.Empty);
        }

        [TestCase("a", new string[] { "b" })]
        [TestCase("r3", new string[] { "r2" })]
        [TestCase("bx", new string[] { "abx" })]
        public void TestBasicFail(string input, string[] inputList)
        {
            StandardParser standardParser = Basic(inputList);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreNotEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
            Assert.AreNotEqual(seq.Error, string.Empty);
        }

        [TestCase("_q", new string[] { "q" }, new int[] { 0 })]
        [TestCase("_x_yxy", new string[] { "x", "y" }, new int[] { 0, 1 })]
        [TestCase("_ab_rl", new string[] { "a", "b", "r", "l" }, new int[] { 0, 2} )]
        [TestCase("b25_l3l_l", new string[] { "l3", "b25", "l" }, new int[] { 1, 3} )]
        public void TestHold(string input, string[] inputList, int[] expectedHoldIndices)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new HoldParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < expectedHoldIndices.Length; i++)
            {
                int holdIndex = expectedHoldIndices[i];
                Assert.AreEqual(seq.Inputs[holdIndex][0].hold, true);
            }
        }

        [TestCase("-q", new string[] { "q" }, new int[] { 0 })]
        [TestCase("-ab-rl", new string[] { "a", "b", "r", "l" }, new int[] { 0, 2 })]
        [TestCase("rwrew-w", new string[] { "q", "w", "e", "r" }, new int[] { 5 })]
        public void TestRelease(string input, string[] inputList, int[] expectedReleaseIndices)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new ReleaseParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < expectedReleaseIndices.Length; i++)
            {
                int releaseIndex = expectedReleaseIndices[i];
                Assert.AreEqual(seq.Inputs[releaseIndex][0].release, true);
            }
        }

        [TestCase("a", new string[] { "a" }, new int[] { 0 })]
        [TestCase("&1a", new string[] { "a" }, new int[] { 1 })]
        [TestCase("&0a&88r", new string[] { "a", "r" }, new int[] { 0, 88 })]
        [TestCase("&2b", new string[] { "b" }, new int[] { 2 })]
        [TestCase("&7b", new string[] { "b" }, new int[] { 7 })]
        [TestCase("&777&21q", new string[] { "7", "q" }, new int[] { 77, 21 })]
        [TestCase("&72b", new string[] { "2b" }, new int[] { 7 })]
        [TestCase("&161a", new string[] { "1a" }, new int[] { 16 })]
        [TestCase("&99b", new string[] { "b" }, new int[] { 99 })]
        public void TestPort(string input, string[] inputList, int[] expectedPorts)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new PortParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.MaxPortNum = expectedPorts.Max();

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].controllerPort, expectedPorts[i]);
            }
        }

        [TestCase("nmh", new string[] { "n", "m", "h" }, new int[] { 100, 100, 100 })]
        [TestCase("a30%", new string[] { "a" }, new int[] { 30 })]
        [TestCase("r74%y37%b1%", new string[] { "b", "r", "y" }, new int[] { 74, 37, 1 })]
        [TestCase("r111%l222%r333%l444%", new string[] { "r1", "l2", "r3", "l4" }, new int[] { 11, 22, 33, 44 })]
        public void TestPercent(string input, string[] inputList, int[] expectedPercents)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new PercentParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
            
            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].percent, expectedPercents[i]);
            }
        }

        # region Utility

        private StandardParser Basic(string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList)
            };

            StandardParser standardParser = new StandardParser(components, 0, 4, 200, 60000, false);
            return standardParser;
        }

        #endregion
    }
}