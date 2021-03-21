/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
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
using System.IO;
using TRBot.Connection;
using TRBot.Permissions;
using TRBot.Data;
using TRBot.Utilities;

namespace TRBot.Commands
{
    /// <summary>
    /// Sets a game message that can be displayed on the stream.
    /// </summary>
    public sealed class SetGameMessageCommand : SaveTextToFileCommand
    {
        protected override bool PathIsRelative => DataHelper.GetSettingInt(SettingsConstants.GAME_MESSAGE_PATH_IS_RELATIVE, 1L) == 1;

        protected override string PermissionAbilityRequired => PermissionConstants.SET_GAME_MESSAGE_ABILITY;

        protected override string PermissionDeniedMessage => "You don't have the ability to set the game message!";

        public SetGameMessageCommand()
        {

        }
    }
}
