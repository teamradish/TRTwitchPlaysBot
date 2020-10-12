/* This file is part of TRBot.
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
    public sealed class ExecCommand : BaseCommand
    {
        //Add references and assemblies
        private readonly Assembly[] References = new Assembly[]
        {
            typeof(Console).Assembly,
            typeof(List<int>).Assembly,
            typeof(ExecCommand).Assembly,
            typeof(Parsing.Parser).Assembly,
            typeof(ParserData.InputMacro).Assembly,
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
            $"{nameof(TRBot)}.{nameof(TRBot.ParserData)}",
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
            Console.WriteLine(nameof(TRBot.Parsing));
        }

        public override void Initialize(CommandHandler cmdHandler, DataContainer dataContainer)
        {
            base.Initialize(cmdHandler, dataContainer);

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

            ExecuteCSharpScript(code);
        }

        private async void ExecuteCSharpScript(string code)
        {
            //Store the default console output stream
            TextWriter defaultOut = Console.Out;

            bool prevIgnoreConsoleLogVal = DataContainer.MessageHandler.LogToConsole;

            try
            {
                //Output any Console output to the chat
                //To do this, we're overriding the Console's output stream
                using (BotWriter writer = new BotWriter(DataContainer.MessageHandler))
                {
                    Console.SetOut(writer);
                    DataContainer.MessageHandler.SetLogToConsole(false);

                    var script = await CSharpScript.RunAsync(code, ScriptCompileOptions);

                    Console.SetOut(defaultOut);
                    DataContainer.MessageHandler.SetLogToConsole(prevIgnoreConsoleLogVal);
                }
            }
            catch (CompilationErrorException exception)
            {
                QueueMessage($"Compiler error: {exception.Message}");
            }
            //Regardless of what happens, return the output stream to the default
            finally
            {
                Console.SetOut(defaultOut);
                DataContainer.MessageHandler.SetLogToConsole(prevIgnoreConsoleLogVal);
            }
        }

        /// <summary>
        /// TextWriter class that outputs a message through the bot.
        /// </summary>
        public class BotWriter : TextWriter
        {
            public override Encoding Encoding => System.Text.Encoding.Default;

            private BotMessageHandler MessageHandler = null;

            public BotWriter(BotMessageHandler msgHandler)
            {
                MessageHandler = msgHandler;
            }

            public override void Write(string value)
            {
                MessageHandler.QueueMessage(value);
            }

            public override void WriteLine(string value)
            {
                MessageHandler.QueueMessage(value);
            }
        }
    }
}
