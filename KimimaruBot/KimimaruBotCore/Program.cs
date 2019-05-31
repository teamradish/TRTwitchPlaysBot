using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimimaruBot
{
    class Program
    {
        static void Main(string[] args)
        {
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
