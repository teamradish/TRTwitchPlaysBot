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
using System.Linq;
using TRBot.Misc;
using TRBot.Utilities;
using TRBot.VirtualControllers;

namespace TRBot.Data
{
    /// <summary>
    /// A container for common data.
    /// This is used to pass specific data around among objects.
    /// When the data is changed in one object, it will reflect across all other objects holding onto this container.
    /// </summary>
    public class DataContainer
    {
        public BotMessageHandler MessageHandler { get; private set; } = null;
        public DataReloader DataReloader { get; private set; } = null;
        public IVirtualControllerManager ControllerMngr { get; private set; } = null;

        //Unsure if this is the best way to track changes
        //The sole purpose of this is to compare this value with the new virtual controller type
        //This allows us to change the virtual controller if the value in the database changed
        public VirtualControllerTypes CurVControllerType { get; private set; } = VirtualControllerTypes.Dummy;

        public DataContainer()
        {

        }

        public void SetMessageHandler(BotMessageHandler msgHandler)
        {
            MessageHandler = msgHandler;
        }

        public void SetDataReloader(DataReloader dataReloader)
        {
            DataReloader = dataReloader;
        }

        public void SetControllerManager(IVirtualControllerManager controllerMngr)
        {
            ControllerMngr = controllerMngr;
        }

        public void SetCurVControllerType(in VirtualControllerTypes curVControllerType)
        {
            CurVControllerType = curVControllerType;
        }
    }
}
