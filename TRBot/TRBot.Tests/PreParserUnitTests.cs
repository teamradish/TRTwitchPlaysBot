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

        #region New Macro Preparser

        [TestCase("#pressa", new string[] { "#press", "#pressa" }, new string[] { "b", "a" }, "a")]
        [TestCase("#pressa", new string[] { "#press", "#pressa" }, new string[] { "b", "superathing" }, "superathing")]
        [TestCase("#mymacro #mymacro2", new string[] { "#mymacro", "#mymacro2" }, new string[] { "_left b up500ms", "_right down left1.5s" }, "_left b up500ms _right down left1.5s")]
        [TestCase("#nestedmacro", new string[] { "#nestedmacro", "#macro", "#nested" }, new string[] { "#nested #macro", "up350ms", "right a" }, "right a up350ms")]
        [TestCase("##b", new string[] { "#b" }, new string[] { "b300ms" }, "#b300ms")]
        [TestCase("#hello#helabr", new string[] { "#hello", "#he" }, new string[] { "right750msup", "left" }, "right750msupleftlabr")]
        [TestCase("#hello#helabr", new string[] { "#hello", "#he", "#hela", "#helab" }, new string[] { "right750msup", "left", "right", "up" }, "right750msupupr")]
        [TestCase("#friend#free", new string[] { "#friend", "#free" }, new string[] { "#free", "#friend" }, "#friend#free" )]
        [TestCase("#friendf#friend#free", new string[] { "#friendf", "#friend", "#free", "#test" }, new string[] { "#test", "#free", "#friend", "up450ms" }, "up450ms#friend#free" )]
        public void TestNormalMacrosNew(string input, string[] macroNames, string[] macroValues, string expectedOutput)
        {
            Assert.AreEqual(macroNames.Length, macroValues.Length);

            IQueryable<InputMacro> macros = BuildMacroList(macroNames, macroValues);

            InputMacroPreparserNew imp = new InputMacroPreparserNew(macros.AsQueryable());
            string output = imp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        [TestCase("#press(a)", new string[] { "#press(*)" }, new string[] { "<0>" }, "a")]
        [TestCase("#press(abcdefg)", new string[] { "#press(*)" }, new string[] { "<0>" }, "abcdefg")]
        [TestCase("#press(35,b)", new string[] { "#press(*,*)" }, new string[] { "<0>ms _<1>" }, "35ms _b")]
        [TestCase("#mash(a,20)", new string[] { "#mash(*,*)" }, new string[] { "[<0>34ms #34ms]*<1>" }, "[a34ms #34ms]*20")]
        [TestCase(
            "#pspin(5,10,47)",
            new string[] { "#pspin(*,*,*)" },
            new string[]
            { "[_up<2>%17ms#<0>ms_right<2>%17ms#<0>ms-up17ms#<0>ms_down<2>%17ms#<0>ms-right17ms#<0>ms_left<2>%17ms#<0>ms-down17ms#<0>ms_up<2>%17ms#<0>ms-left17ms]*<1>"
            },
            "[_up47%17ms#5ms_right47%17ms#5ms-up17ms#5ms_down47%17ms#5ms-right17ms#5ms_left47%17ms#5ms-down17ms#5ms_up47%17ms#5ms-left17ms]*10"
        )]
        [TestCase("#dy(#test(q,b))", new string[] { "#dy(*)", "#test(*,*)" }, new string[] { "<0>500ms", "[<0>34ms<1>250ms]*2 r" }, "[q34msb250ms]*2 r500ms")]
        [TestCase(
            "#dy(#dy,#t(n,r,#dy(b, n)))",
            new string[] { "#dy", "#dy(*,*)", "#t(*,*,*)" },
            new string[] { "l320%", "<0>4ms<1>", "<0>1s <1>2s #dy(<1>, <2>)" },
            "l320%4msn1s r2s r4ms b4ms n"
        )]
        [TestCase(
            "#bow(x)",
            new string[] { "#item(*,*,*)", "#bow(*)" },
            new string[] { "dup #500ms [<0>100ms #100ms]*<1> <2> #300ms b", "#item(right,2,<0>)" },
            "dup #500ms [right100ms #100ms]*2 x #300ms b"
        )]
        [TestCase(
            "#bow(x) #item(left,2,z)",
            new string[] { "#item(*,*,*)", "#bow(*)" },
            new string[] { "dup #500ms [<0>100ms #100ms]*<1> <2> #300ms b", "#item(right,2,<0>)" },
            "dup #500ms [right100ms #100ms]*2 x #300ms b dup #500ms [left100ms #100ms]*2 z #300ms b"
        )]
        public void TestDynamicMacrosNew(string input, string[] macroNames, string[] macroValues, string expectedOutput)
        {
            Assert.AreEqual(macroNames.Length, macroValues.Length);

            IQueryable<InputMacro> macros = BuildMacroList(macroNames, macroValues);

            InputMacroPreparserNew imp = new InputMacroPreparserNew(macros.AsQueryable());
            string output = imp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        [TestCase("#press(a)#press", new string[] { "#press(*)", "#press" }, new string[] { "<0>", "b" }, "ab")]
        [TestCase("#press(a)#press(b)#press#press(ba)", new string[] { "#press(*)", "#press" }, new string[] { "<0>", "c" }, "abcba")]
        [TestCase("#press(#press)", new string[] { "#press(*)", "#press" }, new string[] { "<0>", "b" }, "b")]
        [TestCase("#mash(#ma,#mb)", new string[] { "#m", "#mash(*,*)", "#ma", "#mb" }, new string[] { "r", "[<0>34ms#34ms<1>34ms]*10", "a10%", "b20%" }, "[a10%34ms#34msb20%34ms]*10")]
        [TestCase("#mash(#ma,#mb)", new string[] { "#m", "#mash(*)", "#mash(*,*)", "#mash(*,*,*)", "#ma", "#mb" }, new string[] { "r", "<0>100ms", "[<0>34ms#34ms<1>34ms]*10", "<0>20ms<1>50ms<2>40ms", "a10%", "b20%" }, "[a10%34ms#34msb20%34ms]*10")]
        [TestCase("#press(#mash(b))", new string[] { "#press(*)", "#mash(*)" }, new string[] { "<0>", "[<0>#400ms]*15" }, "[b#400ms]*15")]
        public void TestNormalAndDynamicMacrosNew(string input, string[] macroNames, string[] macroValues, string expectedOutput)
        {
            Assert.AreEqual(macroNames.Length, macroValues.Length);

            IQueryable<InputMacro> macros = BuildMacroList(macroNames, macroValues);

            InputMacroPreparserNew imp = new InputMacroPreparserNew(macros.AsQueryable());
            string output = imp.Preparse(input);

            Assert.AreEqual(output, expectedOutput);
        }

        #endregion

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

        [TestCase("A", "a")]
        [TestCase("_b+R700ms", "_b+r700ms")]
        [TestCase("AaBbAbrARAmW", "aabbabraramw")]
        public void TestLower(string input, string expectedOutput)
        {
            LowercasePreparser lp = new LowercasePreparser();

            string output = lp.Preparse(input);

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