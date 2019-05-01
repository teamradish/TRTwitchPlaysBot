using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace KimimaruBot
{
    /// <summary>
    /// Handles carrying out input sequences.
    /// </summary>
    public static class InputHandler
    {
        /// <summary>
        /// A simple struct containing an input sequence and the controller number to perform the sequence on.
        /// </summary>
        private struct InputWrapper
        {
            public int ControllerNum;
            public List<List<Parser.Input>> InputList;

            public InputWrapper(in int controllerNum, List<List<Parser.Input>> inputList)
            {
                ControllerNum = controllerNum;
                InputList = inputList;
            }
        }

        /// <summary>
        /// The current number of running input sequences.
        /// </summary>
        public static int CurrentRunningInputs => RunningInputThreads;

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
        public static void CancelRunningInputs()
        {
            StopRunningInputs = true;
        }

        /// <summary>
        /// Allows new inputs to be processed.
        /// </summary>
        public static void ResumeRunningInputs()
        {
            StopRunningInputs = false;
        }

        /// <summary>
        /// Carries out a set of inputs.
        /// </summary>
        /// <param name="inputList">A list of lists of inputs to execute.</param>
        public static void CarryOutInput(List<List<Parser.Input>> inputList, in int controllerNum)
        {
            /*Kimimaru: We're using a thread pool for efficiency
             * Though very unlikely, there's a chance the input won't execute right away if it has to wait for a thread to be available
             * However, there are often plenty of available threads, so this shouldn't be an issue since we use only one thread per input string
             * Uncomment the following lines to see how many threads are supported in the pool on your machine */
            //ThreadPool.GetMinThreads(out int workermin, out int completionmin);
            //ThreadPool.GetMaxThreads(out int workerthreads, out int completionPortThreads);
            //Console.WriteLine($"Min workers: {workermin} Max workers: {workerthreads} Min async IO threads: {completionmin} Max async IO threads: {completionPortThreads}");

            InputWrapper inputWrapper = new InputWrapper(controllerNum, inputList);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteInput), inputWrapper);
        }

        private static void ExecuteInput(object obj)
        {
            //Increment running threads
            Interlocked.Increment(ref RunningInputThreads);

            //Get the input list and controller we're using - this should have been validated beforehand
            InputWrapper inputWrapper = (InputWrapper)obj;
            List<List<Parser.Input>> inputList = inputWrapper.InputList;
            VJoyController controller = VJoyController.GetController(inputWrapper.ControllerNum);

            Stopwatch sw = new Stopwatch();

            List<int> indices = new List<int>(16);
            int nonWaits = 0;

            for (int i = 0; i < inputList.Count; i++)
            {
                List<Parser.Input> inputs = inputList[i];

                indices.Clear();
                nonWaits = 0;

                //Press all buttons unless it's a release input
                for (int j = 0; j < inputs.Count; j++)
                {
                    indices.Add(j);
                    Parser.Input input = inputs[j];

                    //Don't do anything for a wait input
                    if (InputGlobals.CurrentConsole.IsWait(input) == true)
                    {
                        continue;
                    }

                    nonWaits++;

                    if (input.release == true)
                    {
                        controller.ReleaseInput(input);
                    }
                    else
                    {
                        controller.PressInput(input);
                    }
                }

                //Update the controller if there are non-wait inputs
                if (nonWaits > 0)
                {
                    controller.UpdateJoystickEfficient();
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
                        Parser.Input input = inputs[indices[j]];

                        if (sw.ElapsedMilliseconds < input.duration)
                        {
                            continue;
                        }

                        //Release if the input isn't a hold and isn't a wait input
                        if (input.hold == false && InputGlobals.CurrentConsole.IsWait(input) == false)
                        {
                            controller.ReleaseInput(input);

                            controller.UpdateJoystickEfficient();
                        }

                        indices.RemoveAt(j);
                    }
                }

                sw.Reset();
            }

            //End label to skip to if we should cancel early
            End:

            //At the end of it all, release every input
            for (int i = 0; i < inputList.Count; i++)
            {
                List<Parser.Input> inputs = inputList[i];
                for (int j = 0; j < inputs.Count; j++)
                {
                    controller.ReleaseInput(inputs[j]);
                }
            }

            controller.UpdateJoystickEfficient();

            //Decrement running threads
            Interlocked.Decrement(ref RunningInputThreads);
        }
    }
}
