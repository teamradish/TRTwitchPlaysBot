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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using static TRBot.VirtualControllers.VirtualControllerDelegates;

namespace TRBot.Misc
{
    /// <summary>
    /// Handles executing arbitrary code when certain inputs are pressed.
    /// </summary>
    public class ArbitraryCodeManager
    {
        private IVirtualControllerManager VControllerMngr = null;

        private Dictionary<int, OnInputPressed> InputPressedDelegates = new Dictionary<int, OnInputPressed>(4);
        private Dictionary<int, OnInputReleased> InputReleasedDelegates = new Dictionary<int, OnInputReleased>(4);
        private Dictionary<int, OnControllerUpdated> ControllerUpdatedDelegates = new Dictionary<int, OnControllerUpdated>(4);

        private Dictionary<string, bool> InputsPressed = new Dictionary<string, bool>(16);

        public ArbitraryCodeManager()
        {
            
        }

        public void CleanUp()
        {
            if (VControllerMngr == null)
            {
                ClearDictionaries();
                return;
            }

            for (int i = 0; i < VControllerMngr.ControllerCount; i++)
            {
                IVirtualController controller = VControllerMngr.GetController(i);

                if (InputPressedDelegates.TryGetValue(i, out OnInputPressed pressedDelegate) == true)
                {
                    controller.InputPressedEvent -= pressedDelegate;
                }   

                if (InputReleasedDelegates.TryGetValue(i, out OnInputReleased releasedDelegate) == true)
                {
                    controller.InputReleasedEvent -= releasedDelegate;
                } 

                if (ControllerUpdatedDelegates.TryGetValue(i, out OnControllerUpdated updatedDelegate) == true)
                {
                    controller.ControllerUpdatedEvent -= updatedDelegate;
                }   
            }

            ClearDictionaries();
        }

        public void SetVControllerManager(IVirtualControllerManager vControllerMngr)
        {
            //Don't do anything if the values are equal
            if (VControllerMngr == vControllerMngr)
            {
                return;
            }

            CleanUp();

            VControllerMngr = vControllerMngr;

            for (int i = 0; i < VControllerMngr.ControllerCount; i++)
            {
                //Store the index locally for the delegate to capture it properly
                //Otherwise, the delegate would use the value of i at the end of the loop only 
                int port = i;

                //Store all the events in dictionaries so we can unsubscribe from them later
                //We have to use lambdas to 
                OnInputPressed pressedDelegate = (in string inputName) => VControllerInputPressed(inputName, port);
                InputPressedDelegates.Add(port, pressedDelegate);

                OnInputReleased releasedDelegate = (in string inputName) => VControllerInputReleased(inputName, port);
                InputReleasedDelegates.Add(port, releasedDelegate);

                OnControllerUpdated updatedDelegate = () => VControllerUpdated(port);
                ControllerUpdatedDelegates.Add(port, updatedDelegate);
            }
        }

        private void VControllerInputPressed(in string inputName, in int controllerPort)
        {

        }

        private void VControllerInputReleased(in string inputName, in int controllerPort)
        {

        }

        private void VControllerUpdated(in int controllerPort)
        {
            
        }

        private void ClearDictionaries()
        {
            InputPressedDelegates.Clear();
            InputReleasedDelegates.Clear();
            ControllerUpdatedDelegates.Clear();

            InputsPressed.Clear();
        }
    }
}
