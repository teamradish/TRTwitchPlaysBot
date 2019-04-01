using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Execute arbitrary C# code (simple). Admins only.
    /// </summary>
    public sealed class ExecCommand : BaseCommand
    {
        private ScriptOptions ScriptCompileOptions = null;

        //Kimimaru: Add references and assemblies
        private readonly Assembly[] References = new Assembly[] { typeof(ExecCommand).Assembly, typeof(Console).Assembly };
        private readonly string[] Imports = new string[] { nameof(KimimaruBot), nameof(System) };

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
