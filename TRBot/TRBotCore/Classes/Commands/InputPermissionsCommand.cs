using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Client.Events;

namespace TRBot
{
    public sealed class InputPermissionsCommand : BaseCommand
    {
        private StringBuilder StrBuilder = null;

        public override void Initialize(CommandHandler commandHandler)
        {
            base.Initialize(commandHandler);
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            //See the permissions
            if (args.Count == 0)
            {
                BotProgram.QueueMessage($"Inputs are allowed for {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above. To set the permissions, add one as an argument: {GetValidPermsStr()}");
                return;
            }

            string permsStr = args[0];

            if (Enum.TryParse<AccessLevels.Levels>(permsStr, true, out AccessLevels.Levels perm) == false)
            {
                BotProgram.QueueMessage($"Please enter a valid permission: {GetValidPermsStr()}");
                return;
            }

            if ((int)perm == BotProgram.BotData.InputPermissions)
            {
                BotProgram.QueueMessage($"The permissions are already {(AccessLevels.Levels)BotProgram.BotData.InputPermissions}!");
                return;
            }

            BotProgram.BotData.InputPermissions = (int)perm;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage($"Set input permissions to {(AccessLevels.Levels)BotProgram.BotData.InputPermissions} and above!");
        }

        private string GetValidPermsStr()
        {
            string[] names = EnumUtility.GetNames<AccessLevels.Levels>.EnumNames;

            if (StrBuilder == null)
            {
                StrBuilder = new StringBuilder(500);
            }

            StrBuilder.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                StrBuilder.Append(names[i]).Append(',').Append(' ');
            }

            StrBuilder.Remove(StrBuilder.Length - 2, 2);

            return StrBuilder.ToString();
        }
    }
}
