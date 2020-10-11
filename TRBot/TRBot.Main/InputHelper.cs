using System;
using System.Collections.Generic;
using TRBot.Parsing;
using TRBot.Consoles;
using TRBot.VirtualControllers;

namespace TRBot.Core
{
    public static class InputHelper
    {
        public static void PressInput(in ParsedInput input, GameConsole curConsole, IVirtualController vController)
        {
            if (curConsole.IsBlankInput(input) == true)
            {
                return;
            }

            if (curConsole.GetAxis(input, out InputAxis axis) == true)
            {
                vController.PressAxis(axis.AxisVal, axis.MinAxisVal, axis.MaxAxisVal, input.percent);

                //Release a button with the same name (Ex. L/R buttons on GCN)
                if (curConsole.GetButtonValue(input.name, out InputButton btnVal) == true)
                {
                    vController.ReleaseButton(btnVal.ButtonVal);
                }
            }
            else if (curConsole.GetButtonValue(input.name, out InputButton btnVal) == true)
            {
                vController.PressButton(btnVal.ButtonVal);

                //Release an axis with the same name (Ex. L/R buttons on GCN)
                if (curConsole.GetAxisValue(input.name, out InputAxis value) == true)
                {
                    vController.ReleaseAxis(value.AxisVal);
                }
            }

            vController.SetInputNamePressed(input.name);
        }

        public static void ReleaseInput(in ParsedInput input, GameConsole curConsole, IVirtualController vController)
        {
            if (curConsole.IsBlankInput(input) == true)
            {
                return;
            }

            if (curConsole.GetAxis(input, out InputAxis axis) == true)
            {
                vController.ReleaseAxis(axis.AxisVal);

                //Release a button with the same name (Ex. L/R buttons on GCN)
                if (curConsole.GetButtonValue(input.name, out InputButton btnVal) == true)
                {
                    vController.ReleaseButton(btnVal.ButtonVal);
                }
            }
            else if (curConsole.GetButtonValue(input.name, out InputButton btnVal) == true)
            {
                vController.ReleaseButton(btnVal.ButtonVal);

                //Release an axis with the same name (Ex. L/R buttons on GCN)
                if (curConsole.GetAxisValue(input.name, out InputAxis value) == true)
                {
                    vController.ReleaseAxis(value.AxisVal);
                }
            }

            vController.SetInputNameReleased(input.name);
        }
    }
}
