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

#if true

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
    public class MacroPreparserBenchmarks
    {
        private string Message = string.Empty;
        private IQueryable<InputMacro> Macros = null;

        public MacroPreparserBenchmarks()
        {
            Message = "#bow(x)#item(left,2,z) #pressb #mash(a,#mash) #bow(#pressb) #item(down,7,r)";

            Macros = new List<InputMacro>()
            {
                new InputMacro("#pressb", "b1s"),
                new InputMacro("#mash(*,*)", "[<0>34ms #34ms]*<1>"),
                new InputMacro("#bow(*)", "#item(right,2,<0>)"),
                new InputMacro("#item(*,*,*)", "dup #500ms [<0>100ms #100ms]*<1> <2> #300ms b"),
                new InputMacro("#mash", "left34msright34ms")
            }.AsQueryable();            
        }

        [Benchmark]
        public void TestOldMacroPreparser()
        {
            InputMacroPreparser macroPreparser = new InputMacroPreparser(Macros);
            
            macroPreparser.Preparse(Message);
        }

        [Benchmark]
        public void TestNewMacroPreparser()
        {
            InputMacroPreparserNew newMacroPreparser = new InputMacroPreparserNew(Macros);
            
            newMacroPreparser.Preparse(Message);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<MacroPreparserBenchmarks>();
        }
    }
}

#endif