/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Routines;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace TRBot.Commands
{
    /// <summary>
    /// Execute arbitrary C# code.
    /// <para>
    /// It's highly recommended to have this accessible ONLY to the streamer.
    /// This has potential to corrupt, delete, or modify data and is provided as a convenience
    /// for carrying out whatever cannot be done normally through TRBot while the application is running.
    /// You are responsible for entrusting use of this command to others.
    /// </para>
    /// </summary>
    public class ExecCommand : BaseCommand
    {
        //Add references and assemblies
        private readonly Assembly[] References = new Assembly[]
        {
            typeof(Console).Assembly,
            typeof(List<int>).Assembly,
            typeof(ExecCommand).Assembly,
            typeof(Parsing.Parser).Assembly,
            typeof(GameConsole).Assembly,
            typeof(VirtualControllers.IVirtualController).Assembly,
            typeof(IClientService).Assembly,
            typeof(TRBot.Misc.BotMessageHandler).Assembly,
            typeof(TRBot.Data.CommandData).Assembly,
            typeof(TRBot.Utilities.EnumUtility).Assembly
        };

        private readonly string[] Imports = new string[]
        {
            nameof(System),
            $"{nameof(System)}.{nameof(System.Collections)}.{nameof(System.Collections.Generic)}",
            Assembly.GetExecutingAssembly().GetName().Name,
            $"{nameof(TRBot)}.{nameof(TRBot.Parsing)}",
            $"{nameof(TRBot)}.{nameof(TRBot.Consoles)}",
            $"{nameof(TRBot)}.{nameof(TRBot.VirtualControllers)}",
            $"{nameof(TRBot)}.{nameof(TRBot.Connection)}",
            $"{nameof(TRBot)}.{nameof(TRBot.Misc)}",
            $"{nameof(TRBot)}.{nameof(TRBot.Data)}",
            $"{nameof(TRBot)}.{nameof(TRBot.Utilities)}"
        };

        private ScriptOptions ScriptCompileOptions = null;

        private string UsageMessage = "Usage: \"C# code\"";

        public ExecCommand()
        {

        }

        public override void Initialize()
        {
            ScriptCompileOptions = ScriptOptions.Default.WithReferences(References).WithImports(Imports);
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            string code = args.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(code) == true)
            {
                QueueMessage(UsageMessage);
                return;
            }

            ExecuteCSharpScript(code, args);
        }

        protected async void ExecuteCSharpScript(string code, EvtChatCommandArgs args)
        {
            //Store the default console output stream
            TextWriter defaultOut = Console.Out;

            bool prevIgnoreConsoleLogVal = DataContainer.MessageHandler.LogToLogger;

            try
            {
                ExecScriptDataContainer execContainer = new ExecScriptDataContainer(this, CmdHandler, DataContainer, RoutineHandler, args);

                var script = await CSharpScript.RunAsync(code, ScriptCompileOptions, execContainer, typeof(ExecScriptDataContainer));
            }
            catch (CompilationErrorException exception)
            {
                QueueMessage($"Compiler error: {exception.Message}", Serilog.Events.LogEventLevel.Warning);
            }
        }

        /// <summary>
        /// A container class holding global data accessible to arbitrary code.
        /// </summary>
        public class ExecScriptDataContainer
        {
            public BaseCommand ThisCmd = null;
            public CommandHandler CmdHandler = null;
            public DataContainer DataContainer = null;
            public BotRoutineHandler RoutineHandler = null;
            public EvtChatCommandArgs Args = null;

            public ExecScriptDataContainer(BaseCommand thisCmd, CommandHandler cmdHandler,
                DataContainer dataContainer, BotRoutineHandler routineHandler, EvtChatCommandArgs args)
            {
                ThisCmd = thisCmd;
                CmdHandler = cmdHandler;
                DataContainer = dataContainer;
                RoutineHandler = routineHandler;
                Args = args;
            }
        }
    }
}
