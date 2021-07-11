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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.Data;
using TRBot.Logging;
using TRBot.Utilities;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace TRBot.Routines
{
    /// <summary>
    /// Execute arbitrary C# code from a given file. This allows for custom routines.
    /// These custom source files are subject to the same license terms as TRBot.
    /// <para>
    /// It's highly recommended to have this accessible ONLY to the streamer.
    /// This has potential to corrupt, delete, or modify data and is provided as a convenience
    /// for carrying out whatever cannot be done normally through TRBot while the application is running.
    /// You are responsible for your use of this routine and how it can be used to affect your machine.
    /// </para>
    /// </summary>
    public class ExecFromFileRoutine : BaseRoutine
    {
        //Add references and assemblies
        private readonly Assembly[] References = new Assembly[]
        {
            typeof(Console).Assembly,
            typeof(List<int>).Assembly,
            typeof(IClientService).Assembly,
            typeof(GameConsole).Assembly,
            typeof(TRBot.Data.CommandData).Assembly,
            typeof(TRBot.Logging.TRBotLogger).Assembly,
            typeof(TRBot.Misc.BotMessageHandler).Assembly,
            typeof(Parsing.IParser).Assembly,
            typeof(TRBot.Permissions.PermissionAbility).Assembly,
            typeof(TRBot.Routines.BaseRoutine).Assembly,
            typeof(TRBot.Utilities.EnumUtility).Assembly,
            typeof(VirtualControllers.IVirtualController).Assembly,
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

        public ExecFromFileRoutine()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            ScriptCompileOptions = ScriptOptions.Default.WithReferences(References).WithImports(Imports);
        }

        public override void CleanUp()
        {
            base.CleanUp();
        }

        public override void UpdateRoutine(in DateTime currentTimeUTC)
        {
            //First try to read the file as an absolute path
            string codeText = FileHelpers.ReadFromTextFile(ValueStr);

            //If that wasn't found, try a relative path
            if (string.IsNullOrEmpty(codeText) == true)
            {
                codeText = FileHelpers.ReadFromTextFile(DataConstants.DataFolderPath, ValueStr);
            }

            if (string.IsNullOrEmpty(codeText) == true)
            {
                DataContainer.MessageHandler.QueueMessage("Invalid source file for custom routine. Double check its location.", Serilog.Events.LogEventLevel.Warning);
                return;
            }

            //Execute the code
            ExecuteCSharpScript(codeText, currentTimeUTC);
        }

        protected async void ExecuteCSharpScript(string code, DateTime currentTimeUTC)
        {
            //Store the default console output stream
            TextWriter defaultOut = Console.Out;

            bool prevIgnoreConsoleLogVal = DataContainer.MessageHandler.LogToLogger;

            try
            {
                ExecScriptDataContainer execContainer = new ExecScriptDataContainer(this, RoutineHandler, DataContainer, currentTimeUTC);

                var script = await CSharpScript.RunAsync(code, ScriptCompileOptions, execContainer, typeof(ExecScriptDataContainer));
            }
            catch (CompilationErrorException exception)
            {
                DataContainer.MessageHandler.QueueMessage($"Compiler error on custom routine. {exception.Message}", Serilog.Events.LogEventLevel.Warning);
            }
            catch (Exception otherExc)
            {
                DataContainer.MessageHandler.QueueMessage($"Exec runtime error. {otherExc.Message}", Serilog.Events.LogEventLevel.Warning);
                TRBotLogger.Logger.Warning($"Exec runtime error. {otherExc.Message} - at\n{otherExc.StackTrace}");
            }
        }

        /// <summary>
        /// A container class holding global data accessible to arbitrary code.
        /// </summary>
        public class ExecScriptDataContainer
        {
            public BaseRoutine ThisRoutine = null;
            public BotRoutineHandler RoutineHndlr = null;
            public DataContainer DataContainer = null;
            public DateTime CurrentTimeUTC = default;

            public ExecScriptDataContainer(BaseRoutine thisRoutine, BotRoutineHandler routineHandler,
                DataContainer dataContainer, in DateTime currentTimeUTC)
            {
                ThisRoutine = thisRoutine;
                RoutineHndlr = routineHandler;
                DataContainer = dataContainer;
                CurrentTimeUTC = currentTimeUTC;
            }
        }
    }
}
