using BabyationApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BabyationApp.Helpers {
    /// <summary>
    /// This static class is to have all the application wise validation logic
    /// </summary>
    public static class InputValidator {
        /// <summary>
        /// Validate an emal
        /// </summary>
        /// <param name="email">The email to validate to</param>
        /// <returns>Returns true if its a valid email; otherwise returns false</returns>
        public static bool IsValidEmail(string email) {
            if (String.IsNullOrEmpty(email)) return false;
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// Validates a password
        /// </summary>
        /// <param name="pass">Password to validate to</param>
        /// <returns>Returns true if its a valid password; otherwise returns false</returns>
        public static bool IsValidPassword(String pass) {
            //LIST of allowed Symbol: !"#$%&'()*+,-./:;<=>?@[\]^_`{|}~
            // The list is covered by Char.IsSymbol || Char.IsPunctuation
            return String.IsNullOrEmpty(pass) ? false : (pass.Length >= 8 &&
                   pass.ContainsChar(2) && pass.ContainsDigit(2) && pass.ContainsSymbol());
        }

        /// <summary>
        /// Generally checks if an input is valid or not
        /// </summary>
        /// <remarks>Invalid inputs are if the string is empty or have something set to --/--, --:--, --.--, --, ----</remarks>
        /// <param name="txt">The given input to check for validity</param>
        /// <returns>Returns true if valid; otherwise false</returns>
        public static bool IsValidInput(String txt) {
            if (String.IsNullOrEmpty(txt)) {
                return false;
            }

            if (txt == "--/--" || txt == "--:--" || txt == "--.--" || txt == "--" || txt == "----" || txt == "__/__/____" || txt == "__:__" || txt == "__._" || txt == AppResource.CommonPlaceholderDelimiter) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Do validation on input based on a length
        /// </summary>
        /// <param name="value">The input value to check for validity</param>
        /// <param name="maxDigits">Number of digits to check for validity</param>
        /// <returns></returns>
        public static string ValidateInput(String value, int maxDigits = 3) {
            if (!String.IsNullOrEmpty(value)) {
                if (value.Contains(".")) maxDigits = 4;
                if (value.Length > maxDigits) {
                    return value.Substring(0, maxDigits);
                }
            }
            return value;
        }

        /// <summary>
        /// Validates the birthday date for format MM/DD/YYYY
        /// </summary>
        /// <returns><c>true</c>, if birthday date was validated, <c>false</c> otherwise.</returns>
        /// <param name="value">Value.</param>
        public static bool ValidateBirthdayDate(String value) {
            if (String.IsNullOrEmpty(value)) return false;

            return Regex.IsMatch(value, @"([01][12])[/](0[1-9]|[12][0-9]|3[01])[/]([1-9][0-9]{3})$");
        }
    }
}
