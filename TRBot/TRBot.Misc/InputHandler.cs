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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;

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
        /// The current number of running input sequences.
        /// </summary>
        public static int CurrentRunningInputs => Interlocked.CompareExchange(ref RunningInputThreads, 0, 0);

        /// <summary>
        /// The current number of running input threads.
        /// </summary>
        private static volatile int RunningInputThreads = 0;

        /// <summary>
        /// Whether inputs are being stopped.
        /// </summary>
        public static bool StopRunningInputs { get; private set; } = false;

        /// <summary>
        /// Cancels all currently running inputs.
        /// After calling this, all inputs are officially cancelled when <see cref="CurrentRunningInputs"/> is 0.
        /// </summary>
        private static void CancelRunningInputs()
        {
            StopRunningInputs = true;
        }

        /// <summary>
        /// Allows new inputs to be processed.
        /// </summary>
        private static void ResumeRunningInputs()
        {
            StopRunningInputs = false;
        }

        /// <summary>
        /// Stops all ongoing inputs.
        /// </summary>
        public static async void StopAllInputs()
        {
            //Console.WriteLine("Stopping all inputs!");
            
            CancelRunningInputs();

            await WaitAllInputsStopped();

            ResumeRunningInputs();

            //Console.WriteLine("All inputs resumed!");
        }

        private static async Task WaitAllInputsStopped()
        {
            const int delay = 1;

            while (CurrentRunningInputs != 0)
            {
                Console.WriteLine($"Delaying {delay}ms");
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// Carries out a set of inputs.
        /// </summary>
        /// <param name="inputList">A list of lists of inputs to execute.</param>
        public static void CarryOutInput(List<List<ParsedInput>> inputList, GameConsole currentConsole, IVirtualControllerManager vcManager)
        {
            /*Kimimaru: We're using a thread pool for efficiency
             * Though very unlikely, there's a chance the input won't execute right away if it has to wait for a thread to be available
             * However, there are often plenty of available threads, so this shouldn't be an issue since we use only one thread per input string
             * Uncomment the following lines to see how many threads are supported in the pool on your machine */
            //ThreadPool.GetMinThreads(out int workermin, out int completionmin);
            //ThreadPool.GetMaxThreads(out int workerthreads, out int completionPortThreads);
            //Console.WriteLine($"Min workers: {workermin} Max workers: {workerthreads} Min async IO threads: {completionmin} Max async IO threads: {completionPortThreads}");

            //Kimimaru: Copy the input list over to an array, which is more performant
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
            int[] nonWaits = new int[controllerCount];

            //This is used to track which controller ports were used across all inputs
            //This helps prevent updating controllers that weren't used at the end
            int[] usedControllerPorts = new int[controllerCount];

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
                        if (nonWaits[waitIdx] > 0)
                        {
                            IVirtualController controller = vcMngr.GetController(waitIdx);
                            controller.UpdateController();
                            nonWaits[waitIdx] = 0;
                        }
                    }

                    sw.Start();

                    while (indices.Count > 0)
                    {
                        //End the input prematurely
                        if (StopRunningInputs == true)
                        {
                            goto End;
                        }

                        //Release buttons when we should
                        for (int j = indices.Count - 1; j >= 0; j--)
                        {
                            ref ParsedInput input = ref inputs[indices[j]];

                            if (sw.ElapsedMilliseconds < input.duration)
                            {
                                continue;
                            }

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
                            if (nonWaits[waitIdx] > 0)
                            {
                                IVirtualController controller = vcMngr.GetController(waitIdx);
                                controller.UpdateController();

                                nonWaits[waitIdx] = 0;
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
