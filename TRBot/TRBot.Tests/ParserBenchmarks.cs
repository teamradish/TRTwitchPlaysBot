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

#if false

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TRBot.Parsing;

namespace TRBot.Tests
{
    [MemoryDiagnoser]
    public class ParserBenchmarks
    {
        private string Message = string.Empty;
        private IQueryable<InputMacro> Macros = null;
        private IEnumerable<InputSynonym> Synonyms = null;
        private string[] ValidInputs = null;

        private string ParserCachedMessage = string.Empty;
        private List<IParserComponent> ParserComponents = null;

        public ParserBenchmarks()
        {
            Message = "[_r34ms #mash(a,150) [&1a #750ms &2b up80%400ms]*700] #pressb -r34ms .72ms]*355";

            Macros = new List<InputMacro>()
            {
                new InputMacro("#pressb", "b1s"),
                new InputMacro("#mash(*,*)", "[<0>34ms #34ms]*<1>"),
            }.AsQueryable();

            Synonyms = new List<InputSynonym>()
            {
                new InputSynonym(".", "#")
            };

            ValidInputs = new string[] { "a", "b", "r", "up", "#" };

            List<IPreparser> Preparsers = new List<IPreparser>()
            {
                new InputMacroPreparser(Macros),
                new InputSynonymPreparser(Synonyms),
                new ExpandPreparser(),
                new RemoveWhitespacePreparser()
            };

            ParserCachedMessage = Message;

            for (int i = 0; i < Preparsers.Count; i++)
            {
                ParserCachedMessage = Preparsers[i].Preparse(ParserCachedMessage);
            }

            ParserComponents = new List<IParserComponent>()
            {
                new PortParserComponent(),
                new HoldParserComponent(),
                new ReleaseParserComponent(),
                new InputParserComponent(ValidInputs),
                new PercentParserComponent(),
                new MillisecondParserComponent(),
                new SecondParserComponent(),
                new SimultaneousParserComponent()
            };
        }

        [Benchmark]
        public void TestParser()
        {
            List<IPreparser> preparsers = new List<IPreparser>()
            {
                new InputMacroPreparser(Macros),
                new InputSynonymPreparser(Synonyms),
                new ExpandPreparser(),
                new RemoveWhitespacePreparser()
            };

            List<IParserComponent> components = new List<IParserComponent>()
            {
                new PortParserComponent(),
                new HoldParserComponent(),
                new ReleaseParserComponent(),
                new InputParserComponent(ValidInputs),
                new PercentParserComponent(),
                new MillisecondParserComponent(),
                new SecondParserComponent(),
                new SimultaneousParserComponent()
            };

            StandardParser standardParser = new StandardParser(preparsers, components, 0, 99, 200, 60000, false);
            standardParser.ParseInputs(Message);
        }

        [Benchmark]
        public void TestNewParserCached()
        {
            StandardParser standardParser = new StandardParser(ParserComponents, 0, 99, 200, 60000, false);
            standardParser.ParseInputs(ParserCachedMessage);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ParserBenchmarks>();
        }
    }
}

#endif