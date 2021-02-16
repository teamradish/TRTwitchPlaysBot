using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using TRBot.Parsing;

namespace TRBot.Tests
{
    [TestFixture]
    public class StandardParserUnitTests
    {
        [TestCase("a", new string[] { "a" })]
        [TestCase("bx", new string[] { "a", "b", "x" })]
        public void TestBasic(string input, params string[] inputList)
        {
            StandardParser standardParser = Basic(inputList);

            ParsedInputSequence seq = standardParser.ParseInputs(input);
            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
        }

        [TestCase("a", new string[] { "b" })]
        [TestCase("r3", new string[] { "r2" })]
        [TestCase("bx", new string[] { "abx" })]
        [TestCase("arrb", new string[] { "a", "b" })]
        [TestCase("raab", new string[] { "a", "b" })]
        [TestCase("aabr", new string[] { "a", "b" })]
        [TestCase("aq", new string[] { "a" })]
        public void TestBasicNormalMsg(string input, string[] inputList)
        {
            StandardParser standardParser = Basic(inputList);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreNotEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
        }

        [TestCase("_q", new string[] { "q" }, new bool[] { true })]
        [TestCase("_x_yxy", new string[] { "x", "y" }, new bool[] { true, true, false, false })]
        [TestCase("_ab_rl", new string[] { "a", "b", "r", "l" }, new bool[] { true, false, true, false} )]
        [TestCase("b25_l3l_l", new string[] { "l3", "b25", "l" }, new bool[] { false, true, false, true } )]
        [TestCase("__", new string[] { "_" }, new bool[] { true } )]
        [TestCase("____", new string[] { "___" }, new bool[] { true } )]
        public void TestHold(string input, string[] inputList, bool[] expectedHolds)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new HoldParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Hold, expectedHolds[i]);
            }
        }

        [TestCase("-q", new string[] { "q" }, new bool[] { true })]
        [TestCase("-ab-rl", new string[] { "a", "b", "r", "l" }, new bool[] { true, false, true, false })]
        [TestCase("rwrew-w", new string[] { "q", "w", "e", "r" }, new bool[] { false, false, false, false, false, true })]
        [TestCase("--", new string[] { "-" }, new bool[] { true })]
        [TestCase("-----", new string[] { "----" }, new bool[] { true })]
        public void TestRelease(string input, string[] inputList, bool[] expectedReleases)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new ReleaseParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Release, expectedReleases[i]);
            }
        }

        //Note: Controller index is zero-based, but the syntax isn't
        //Port 1 in the input means it's at index 0
        //This test presumes the parser is doing this
        [TestCase("a", new string[] { "a" }, new int[] { 0 })]
        [TestCase("&1a", new string[] { "a" }, new int[] { 0 })]
        [TestCase("&1a&88r", new string[] { "a", "r" }, new int[] { 0, 87 })]
        [TestCase("&2b", new string[] { "b" }, new int[] { 1 })]
        [TestCase("&7b", new string[] { "b" }, new int[] { 6 })]
        [TestCase("&777&21q", new string[] { "7", "q" }, new int[] { 76, 20 })]
        [TestCase("&72b", new string[] { "2b" }, new int[] { 6 })]
        [TestCase("&161a", new string[] { "1a" }, new int[] { 15 })]
        [TestCase("&99b", new string[] { "b" }, new int[] { 98 })]
        [TestCase("&72&", new string[] { "&" }, new int[] { 71 })]
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
                Assert.AreEqual(seq.Inputs[i][0].ControllerPort, expectedPorts[i]);
            }
        }

        [TestCase("nmh", new string[] { "n", "m", "h" }, new double[] { 100, 100, 100 })]
        [TestCase("a30%", new string[] { "a" }, new double[] { 30 })]
        [TestCase("l832.1%a80.27%b25.832%", new string[] { "l8", "a", "b" }, new double[] { 32.1d, 80.27d, 25.832d })]
        [TestCase("r74%y37%b1%", new string[] { "b", "r", "y" }, new double[] { 74, 37, 1 })]
        [TestCase("r111%l222%r333%l444%", new string[] { "r1", "l2", "r3", "l4" }, new double[] { 11, 22, 33, 44 })]
        [TestCase("%30%%%1%", new string[] { "%", "%%" }, new double[] { 30, 1 })]
        [TestCase("1%1%%11%", new string[] { "1%", "%1" }, new double[] { 1, 1 })]
        public void TestPercent(string input, string[] inputList, double[] expectedPercents)
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
                Assert.AreEqual(seq.Inputs[i][0].Percent, expectedPercents[i]);
            }
        }

        [TestCase("a", new string[] { "a" }, new int[] { 1 } )]
        [TestCase("b+r", new string[] { "b", "r" }, new int[] { 2 } )]
        [TestCase("aa+b+cr+b", new string[] { "a", "b", "c", "r" }, new int[] { 1, 3, 2 } )]
        [TestCase("11+r2+b3+n4+q5+76", new string[] { "11", "r2", "b3", "n4", "q5", "76" }, new int[] { 6 } )]
        [TestCase("wekl+l2b5+n+k+web5l2", new string[] { "n", "k", "l", "l2", "b5", "we" }, new int[] { 1, 1, 2, 4, 1, 1 } )]
        [TestCase("+++++++++++++++", new string[] { "+" }, new int[] { 8 } )]
        public void TestSimultaneous(string input, string[] inputList, int[] expectedSubsets)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new SimultaneousParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
            
            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i].Count, expectedSubsets[i]);
            }
        }

        [TestCase("a", new string[] { "a" }, new int[] { 200 })]
        [TestCase("a1s", new string[] { "a" }, new int[] { 1000 })]
        [TestCase("a150sb7s", new string[] { "a", "b" }, new int[] { 150000, 7000 })]
        [TestCase("c0sq11sc2s", new string[] { "c", "q" }, new int[] { 0, 11000, 2000 })]
        [TestCase("111s8888s888888s", new string[] { "11", "888" }, new int[] { 1000, 8000, 888000 })]
        [TestCase("b443s", new string[] { "b" }, new int[] { 443000 })]
        public void TestSecondDuration(string input, string[] inputList, int[] expectedDurations)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new SecondParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.CheckMaxDur = false;
            standardParser.DefaultInputDuration = 200;

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
            
            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Duration, expectedDurations[i]);
            }
        }

        [TestCase("a", new string[] { "a" }, new int[] { 200 })]
        [TestCase("a1500ms", new string[] { "a" }, new int[] { 1500 })]
        [TestCase("a1msb355557ms", new string[] { "a", "b" }, new int[] { 1, 355557 })]
        [TestCase("8134msms99761ms", new string[] { "81", "ms" }, new int[] { 34, 99761 })]
        [TestCase("222222ms555555555ms", new string[] { "22", "5555" }, new int[] { 2222, 55555 })]
        [TestCase("l343msl333msr3100msr31257ms", new string[] { "r3", "l3" }, new int[] { 43, 33, 100, 1257 })]
        public void TestMillisecondDuration(string input, string[] inputList, int[] expectedDurations)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new MillisecondParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.CheckMaxDur = false;
            standardParser.DefaultInputDuration = 200;

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
            
            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Duration, expectedDurations[i]);
            }
        }

        [TestCase("__a", new string[] { "a" })]
        [TestCase("b__r", new string[] { "r" })]
        [TestCase("__n_q", new string[] { "n", "q" })]
        [TestCase("33_", new string[] { "33" })]
        [TestCase("___g", new string[] { "g" })]
        public void TestHoldInvalid(string input, string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new HoldParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.Inputs, null);
        }

        [TestCase("--a", new string[] { "a" })]
        [TestCase("b--r", new string[] { "r" })]
        [TestCase("--n-q", new string[] { "n", "q" })]
        [TestCase("33-", new string[] { "33" })]
        [TestCase("---g", new string[] { "g" })]
        public void TestReleaseInvalid(string input, string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new ReleaseParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.Inputs, null);
        }

        [TestCase("&a", new string[] { "a" }, 0)]
        [TestCase("a&a", new string[] { "a" }, 0)]
        [TestCase("&99b", new string[] { "b" }, 5)]
        public void TestPortInvalid(string input, string[] inputList, int maxPort)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new PortParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.MaxPortNum = maxPort;

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreNotEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
        }

        [TestCase("a%", new string[] { "a" })]
        [TestCase("b5%r%", new string[] { "b", "r" })]
        [TestCase("y101%", new string[] { "y" })]
        [TestCase("y100.001%", new string[] { "y" })]
        [TestCase("h5.7821%", new string[] { "h" })]
        [TestCase("%1%%5%%1", new string[] { "%" })]
        public void TestPercentInvalid(string input, string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new PercentParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreNotEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
        }

        [TestCase("j+", new string[] { "j" })]
        [TestCase("w+wj+w+j+", new string[] { "j", "w" })]
        [TestCase("r+v+d+d+v+r+v+ddrvvdr+v+", new string[] { "r", "v", "d" })]
        public void TestSimultaneousInvalid(string input, string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new SimultaneousParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Invalid);
        }

        [TestCase("as", new string[] { "a" })]
        [TestCase("bss", new string[] { "bs" })]
        [TestCase("r7se2s", new string[] { "r", "e2" })]
        public void TestSecondDurationInvalid(string input, string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new SecondParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreNotEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
        }

        [TestCase("ams", new string[] { "a" })]
        [TestCase("mss1ms1", new string[] { "ms", "s" })]
        [TestCase(";35msqq;ms", new string[] { "q", ";" })]
        public void TestMillisecondDurationInvalid(string input, string[] inputList)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new MillisecondParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreNotEqual(seq.ParsedInputResult, ParsedInputResults.Valid);
        }

        [TestCase("a1sb2570msr15s", new string[] { "a", "b", "r" }, new int[] { 1000, 2570, 15000 })]
        [TestCase("s37msmsss37s", new string[] { "ms", "s" }, new int[] { 37, 200, 200, 37000 })]
        [TestCase("aab35msab2sabb3s", new string[] { "ab", "a", "b" }, new int[] { 200, 35, 2000, 200, 3000 })]
        public void TestAllDurationTypes(string input, string[] inputList, int[] expectedDurations)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new MillisecondParserComponent(),
                new SecondParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.CheckMaxDur = false;
            standardParser.DefaultInputDuration = 200;

            ParsedInputSequence seq = standardParser.ParseInputs(input);
            
            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Duration, expectedDurations[i]);
            }
        }

        [TestCase("a50%500ms", new string[] { "a" }, new double[] { 50 }, new int[] { 500 })]
        [TestCase("b50%1sl32%5748ms", new string[] { "b", "l" }, new double[] { 50, 32 }, new int[] { 1000, 5748 })]
        [TestCase("b50.75%1sl32.036%5748ms", new string[] { "b", "l" }, new double[] { 50.75, 32.036 }, new int[] { 1000, 5748 })]
        [TestCase("q34msba1sr30%l23%", new string[] { "q", "b", "a", "r", "l" }, new double[] { 100, 100, 100, 30, 23 }, new int[] { 34, 200, 1000, 200, 200 })]
        public void TestPercentDurations(string input, string[] inputList, double[] expectedPercents, int[] expectedDurations)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new InputParserComponent(inputList),
                new PercentParserComponent(),
                new MillisecondParserComponent(),
                new SecondParserComponent(),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.CheckMaxDur = false;
            standardParser.DefaultInputDuration = 200;

            ParsedInputSequence seq = standardParser.ParseInputs(input);
            
            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Percent, expectedPercents[i]);
                Assert.AreEqual(seq.Inputs[i][0].Duration, expectedDurations[i]);
            }
        }

        [TestCase("a", new string[] { "a" }, new bool[] { false }, new bool[] { false })]
        [TestCase("_l-ab-l_l_b-r-a", new string[] { "a", "b", "l", "r" }, new bool[] { true, false, false, false, true, true, false, false },
            new bool[] { false, true, false, true, false, false, true, true })]
        [TestCase("_a_b_a_b", new string[] { "a", "b" }, new bool[] { true, true, true, true }, new bool[] { false, false, false, false })]
        [TestCase("-a-b-a-b", new string[] { "a", "b" }, new bool[] { false, false, false, false }, new bool[] { true, true, true, true })]
        public void TestHoldRelease(string input, string[] inputList, bool[] expectedHolds, bool[] expectedReleases)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new HoldParserComponent(),
                new ReleaseParserComponent(),
                new InputParserComponent(inputList),
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.CheckMaxDur = false;
            standardParser.DefaultInputDuration = 200;

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                Assert.AreEqual(seq.Inputs[i][0].Hold, expectedHolds[i]);
                Assert.AreEqual(seq.Inputs[i][0].Release, expectedReleases[i]);
            }
        }

        //Test everything!
        [TestCase("a", new [] { "a" },
            new [] { 0 }, new [] { false }, new [] { false }, new double[] { 100 }, new [] { 200 }, new [] { 1 })]
        [TestCase("&2_a8%34ms+&6-y99.9%13s&10-b30%3472ms", new [] { "a", "b", "y" },
            new [] { 1, 5, 9 }, new [] { true, false, false }, new [] { false, true, true },
            new double[] { 8, 99.9, 30 }, new [] { 34, 13000, 3472 }, new [] { 2, 1 })]
        public void TestFullSyntax(string input, string[] inputList,
            int[] ports, bool[] holds, bool[] releases, double[] percents, int[] durations, int[] subsetCounts)
        {
            List<IParserComponent> components = new List<IParserComponent>()
            {
                new PortParserComponent(),
                new HoldParserComponent(),
                new ReleaseParserComponent(),
                new InputParserComponent(inputList),
                new PercentParserComponent(),
                new MillisecondParserComponent(),
                new SecondParserComponent(),
                new SimultaneousParserComponent()
            };

            StandardParser standardParser = new StandardParser(components);
            standardParser.CheckMaxDur = false;
            standardParser.DefaultInputDuration = 200;
            standardParser.MaxPortNum = 100;

            ParsedInputSequence seq = standardParser.ParseInputs(input);

            Assert.AreEqual(seq.ParsedInputResult, ParsedInputResults.Valid);

            int index = 0;

            for (int i = 0; i < seq.Inputs.Count; i++)
            {
                List<ParsedInput> parsedInpList = seq.Inputs[i];

                //Validate how many sets exist
                Assert.AreEqual(parsedInpList.Count, subsetCounts[i]);

                for (int j = 0; j < parsedInpList.Count; j++)
                {
                    ParsedInput parsedInp = seq.Inputs[i][j];

                    Assert.AreEqual(parsedInp.ControllerPort, ports[index]);
                    Assert.AreEqual(parsedInp.Hold, holds[index]);
                    Assert.AreEqual(parsedInp.Release, releases[index]);
                    Assert.AreEqual(parsedInp.Percent, percents[index]);
                    Assert.AreEqual(parsedInp.Duration, durations[index]);

                    index++;
                }
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