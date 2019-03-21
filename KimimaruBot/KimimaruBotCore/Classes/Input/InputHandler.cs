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
        public static void CarryOutInput(List<List<Parser.Input>> inputList)
        {
            /*Kimimaru: We're using a thread pool for efficiency
             * Though very unlikely, there's a chance the input won't execute right away if it has to wait for a thread to be available
             * However, there are often plenty of available threads, so this shouldn't be an issue since we use only one thread per input string
             * Uncomment the following lines to see how many threads are supported in the pool on your machine */
            //ThreadPool.GetMinThreads(out int workermin, out int completionmin);
            //ThreadPool.GetMaxThreads(out int workerthreads, out int completionPortThreads);
            //Console.WriteLine($"Min workers: {workermin} Max workers: {workerthreads} Min async IO threads: {completionmin} Max async IO threads: {completionPortThreads}");

            ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteInput), inputList);
        }

        private static void ExecuteInput(object obj)
        {
            //Increment running threads
            Interlocked.Increment(ref RunningInputThreads);

            List<List<Parser.Input>> inputList = (List<List<Parser.Input>>)obj;

            Stopwatch sw = new Stopwatch();

            List<int> indices = new List<int>(16);

            for (int i = 0; i < inputList.Count; i++)
            {
                List<Parser.Input> inputs = inputList[i];

                indices.Clear();

                //Press all buttons unless it's a release input
                for (int j = 0; j < inputs.Count; j++)
                {
                    indices.Add(j);
                    Parser.Input input = inputs[j];

                    if (input.release == true)
                    {
                        VJoyController.Joystick.ReleaseInput(input);
                    }
                    else
                    {
                        VJoyController.Joystick.PressInput(input);
                    }
                }

                VJoyController.Joystick.UpdateJoystickEfficient();

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

                        if (sw.ElapsedMilliseconds < input.duration) continue;

                        if (input.hold == false)
                        {
                            VJoyController.Joystick.ReleaseInput(input);

                            VJoyController.Joystick.UpdateJoystickEfficient();
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
                    VJoyController.Joystick.ReleaseInput(inputs[j]);
                }
            }

            VJoyController.Joystick.UpdateJoystickEfficient();

            //Decrement running threads
            Interlocked.Decrement(ref RunningInputThreads);
        }
    }
}
