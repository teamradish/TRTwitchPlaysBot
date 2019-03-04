using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using Newtonsoft.Json;

namespace KimimaruBot
{
    public sealed class MemesCommand : BaseCommand
    {
        public const int CHAR_LIMIT = 500;

        public static Dictionary<string, string> Memes = new Dictionary<string, string>();
        public static string MemesFilePath => $"{Globals.DataPath}Memes.txt";

        private static List<string> MemesCache = new List<string>();

        public MemesCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            string text = File.ReadAllText(MemesFilePath);
            Memes = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            CacheMemesString();
        }

        public static void SaveMemesDict()
        {
            string JSON = JsonConvert.SerializeObject(Memes, Formatting.Indented);
            File.WriteAllText(MemesFilePath, JSON);
        }

        public static void CacheMemesString()
        {
            MemesCache.Clear();

            string curString = string.Empty;

            //List all memes
            string[] memes = Memes.Keys.ToArray();

            for (int i = 0; i < memes.Length; i++)
            {
                int length = memes[i].Length + curString.Length;

                if (length >= CHAR_LIMIT)
                {
                    MemesCache.Add(curString);
                    curString = string.Empty;
                }

                curString += memes[i];

                if (i < (memes.Length - 1))
                {
                    curString += ", ";
                }
            }

            if (curString != string.Empty)
            {
                MemesCache.Add(curString);
            }
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (MemesCache.Count == 0)
            {
                BotProgram.QueueMessage("There are none!");
                return;
            }

            for (int i = 0; i < MemesCache.Count; i++)
            {
                string message = (i == 0) ? "Here are the list of memes: " : string.Empty;
                message += MemesCache[i];
                BotProgram.QueueMessage(message);
            }
        }
    }
}
