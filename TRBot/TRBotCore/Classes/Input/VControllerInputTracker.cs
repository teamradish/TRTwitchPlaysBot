/* This file is part of TRBot.
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
using System.Collections.Concurrent;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Helps track the states of inputs on a virtual controller and invokes input callbacks.
    /// </summary>
    public class VControllerInputTracker
    {
        private ConcurrentDictionary<string, ButtonStates> CurInputStates = new ConcurrentDictionary<string, ButtonStates>(Environment.ProcessorCount * 2, 32);
        private ConcurrentDictionary<string, ButtonStates> TempInputStates = new ConcurrentDictionary<string, ButtonStates>(Environment.ProcessorCount * 2, 32);

        private ConcurrentDictionary<uint, ButtonStates> CurButtonStates = new ConcurrentDictionary<uint, ButtonStates>(Environment.ProcessorCount * 2, 32);
        private ConcurrentDictionary<uint, ButtonStates> TempButtonStates = new ConcurrentDictionary<uint, ButtonStates>(Environment.ProcessorCount * 2, 32);

        private ConcurrentDictionary<int, int> CurAxesStates = new ConcurrentDictionary<int, int>(Environment.ProcessorCount * 2, 32);
        private ConcurrentDictionary<int, int> TempAxesStates = new ConcurrentDictionary<int, int>(Environment.ProcessorCount * 2, 32);

        public void ResetStates()
        {
            CurInputStates.Clear();
            TempInputStates.Clear();
            
            CurButtonStates.Clear();
            TempButtonStates.Clear();

            CurAxesStates.Clear();
            TempAxesStates.Clear();
        }

        public ButtonStates GetInputState(in string inputName)
        {
            if (CurInputStates.TryGetValue(inputName, out ButtonStates btnState) == true)
            {
                return btnState;
            }
            
            return ButtonStates.Released;
        }

        public ButtonStates GetButtonState(in uint buttonVal)
        {
            if (CurButtonStates.TryGetValue(buttonVal, out ButtonStates btnState) == true)
            {
                return btnState;
            }
            
            return ButtonStates.Released;
        }

        public int GetAxisState(in int axisVal)
        {
            if (CurAxesStates.TryGetValue(axisVal, out int axisPercent) == true)
            {
                return axisPercent;
            }

            return 0;
        }

        public void PressInput(in string inputName)
        {
            TempInputStates[inputName] = ButtonStates.Pressed;
        }

        public void ReleaseInput(in string inputName)
        {
            TempInputStates[inputName] = ButtonStates.Released;
        }

        public void PressButton(in uint buttonVal)
        {
            TempButtonStates[buttonVal] = ButtonStates.Pressed;
        }

        public void ReleaseButton(in uint buttonVal)
        {
            TempButtonStates[buttonVal] = ButtonStates.Released;
        }

        public void PressAxis(in int axisVal, in int percent)
        {
            TempAxesStates[axisVal] = percent;
        }

        public void ReleaseAxis(in int axisVal)
        {
            TempAxesStates[axisVal] = 0;
        }

        public void UpdateCurrentStates()
        {
            //Copy button states over
            CurButtonStates.CopyDictionaryData(TempButtonStates);

            //Copy axis states over
            CurAxesStates.CopyDictionaryData(TempAxesStates);

            Dictionary<string, InputCallback> inputCallbacks = BotProgram.InputCBData.Callbacks;

            //Check for differences in the temp and current input states to invoke input callbacks
            //Then copy them over
            foreach (KeyValuePair<string, ButtonStates> kvPair in TempInputStates)
            {
                string inputName = kvPair.Key;
                ButtonStates pressedState = kvPair.Value;

                //Console.WriteLine($"{inputName} is {pressedState}");

                //Invoke input callbacks
                if (inputCallbacks.TryGetValue(inputName, out InputCallback cbData) == true)
                {
                    long invocation = (long)cbData.InvocationType;

                    bool contains = CurInputStates.TryGetValue(inputName, out ButtonStates prevState);

                    //Console.WriteLine($"{inputName} WAS {prevState}");

                    //Console.WriteLine($"Invocation is {cbData.InvocationType} of type {cbData.Callback.GetInvocationList()[0].Method.Name} with val {cbData.CBValue.ToString()} | " + 
                    //    $"Has press = {EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Press) == true} | " +
                    //    $"Has hold = {EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Hold) == true} | " +
                    //    $"Has release = {EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Release) == true}");

                    //Check for invoking on press or hold
                    if (pressedState == ButtonStates.Pressed && prevState == ButtonStates.Released)
                    {
                        //Console.WriteLine($"{inputName} just pressed or held");

                        if (EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Press) == true
                            || EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Hold) == true)
                        {
                            //Console.WriteLine($"Invoking cb {cbData.Callback}");
                            cbData.Callback?.Invoke(cbData.CBValue);
                        }
                    }
                    //Check for invoking on release
                    else if (pressedState == ButtonStates.Released && prevState == ButtonStates.Pressed)
                    {
                        //Console.WriteLine($"{inputName} just released");
                        
                        if (EnumUtility.HasEnumVal(invocation, (long)InputCBInvocation.Release) == true)
                        {
                            //Console.WriteLine($"Invoking cb {cbData.Callback}");
                            cbData.Callback?.Invoke(cbData.CBValue);
                        }
                    }
                }

                //Update current state from the temp state
                CurInputStates[inputName] = pressedState;
            }
        }

        public string[] GetPressedInputs()
        {
            List<string> pressedInputs = new List<string>(CurInputStates.Count);
            foreach (KeyValuePair<string, ButtonStates> kvPair in CurInputStates)
            {
                if (kvPair.Value == ButtonStates.Pressed)
                {
                    pressedInputs.Add(kvPair.Key);
                }
            }

            if (pressedInputs.Count == 0)
            {
                return Array.Empty<string>();
            }
            
            return pressedInputs.ToArray();
        }
    }
}
