using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace KimimaruBot
{
    /// <summary>
    /// Base console definition.
    /// </summary>
    public abstract class ConsoleBase
    {
        #region Properties

        /// <summary>
        /// The valid inputs for this console.
        /// </summary>
        public abstract string[] ValidInputs { get; protected set; }

        /// <summary>
        /// The input axes this console supports.
        /// </summary>
        public abstract Dictionary<string, HID_USAGES> InputAxes { get; protected set; }

        /// <summary>
        /// The button input map for this console.
        /// Each value corresponds to a numbered button on a vJoy controller.
        /// </summary>
        public abstract Dictionary<string, uint> ButtonInputMap { get; protected set; }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// A more efficient version of telling whether an input is an axis.
        /// Returns the axis if found to save a dictionary lookup if one is needed afterwards.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="axis">The axis value that is assigned. If no axis is found, the default value.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public abstract bool GetAxis(in Parser.Input input, out HID_USAGES axis);

        /// <summary>
        /// Tells whether an input is an axis or not.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an axis, otherwise false.</returns>
        public abstract bool IsAxis(in Parser.Input input);

        /// <summary>
        /// Tells whether the input is a button.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is a button, otherwise false.</returns>
        public abstract bool IsButton(in Parser.Input input);

        /// <summary>
        /// Tells whether the axis is an axis that ranges from a negative value to 0.
        /// This is used to tell the vJoy controller how to press the axis.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is a min axis, otherwise false.</returns>
        public abstract bool IsMinAxis(in Parser.Input input);

        /// <summary>
        /// Tells whether the input is an absolute axis - one that starts at 0 and goes up to a value.
        /// <para>This is usually true only for triggers, such as the GameCube's L and R buttons.</para>
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is an absolute axis, otherwise false.</returns>
        public abstract bool IsAbsoluteAxis(in Parser.Input input);

        #endregion

        #region Methods

        /// <summary>
        /// Tells whether the input is a wait input.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <returns>true if the input is one of the wait characters, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWait(in Parser.Input input) => (input.name == "#" || input.name == ".");

        #endregion
    }
}
