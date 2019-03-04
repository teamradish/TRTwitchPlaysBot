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

                botProgram.Run();
            }
        }
    }
}
