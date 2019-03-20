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
        public static void CarryOutInput(List<List<Parser.Input>> inputList)
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                for (int j = 0; j < inputList[i].Count; j++)
                {
                    Thread thread = new Thread(InefficientInputs);
                    thread.Start(inputList[i][j]);
                }
            }
        }

        private static void InputThread(object obj)
        {
            List<List<Parser.Input>> inputList = (List<List<Parser.Input>>)obj;

            for (int i = 0; i < inputList.Count; i++)
            {
                List<Parser.Input> inputs = inputList[i];
                
            }

            Stopwatch sw = Stopwatch.StartNew();
        }

        private static void InefficientInputs(object obj)
        {
            Parser.Input input = (Parser.Input)obj;

            int dur = input.duration;

            if (InputGlobals.IsAxis(input) == true)
            {
                VJoyController.Joystick.PressAxis(InputGlobals.InputAxes[input.name], InputGlobals.IsMinAxis(input.name), input.percent);
            }
            else if (InputGlobals.IsAbsoluteAxis(input) == true)
            {
                VJoyController.Joystick.PressAbsoluteAxis(InputGlobals.InputAxes[input.name], input.percent);
            }
            else if (InputGlobals.IsButton(input) == true)
            {
                VJoyController.Joystick.PressButton(input.name);
            }

            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < dur)
            {

            }

            sw.Stop();

            if (InputGlobals.IsAxis(input) == true)
            {
                VJoyController.Joystick.ReleaseAxis(InputGlobals.InputAxes[input.name]);
            }
            else if (InputGlobals.IsAbsoluteAxis(input) == true)
            {
                VJoyController.Joystick.ReleaseAbsoluteAxis(InputGlobals.InputAxes[input.name]);
            }
            else if (InputGlobals.IsButton(input) == true)
            {
                VJoyController.Joystick.ReleaseButton(input.name);
            }
        }
    }
}
