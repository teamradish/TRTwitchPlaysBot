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

        //private static void InputThread(object obj)
        //{
        //    List<List<Parser.Input>> inputList = (List<List<Parser.Input>>)obj;
        //    Stopwatch sw = Stopwatch.StartNew();
        //
        //    for (int i = 0; i < inputList.Count; i++)
        //    {
        //        List<Parser.Input> inputs = inputList[i];
        //        
        //    }
        //}

        private static void InefficientInputs(object obj)
        {
            Parser.Input input = (Parser.Input)obj;

            int dur = input.duration;
            if (input.duration_type == "s") dur *= 1000;

            HID_USAGES? axis = null;

            if (input.name == "left")
            {
                axis = HID_USAGES.HID_USAGE_X;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_X, true, input.percent);
            }
            else if (input.name == "right")
            {
                axis = HID_USAGES.HID_USAGE_X;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_X, false, input.percent);
            }
            else if (input.name == "up")
            {
                axis = HID_USAGES.HID_USAGE_Y;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_Y, true, input.percent);
            }
            else if (input.name == "down")
            {
                axis = HID_USAGES.HID_USAGE_Y;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_Y, false, input.percent);
            }
            else if (input.name == "cleft")
            {
                axis = HID_USAGES.HID_USAGE_RX;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_RX, true, input.percent);
            }
            else if (input.name == "cright")
            {
                axis = HID_USAGES.HID_USAGE_RX;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_RX, false, input.percent);
            }
            else if (input.name == "cup")
            {
                axis = HID_USAGES.HID_USAGE_RY;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_RY, true, input.percent);
            }
            else if (input.name == "cdown")
            {
                axis = HID_USAGES.HID_USAGE_RY;
                VJoyController.Joystick.PressAxis(HID_USAGES.HID_USAGE_RY, false, input.percent);
            }

            if (axis == null && (input.name != "#" && input.name != "."))
                VJoyController.Joystick.PressButton(input.name);

            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < dur)
            {

            }

            sw.Stop();

            if (axis == null)
                VJoyController.Joystick.ReleaseButton(input.name);
            else if (input.name != "#" && input.name != ".")
            {
                long min = 0L;
                long max = 0L;
                VJoyController.VJoyInstance.GetVJDAxisMax(VJoyController.Joystick.ControllerID, axis.Value, ref max);
                VJoyController.VJoyInstance.GetVJDAxisMin(VJoyController.Joystick.ControllerID, axis.Value, ref min);

                VJoyController.VJoyInstance.SetAxis((int)((max - min) / 2), VJoyController.Joystick.ControllerID, axis.Value);
            }
        }
    }
}
