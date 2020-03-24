using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// The interface for all virtual controllers.
    /// </summary>
    public interface IVirtualController : IDisposable
    {
        /// <summary>
        /// The ID of the controller.
        /// </summary>
        uint ControllerID { get; }

        /// <summary>
        /// The index of the controller (Ex. port 1).
        /// </summary>
        int ControllerIndex { get; }

        /// <summary>
        /// Tells whether the controller device was successfully acquired.
        /// If this is false, don't use this controller instance to make inputs.
        /// </summary>
        bool IsAcquired { get; }

        /// <summary>
        /// Acquires the controller device.
        /// Make sure the device is acquired before initializing.
        /// </summary>
        void Acquire();

        /// <summary>
        /// Closes the controller device.
        /// Make sure to call this when the device is no longer needed.
        /// </summary>
        void Close();

        /// <summary>
        /// Sets up the controller after it has been acquired.
        /// </summary>
        void Init();

        /// <summary>
        /// Resets the controller to its defaults, including all button and axes presses.
        /// </summary>
        void Reset();

        #region Inputs

        void PressInput(in Parser.Input input);
        void ReleaseInput(in Parser.Input input);

        void PressAxis(in int axis, in bool min, in int percent);
        void ReleaseAxis(in int axis);

        void PressAbsoluteAxis(in int axis, in int percent);
        void ReleaseAbsoluteAxis(in int axis);

        void PressButton(in uint buttonVal);
        void ReleaseButton(in uint buttonVal);

        /// <summary>
        /// Updates the virtual device by applying all changes.
        /// </summary>
        void UpdateController();

        #endregion
    }
}
