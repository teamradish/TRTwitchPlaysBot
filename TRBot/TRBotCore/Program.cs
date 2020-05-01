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
using System.Text;
using System.Threading.Tasks;

namespace TRBot
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
