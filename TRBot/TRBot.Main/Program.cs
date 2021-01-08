using System;
using TRBot.Logging;

namespace TRBot.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set current directory to where the program was run from
            string assemblyLoc = typeof(Program).Assembly.Location;
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(assemblyLoc);

            Console.WriteLine($"Set current working directory to: {Environment.CurrentDirectory}");
            
            using (BotProgram botProgram = new BotProgram())
            {
                botProgram.Initialize();

                if (botProgram.Initialized == true)
                {
                    botProgram.Run();
                }
                else
                {
                    Console.WriteLine("Bot failed to initialize. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}
