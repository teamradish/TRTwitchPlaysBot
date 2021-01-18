﻿/* Copyright (C) 2019-2020 Thomas "Kimimaru" Deeb
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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;
using TRBot.Logging;

namespace TRBot.Misc
{
    /// <summary>
    /// Handles carrying out input sequences.
    /// </summary>
    public static class InputHandler
    {
        /// <summary>
        /// A simple struct wrapping an input sequence.
        /// </summary>
        private struct InputWrapper
        {
            public ParsedInput[][] InputArray;
            public GameConsole Console;
            public IVirtualControllerManager VCManager;

            public InputWrapper(ParsedInput[][] inputArray, GameConsole console, IVirtualControllerManager vcMngr)
            {
                InputArray = inputArray;
                Console = console;
                VCManager = vcMngr;
            }
        }

        /// <summary>
        /// If all inputs in a subsequence have a remaining press time greater than this value,
        /// the InputHandler will wait very briefly to save on CPU time.
        /// </summary>
        private const long CPU_TIME_SAVER_INPUT_REMAINING_MS = 20;

        public delegate void OnInputsHalted();

        /// <summary>
        /// An event invoked when all inputs are halted.
        /// </summary>
        public static event OnInputsHalted InputsHaltedEvent = null;

        /// <summary>
        /// Whether inputs are currently halted.
        /// </summary>
        public static bool InputsHalted { get; private set; } = false;

        /// <summary>
        /// The current number of running input threads.
        /// </summary>
        public static int RunningInputCount => Interlocked.CompareExchange(ref RunningInputThreads, 0, 0);

        // <summary>
        // The current number of running input threads.
        // </summary>
        private static volatile int RunningInputThreads = 0;

        /// <summary>
        /// Cancels all currently running inputs.
        /// After calling this, all inputs are officially cancelled when <see cref="RunningInputCount"/> is 0.
        /// </summary>
        private static void CancelRunningInputs()
        {
            InputsHalted = true;

            //Invoke the event
            InputsHaltedEvent?.Invoke();
        }

        /// <summary>
        /// Allows new inputs to be processed.
        /// </summary>
        public static void ResumeRunningInputs()
        {
            //Throw an exception if we resume when inputs aren't already cancelled
            if (InputsHalted == false)
            {
                throw new Exception("Inputs are being resumed when they weren't cancelled to begin with. This can lead to corruption if there are ongoing inputs.");
            }

            InputsHalted = false;
        }

        /// <summary>
        /// Stops all ongoing inputs, waiting until all inputs are completely stopped, then keeps inputs halted.
        /// The caller is responsible for calling <see cref="ResumeRunningInputs" /> to resume inputs.
        /// </summary>
        public static async void StopAndHaltAllInputs()
        {
            CancelRunningInputs();

            await WaitAllInputsStopped();
        }

        /// <summary>
        /// Stops all ongoing inputs, waiting until all inputs are completely stopped, then resumes them.
        /// </summary>
        public static async void StopThenResumeAllInputs()
        {
            //TRBotLogger.Logger.Information("Stopping all inputs!");
            
            CancelRunningInputs();

            await WaitAllInputsStopped();

            ResumeRunningInputs();

            //TRBotLogger.Logger.Information("All inputs resumed!");
        }

        private static async Task WaitAllInputsStopped()
        {
            if (RunningInputCount == 0)
            {
                return;
            }

            const int delay = 1;

            while (RunningInputCount != 0)
            {
                //TRBotLogger.Logger.Information($"Delaying {delay}ms");
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// Carries out a set of inputs.
        /// </summary>
        /// <param name="inputList">A list of lists of inputs to execute.</param>
        public static void CarryOutInput(List<List<ParsedInput>> inputList, GameConsole currentConsole, IVirtualControllerManager vcManager)
        {
            //Copy the input list over to an array, which is more performant
            //and lets us bypass redundant copying and bounds checks in certain instances
            //This matters once we've begun processing inputs since we're
            //trying to reduce the delay between pressing and releasing inputs as much as we can
            ParsedInput[][] inputArray = new ParsedInput[inputList.Count][];
            for (int i = 0; i < inputArray.Length; i++)
            {
                inputArray[i] = inputList[i].ToArray();
            }

            InputWrapper inputWrapper = new InputWrapper(inputArray, currentConsole, vcManager);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteInput), inputWrapper);
        }

        private static void ExecuteInput(object obj)
        {
            /*************************************************************
            * PERFORMANCE CRITICAL CODE                                  *
            * Even the smallest change must be thoroughly tested         *
            *************************************************************/

            //Increment running threads
            Interlocked.Increment(ref RunningInputThreads);

            //Get the input list - this should have been validated beforehand
            InputWrapper inputWrapper = (InputWrapper)obj;
            ParsedInput[][] inputArray = inputWrapper.InputArray;

            Stopwatch sw = new Stopwatch();

            List<int> indices = new List<int>(16);
            IVirtualControllerManager vcMngr = inputWrapper.VCManager;

            int controllerCount = vcMngr.ControllerCount;

            //Use Span with stack memory to avoid allocations and improve speed
            Span<int> nonWaits = stackalloc int[controllerCount];
            nonWaits.Clear();

            //This is used to track which controller ports were used across all inputs
            //This helps prevent updating controllers that weren't used at the end
            Span<int> usedControllerPorts = stackalloc int[controllerCount];
            usedControllerPorts.Clear();

            GameConsole curConsole = inputWrapper.Console;

            //Don't check for overflow to improve performance
            unchecked
            {
                for (int i = 0; i < inputArray.Length; i++)
                {
                    ref ParsedInput[] inputs = ref inputArray[i];

                    indices.Clear();

                    //Press all buttons unless it's a release input
                    for (int j = 0; j < inputs.Length; j++)
                    {
                        indices.Add(j);

                        //Get a reference to avoid copying the struct
                        ref ParsedInput input = ref inputs[j];

                        //Don't do anything for a blank input
                        if (curConsole.IsBlankInput(input) == true)
                        {
                            continue;
                        }

                        int port = input.controllerPort;

                        //Get the controller we're using
                        IVirtualController controller = vcMngr.GetController(port);

                        //These are set to 1 instead of incremented to prevent any chance of overflow
                        nonWaits[port] = 1;
                        usedControllerPorts[port] = 1;

                        if (input.release == true)
                        {
                            InputHelper.ReleaseInput(input, curConsole, controller);
                        }
                        else
                        {
                            InputHelper.PressInput(input, curConsole, controller);
                        }
                    }

                    //Update the controllers if there are non-wait inputs
                    for (int waitIdx = 0; waitIdx < nonWaits.Length; waitIdx++)
                    {
                        //Store by ref and change directly to avoid calling the indexer twice
                        ref int nonWaitVal = ref nonWaits[waitIdx];

                        if (nonWaitVal > 0)
                        {
                            IVirtualController controller = vcMngr.GetController(waitIdx);
                            controller.UpdateController();
                            nonWaitVal = 0;
                        }
                    }

                    //If this is true, we'll sleep the current thread to save on CPU time
                    bool shouldCPUWait = false;

                    sw.Start();

                    while (indices.Count > 0)
                    {
                        //End the input prematurely
                        if (InputsHalted == true)
                        {
                            goto End;
                        }

                        //If we should wait to save on CPU time, do so now
                        if (shouldCPUWait == true)
                        {
                            Thread.Sleep(1);
                        }

                        //Default to true since we don't know how much time is remaining yet
                        shouldCPUWait = true;

                        //Release buttons when we should
                        for (int j = indices.Count - 1; j >= 0; j--)
                        {
                            ref ParsedInput input = ref inputs[indices[j]];

                            //Check how much time is remaining for this input's duration
                            long diff = input.duration - sw.ElapsedMilliseconds;

                            //If there's still time left, continue to the next input
                            if (diff > 0)
                            {
                                //If it's getting close to finishing this input,
                                //indicate that we shouldn't wait to save on CPU time
                                //in case we hold it for too long
                                if (diff < CPU_TIME_SAVER_INPUT_REMAINING_MS)
                                {
                                    shouldCPUWait = false;
                                }

                                continue;
                            }

                            //TRBotLogger.Logger.Debug($"diff for \"{input.name}{input.duration}\": {diff}");

                            //Release if the input isn't held or released and isn't a blank input
                            if (input.hold == false && input.release == false && curConsole.IsBlankInput(input) == false)
                            {
                                int port = input.controllerPort;
                                
                                //Get the controller we're using
                                IVirtualController controller = vcMngr.GetController(port);

                                InputHelper.ReleaseInput(input, curConsole, controller);

                                //Track that we have a non-wait or hold input so we can update the controller with all input releases at once
                                nonWaits[port] = 1;

                                usedControllerPorts[port] = 1;
                            }

                            indices.RemoveAt(j);
                        }

                        //Update the controllers if there are non-wait inputs
                        for (int waitIdx = 0; waitIdx < nonWaits.Length; waitIdx++)
                        {
                            //Store by ref and change directly to avoid calling the indexer twice
                            ref int nonWaitVal = ref nonWaits[waitIdx];
                            
                            if (nonWaitVal > 0)
                            {
                                IVirtualController controller = vcMngr.GetController(waitIdx);
                                controller.UpdateController();

                                nonWaitVal = 0;
                            }
                        }
                    }

                    sw.Reset();
                }
            }

            //End label to skip to if we should cancel early
            End:

            unchecked
            {
                //At the end of it all, release every input
                for (int i = 0; i < inputArray.Length; i++)
                {
                    ref ParsedInput[] inputs = ref inputArray[i];
                    for (int j = 0; j < inputs.Length; j++)
                    {
                        ref ParsedInput input = ref inputs[j];

                        if (curConsole.IsBlankInput(input) == true)
                        {
                            continue;
                        }

                        //Release if it isn't a wait
                        IVirtualController controller = vcMngr.GetController(input.controllerPort);
                        InputHelper.ReleaseInput(input, curConsole, controller);
                    }
                }
            
                //Update all used controllers
                for (int i = 0; i < usedControllerPorts.Length; i++)
                {
                    //A value of 0 indicates the port wasn't used
                    if (usedControllerPorts[i] == 0)
                    {
                        continue;
                    }

                    IVirtualController controller = vcMngr.GetController(i);
                    controller.UpdateController();
                }
            }

            //Decrement running threads
            Interlocked.Decrement(ref RunningInputThreads);
        }
    }
}
