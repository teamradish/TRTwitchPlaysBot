using NUnit.Framework;
using TRBot;

namespace TRBot.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [TestCase("a", true, 60000)]
        [TestCase("b", true, 60000)]
        [TestCase("x", true, 60000)]
        [TestCase("jump", true, 60000)]
        [TestCase("y65s", true, 60000)] 
        public void TestOneInput(string input, bool checkDur, int maxDur)
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
        [TestCase("a1000ms+b1000ms", 1000, 200, new string[] { "a", "#", "b" })]
        [TestCase("a1000ms b1000ms", 2000, 200, new string[] { "a", "#", "b" })]
        public void TestDuration(string input, int expectedDur, int defaultDur, string[] validInputs)
        {
            string inputRegex = Parser.BuildInputRegex(validInputs);

            Parser.InputSequence inputSequence = Parser.ParseInputs(input, inputRegex, new Parser.ParserOptions(0, defaultDur, false));

            Assert.AreEqual(inputSequence.TotalDuration, expectedDur);
        }
    }
}