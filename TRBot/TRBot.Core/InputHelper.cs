using System;
using System.Collections.Generic;
using TRBot.Parsing;
using TRBot.Connection;
using TRBot.Consoles;
using TRBot.VirtualControllers;

namespace TRBot.Core
{
    public static class InputHelper
    {
        public static void PressInput(in Input input, ConsoleBase curConsole, IVirtualController vController)
        {
            if (curConsole.IsWait(input) == true)
            {
                return;
            }

            if (curConsole.GetAxis(input, out InputAxis axis) == true)
            {
                vController.PressAxis(axis.AxisVal, axis.MinAxisVal, axis.MaxAxisVal, input.percent);

                //Release a button with the same name (Ex. L/R buttons on GCN)
                if (curConsole.ButtonInputMap.TryGetValue(input.name, out InputButton btnVal) == true)
                {
                    vController.ReleaseButton(btnVal.ButtonVal);
                }
            }
            else if (curConsole.IsButton(input) == true)
            {
                vController.PressButton(curConsole.ButtonInputMap[input.name].ButtonVal);

                //Release an axis with the same name (Ex. L/R buttons on GCN)
                if (curConsole.InputAxes.TryGetValue(input.name, out InputAxis value) == true)
                {
                    vController.ReleaseAxis(value.AxisVal);
                }
            }

            vController.SetInputNamePressed(input.name);
        }

        public static void ReleaseInput(in Input input, ConsoleBase curConsole, IVirtualController vController)
        {
            if (curConsole.IsWait(input) == true)
            {
                return;
            }

            if (curConsole.GetAxis(input, out InputAxis axis) == true)
            {
                vController.ReleaseAxis(axis.AxisVal);

                //Release a button with the same name (Ex. L/R buttons on GCN)
                if (curConsole.ButtonInputMap.TryGetValue(input.name, out InputButton btnVal) == true)
                {
                    vController.ReleaseButton(btnVal.ButtonVal);
                }
            }
            else if (curConsole.IsButton(input) == true)
            {
                vController.ReleaseButton(curConsole.ButtonInputMap[input.name].ButtonVal);

                //Release an axis with the same name (Ex. L/R buttons on GCN)
                if (curConsole.InputAxes.TryGetValue(input.name, out InputAxis value) == true)
                {
                    vController.ReleaseAxis(value.AxisVal);
                }
            }

            vController.SetInputNameReleased(input.name);
        }
    }
}
