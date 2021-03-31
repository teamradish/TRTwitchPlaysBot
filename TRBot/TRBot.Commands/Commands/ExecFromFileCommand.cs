/* Copyright (C) 2019-2021 Thomas "Kimimaru" Deeb
 * 
 * This file is part of TRBot, software for playing games through text.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, version 3 of the License.
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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TRBot.Connection;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.Consoles;
using TRBot.Data;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace TRBot.Commands
{
    /// <summary>
    /// Execute arbitrary C# code from a given file. This allows for custom commands.
    /// These custom source files are subject to the same license terms as TRBot.
    /// <para>
    /// It's highly recommended to have this accessible ONLY to the streamer.
    /// This has potential to corrupt, delete, or modify data and is provided as a convenience
    /// for carrying out whatever cannot be done normally through TRBot while the application is running.
    /// You are responsible for entrusting use of this command to others.
    /// </para>
    /// </summary>
    public class ExecFromFileCommand : ExecCommand
    {
        public ExecFromFileCommand()
        {

        }

        public override void ExecuteCommand(EvtChatCommandArgs args)
        {
            if (string.IsNullOrEmpty(ValueStr) == true)
            {
                QueueMessage("Invalid file defined.");
                return;
            }

            //First try to read the file as an absolute path
            string codeText = FileHelpers.ReadFromTextFile(ValueStr);

            //If that wasn't found, try a relative path
            if (string.IsNullOrEmpty(codeText) == true)
            {
                codeText = FileHelpers.ReadFromTextFile(DataConstants.DataFolderPath, ValueStr);
            }

            if (string.IsNullOrEmpty(codeText) == true)
            {
                QueueMessage("Invalid source file. Double check its location.", Serilog.Events.LogEventLevel.Warning);
                return;
            }

            //Execute the code
            ExecuteCSharpScript(codeText, args);
        }
    }
}
