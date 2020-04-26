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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Execute arbitrary C# code (simple). Admins only.
    /// </summary>
    public sealed class ExecCommand : BaseCommand
    {
        private ScriptOptions ScriptCompileOptions = null;

        //Kimimaru: Add references and assemblies
        private readonly Assembly[] References = new Assembly[] { typeof(ExecCommand).Assembly, typeof(Console).Assembly };
        private readonly string[] Imports = new string[] { Assembly.GetExecutingAssembly().GetName().Name, nameof(System) };

        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Admin;

            ScriptCompileOptions = ScriptOptions.Default.
                WithReferences(References).WithImports(Imports);
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            string code = e.Command.ArgumentsAsString;

            if (string.IsNullOrEmpty(code) == true)
            {
                BotProgram.QueueMessage($"Usage: \"C# code\"");
                return;
            }

            ExecuteCSharpScript(code);
        }

        private async void ExecuteCSharpScript(string code)
        {
            //Kimimaru: Store the default console output stream
            TextWriter defaultOut = Console.Out;

            try
            {
                //Kimimaru: Output any Console output to the chat
                //To do this, we're overriding the Console's output stream
                using (BotWriter writer = new BotWriter())
                {
                    Console.SetOut(writer);
                    BotProgram.IgnoreConsoleLog = true;

                    var script = await CSharpScript.RunAsync(code, ScriptCompileOptions);

                    Console.SetOut(defaultOut);
                    BotProgram.IgnoreConsoleLog = false;
                }
            }
            catch (CompilationErrorException exception)
            {
                BotProgram.QueueMessage($"Compiler error: {exception.Message}");
            }
            //Regardless of what happens, return the output stream to the default
            finally
            {
                Console.SetOut(defaultOut);
                BotProgram.IgnoreConsoleLog = false;
            }
        }

        /// <summary>
        /// TextWriter class that outputs a message through the bot.
        /// </summary>
        public class BotWriter : TextWriter
        {
            public override Encoding Encoding => System.Text.Encoding.Default;

            public override void Write(string value)
            {
                BotProgram.QueueMessage(value);
            }

            public override void WriteLine(string value)
            {
                BotProgram.QueueMessage(value);
            }
        }
    }
}
