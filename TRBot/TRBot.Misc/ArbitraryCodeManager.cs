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
        private int VControllerCount = 0;

        //Record all delegates so it's possible to unsubscribe from them later
        private Dictionary<int, OnInputPressed> InputPressedDelegates = new Dictionary<int, OnInputPressed>(4);
        private Dictionary<int, OnInputReleased> InputReleasedDelegates = new Dictionary<int, OnInputReleased>(4);
        private Dictionary<int, OnControllerUpdated> ControllerUpdatedDelegates = new Dictionary<int, OnControllerUpdated>(4);

        /// <summary>
        /// Records the currently pressed inputs on each virtual controller.
        /// The key is the controller port and the value is a dictionary of input names.  
        /// </summary>
        /// <remarks>
        /// A dictionary is used as the value for a faster lookup time.
        /// Since this will be running on the same thread as the <see cref="InputHandler" />, it needs to be as fast as possible.
        /// </remarks>
        private Dictionary<int, Dictionary<string, bool>> InputsPressed = new Dictionary<int, Dictionary<string, bool>>(16);

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

            //Unsubscribe from all delegates
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
            //Don't do anything if the reference and controller counts are the same
            if (VControllerMngr == vControllerMngr && VControllerCount == vControllerMngr.ControllerCount)
            {
                return;
            }

            CleanUp();

            VControllerMngr = vControllerMngr;
            VControllerCount = VControllerMngr.ControllerCount;

            for (int i = 0; i < VControllerMngr.ControllerCount; i++)
            {
                //Store the index locally for the delegate to capture it properly
                //Otherwise, the delegate would use the value of i at the end of the loop only 
                int port = i;

                //Store all the events in dictionaries so we can unsubscribe from them later
                //We have to use lambdas to set the correct ports
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
            //Add if not present
            if (InputsPressed.TryGetValue(controllerPort, out Dictionary<string, bool> inputDict) == false)
            {
                inputDict = new Dictionary<string, bool>(8);
                InputsPressed.Add(controllerPort, inputDict);
            }

            inputDict[inputName] = true;
        }

        private void VControllerInputReleased(in string inputName, in int controllerPort)
        {
            if (InputsPressed.TryGetValue(controllerPort, out Dictionary<string, bool> inputDict) == true)
            {
                //Remove this so iteration is faster when executing the code
                inputDict.Remove(inputName);
            }
        }

        private void VControllerUpdated(in int controllerPort)
        {
            if (InputsPressed.TryGetValue(controllerPort, out Dictionary<string, bool> inputDict) == false)
            {
                return;
            }

            //Get all pressed inputs
            string[] allInputs = inputDict.Keys.ToArray();

            //See which inputs have custom code
            
            //(Filter by input names here)

            //Check if the file for the custom code on each input exists

            //Invoke the async method to run the code
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
