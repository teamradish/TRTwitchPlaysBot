using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class HelpCommand : BaseCommand
    {
        private KeyValuePair<string, BaseCommand>[] CommandCache = null;
        private StringBuilder StrBuilder = new StringBuilder(1000);
        private List<string> MultiMessageCache = new List<string>(16);
        private bool Sorted = false;
        private const string InitMessage = "Here's a list of all commands: ";

        public HelpCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            //Cache command string
            CommandCache = commandHandler.CommandDict.ToArray();
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            //Have to do this on first execute since not every command would have been initialized by the time Initialize is called
            if (Sorted == false)
            {
                Array.Sort<KeyValuePair<string, BaseCommand>>(CommandCache, CompareCmdKVPair);
                Sorted = true;
            }

            User user = BotProgram.GetOrAddUser(e.Command.ChatMessage.Username, false);

            MultiMessageCache.Clear();
            StrBuilder.Clear();

            for (int i = 0; i < CommandCache.Length; i++)
            {
                ref KeyValuePair<string, BaseCommand> cmd = ref CommandCache[i];

                //Ignore if the command is hidden from the help menu
                if (cmd.Value.HiddenFromHelp == true) continue;

                //Show the command only if the user has access to it
                if (user.Level >= cmd.Value.AccessLevel)
                {
                    int newLength = StrBuilder.Length + cmd.Key.Length + 3;
                    int maxLength = Globals.BotCharacterLimit;
                    if (MultiMessageCache.Count == 0)
                    {
                        maxLength -= InitMessage.Length;
                    }

                    //Send in multiple messages if it exceeds the length
                    //NOTE: Do the same thing for memes and macros
                    if (newLength >= maxLength)
                    {
                        MultiMessageCache.Add(StrBuilder.ToString());
                        StrBuilder.Clear();
                    }

                    StrBuilder.Append(Globals.CommandIdentifier).Append(cmd.Key).Append(", ");
                }
            }

            int lastTwo = StrBuilder.Length - 2;

            //Remove trailing comma
            if (lastTwo >= 0)
            {
                StrBuilder.Remove(lastTwo, 2);
            }

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

        private int CompareCmdKVPair(KeyValuePair<string, BaseCommand> pair1, KeyValuePair<string, BaseCommand> pair2)
        {
            if (pair1.Value.AccessLevel > pair2.Value.AccessLevel) return -1;
            else if (pair1.Value.AccessLevel < pair2.Value.AccessLevel) return 1;

            return string.Compare(pair1.Key, pair2.Key, StringComparison.InvariantCulture);
        }
    }
}
