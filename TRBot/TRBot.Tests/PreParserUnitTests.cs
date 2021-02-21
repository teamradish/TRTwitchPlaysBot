/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using TRBot.Parsing;

namespace TRBot.Tests
{
    [TestFixture]
    public class PreParserUnitTests
    {
        [TestCase("abcdefg", "abcdefg")]
        [TestCase("a b c d e f g", "abcdefg")]
        [TestCase("ar bc df eh", "arbcdfeh")]
        public void TestWhitespace(string input, string expectedOutput)
        {
            RemoveWhitespacePreparser rwp = new RemoveWhitespacePreparser();
            string output = rwp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        [TestCase("#pressa", new string[] { "#pressa" }, new string[] { "a" }, "a")]
        [TestCase("#pressa r b", new string[] { "#pressa" }, new string[] { "a" }, "a r b")]
        [TestCase("#jumpup . .b750ms", new string[] { "#jumpup" }, new string[] { "_upa500ms" }, "_upa500ms . .b750ms")]
        public void TestNormalMacros(string input, string[] macroNames, string[] macroValues, string expectedOutput)
        {
            Assert.AreEqual(macroNames.Length, macroValues.Length);

            IQueryable<InputMacro> macros = BuildMacroList(macroNames, macroValues);

            InputMacroPreparser imp = new InputMacroPreparser(macros.AsQueryable());
            string output = imp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        [TestCase("#press(a)", new string[] { "#press(*)" }, new string[] { "<0>" }, "a")]
        [TestCase("#press(abcdefg)", new string[] { "#press(*)" }, new string[] { "<0>" }, "abcdefg")]
        [TestCase("#press(35,b)", new string[] { "#press(*,*)" }, new string[] { "<0>ms _<1>" }, "35ms _b")]
        [TestCase("#mash(a,20)", new string[] { "#mash(*,*)" }, new string[] { "[<0>34ms #34ms]*<1>" }, "[a34ms #34ms]*20")]
        public void TestDynamicMacros(string input, string[] macroNames, string[] macroValues, string expectedOutput)
        {
            Assert.AreEqual(macroNames.Length, macroValues.Length);

            IQueryable<InputMacro> macros = BuildMacroList(macroNames, macroValues);

            InputMacroPreparser imp = new InputMacroPreparser(macros.AsQueryable());
            string output = imp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        [TestCase("abr", new string[] { "b" }, new string[] { "q" }, "aqr")]
        [TestCase("abw", new string[] { "ab" }, new string[] { "ww" }, "www")]
        [TestCase("triangle", new string[] { "a" }, new string[] { "triangle" }, "tritrianglengle")]
        public void TestSynonyms(string input, string[] synonymNames, string[] synonymValues, string expectedOutput)
        {
            Assert.AreEqual(synonymNames.Length, synonymValues.Length);

            IEnumerable<InputSynonym> synonyms = BuildSynonymList(synonymNames, synonymValues);

            InputSynonymPreparser isp = new InputSynonymPreparser(synonyms);
            string output = isp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        [TestCase("[a]*10", "aaaaaaaaaa")]
        [TestCase("[r [#34ms b34ms]*2 #500ms l]*2", "r #34ms b34ms#34ms b34ms #500ms lr #34ms b34ms#34ms b34ms #500ms l")]
        [TestCase("[r #500ms b37ms #1s a80000ms -l+_right]*0", "")]
        [TestCase("[ #mash(b) _left -right #100ms ]*3", " #mash(b) _left -right #100ms  #mash(b) _left -right #100ms  #mash(b) _left -right #100ms ")]
        public void TestExpand(string input, string expectedOutput)
        {
            ExpandPreparser ep = new ExpandPreparser();
            
            string output = ep.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        private IQueryable<InputMacro> BuildMacroList(string[] macroNames, string[] macroValues)
        {
            List<InputMacro> inputMacros = new List<InputMacro>(macroNames.Length);

            for (int i = 0; i < macroNames.Length; i++)
            {
                inputMacros.Add(new InputMacro(macroNames[i], macroValues[i]));
            }

            return inputMacros.AsQueryable();
        }

        private IEnumerable<InputSynonym> BuildSynonymList(string[] synonymNames, string[] synonymValues)
        {
            List<InputSynonym> inputSynonyms = new List<InputSynonym>(synonymNames.Length);

            for (int i = 0; i < synonymNames.Length; i++)
            {
                inputSynonyms.Add(new InputSynonym(synonymNames[i], synonymValues[i]));
            }

            return inputSynonyms.AsEnumerable();
        }
    }
}