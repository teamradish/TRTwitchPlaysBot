using System;
using System.Collections.Generic;
using System.Text;

namespace TRBot
{
    /// <summary>
    /// Post-processes inputs from the parser.
    /// </summary>
    public static class ParserPostProcess
    {
        /// <summary>
        /// Validates permission for a user to perform a certain input.
        /// </summary>
        /// <param name="userLevel">The level of the user.</param>
        /// <param name="inputName">The input to check.</param>
        /// <param name="inputAccessLevels">The dictionary of access levels for inputs.</param>
        /// <returns>An InputValidation object specifying if the input is valid and a message, if any.</returns>
        public static InputValidation CheckInputPermissions(in int userLevel, in string inputName, Dictionary<string,int> inputAccessLevels)
        {
            if (inputAccessLevels.TryGetValue(inputName, out int accessLvl) == true)
            {
                if (userLevel < accessLvl)
                {
                    return new InputValidation(false, $"No permission to use input \"{inputName}\", which requires {(AccessLevels.Levels)accessLvl} access.");
                }
            }

            return new InputValidation(true, string.Empty);
        }

        /// <summary>
        /// Validates permission for a user to perform certain inputs.
        /// </summary>
        /// <param name="userLevel">The level of the user.</param>
        /// <param name="inputs">The inputs to check.</param>
        /// <param name="inputAccessLevels">The dictionary of access levels for inputs.</param>
        /// <returns>An InputValidation object specifying if the input is valid and a message, if any.</returns>
        public static InputValidation CheckInputPermissions(in int userLevel, List<List<Parser.Input>> inputs, Dictionary<string, int> inputAccessLevels)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                for (int j = 0; j < inputs[i].Count; j++)
                {
                    Parser.Input input = inputs[i][j];

                    if (inputAccessLevels.TryGetValue(input.name, out int accessLvl) == false)
                    {
                        continue;
                    }

                    if (userLevel < accessLvl)
                    {
                        return new InputValidation(false, $"No permission to use input \"{input.name}\", which requires at least {(AccessLevels.Levels)accessLvl} access.");
                    }
                }
            }

            return new InputValidation(true, string.Empty);
        }

        public struct InputValidation
        {
            public bool IsValid;
            public string Message;

            public InputValidation(in bool isValid, string message)
            {
                IsValid = isValid;
                Message = message;
            }
        }
    }
}
