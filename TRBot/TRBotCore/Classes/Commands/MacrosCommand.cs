using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class MacrosCommand : BaseCommand
    {
        private StringBuilder StrBuilder = new StringBuilder(500);
        private List<string> MultiMessageCache = new List<string>(16);
        private const string InitMessage = "Macros: ";

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotData.Macros.Count == 0)
            {
                BotProgram.QueueMessage("There are no macros!");
                return;
            }

            List<string> macros = BotProgram.BotData.Macros.Keys.ToList();

            MultiMessageCache.Clear();
            StrBuilder.Clear();

            for (int i = 0; i < macros.Count; i++)
            {
                string macroName = macros[i];

                int newLength = StrBuilder.Length + macroName.Length + 3;
                int maxLength = Globals.BotCharacterLimit;
                if (MultiMessageCache.Count == 0)
                {
                    maxLength -= InitMessage.Length;
                }

                //Send in multiple messages if it exceeds the length
                if (newLength >= maxLength)
                {
                    MultiMessageCache.Add(StrBuilder.ToString());
                    StrBuilder.Clear();
                }

                StrBuilder.Append(macroName).Append(", ");
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            MultiMessageCache.Add(StrBuilder.ToString());

            for (int i = 0; i < MultiMessageCache.Count; i++)
            {
                if (i == 0)
                {
                    BotProgram.QueueMessage($"{InitMessage}{MultiMessageCache[i]}");
                }
                else
                {
                    BotProgram.QueueMessage(MultiMessageCache[i]);
                }
            }
        }
    }
}
