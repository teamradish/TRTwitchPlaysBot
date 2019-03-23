using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimimaruBot
{
    /// <summary>
    /// Global values.
    /// </summary>
    public static class Globals
    {
        //Kimimaru: NOTE - Hackish
        //We should link the data folder so that it's referenced from the executable dir, but .NET Core projects don't yet support it from the IDE
        //Do it manually via editing the .csproj file
        public static string RootDir => Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName).FullName).FullName).FullName;
        public static readonly string DataPath = $"{Path.Combine(RootDir, $"Data{Path.DirectorySeparatorChar}")}";
        public const char CommandIdentifier = '!';
        public const char MacroIdentifier = '#';

        public static string GetDataFilePath(in string fileName)
        {
            return $"{DataPath}{fileName}";
        }
    }
}
