using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    public sealed class MacrosCommand : BaseCommand
    {
        private StringBuilder StrBuilder = new StringBuilder(500);

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (BotProgram.BotData.Macros.Count == 0)
            {
                BotProgram.QueueMessage("There are no macros!");
                return;
            }

            List<string> macros = BotProgram.BotData.Macros.Keys.ToList();

            StrBuilder.Clear();

            StrBuilder.Append("Macros: ");

            for (int i = 0; i < macros.Count; i++)
            {
                StrBuilder.Append(macros[i]).Append(", ");
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            BotProgram.QueueMessage(StrBuilder.ToString());
        }
    }
}
