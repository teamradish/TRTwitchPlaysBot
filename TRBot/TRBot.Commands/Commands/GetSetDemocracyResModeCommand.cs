/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot,software for playing games through text.
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
using System.Text;
using TRBot.Connection;
using TRBot.VirtualControllers;
using TRBot.Data;
using TRBot.Utilities;
using TRBot.Misc;
using TRBot.Permissions;

namespace TRBot.Commands
{
    /// <summary>
    /// Displays or changes the current resolution mode in the Democracy input mode.
    /// </summary>
    public sealed class GetSetDemocracyResModeCommand : BaseCommand
    {
        private string UsageMessage = "Usage: \"resolution mode (string/int)\"";
        private string CachedResolutionModesStr = string.Empty;
        
        public GetSetDemocracyResModeCommand()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            //Show all resolution modes
            DemocracyResolutionModes[] resModeArr = EnumUtility.GetValues<DemocracyResolutionModes>.EnumValues;

            for (int i = 0; i < resModeArr.Length; i++)
            {
                DemocracyResolutionModes resMode = resModeArr[i];

                CachedResolutionModesStr += resMode.ToString();

                if (i < (resModeArr.Length - 1))
                {
                    CachedResolutionModesStr += ", ";
                }
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            List<string> arguments = args.Command.ArgumentsAsList;

            long curResMode = DataHelper.GetSettingInt(SettingsConstants.DEMOCRACY_RESOLUTION_MODE, 0L);
            DemocracyResolutionModes resMode = (DemocracyResolutionModes)curResMode;

            //See the virtual controller
            if (arguments.Count == 0)
            {
                QueueMessage($"The current Democracy resolution mode is {resMode}. To set the resolution mode, add one as an argument: {CachedResolutionModesStr}");
                return;
            }

            //Invalid number of arguments
            if (arguments.Count > 1)
            {
                QueueMessage(UsageMessage);
                return;
            }

            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Check if the user has the ability to set the mode
                User user = DataHelper.GetUserNoOpen(args.Command.ChatMessage.Username, context);

                if (user != null && user.HasEnabledAbility(PermissionConstants.SET_DEMOCRACY_RESOLUTION_MODE_ABILITY) == false)
                {
                    QueueMessage("You don't have the ability to set the Democracy resolution mode!");
                    return;
                }
            }

            string resModeStr = arguments[0];

            //Parse
            if (EnumUtility.TryParseEnumValue(resModeStr, out DemocracyResolutionModes parsedResMode) == false)
            {
                QueueMessage($"Please enter a resolution mode: {CachedResolutionModesStr}");
                return;
            }

            //Same mode
            if (parsedResMode == resMode)
            {
                QueueMessage($"The current resolution mode is already {resMode}!");
                return;
            }
            
            using (BotDBContext context = DatabaseManager.OpenContext())
            {
                //Set the value and save
                Settings resModeSetting = DataHelper.GetSettingNoOpen(SettingsConstants.DEMOCRACY_RESOLUTION_MODE, context);
                resModeSetting.ValueInt = (long)parsedResMode;

                context.SaveChanges();
            }
            
            QueueMessage($"Changed the Democracy resolution mode from {resMode} to {parsedResMode}!");
        }
    }
}
