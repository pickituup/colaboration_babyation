using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Helpers
{
    /// <summary>
    /// This is a stack class to put all the extension methods
    /// </summary>
    public static class ExtensionMethods
    { 
        static DateTime _defDttm = new DateTime(1970, 1, 1);


        /// <summary>
        /// Checks if DateTime is empty or not
        /// </summary>
        /// <param name="datetime">The DateTime object to which this method will be extented to</param>
        /// <returns>Returns true if the DateTime is empty; otherwise false</returns>
        public static bool IsEmpty(this DateTime datetime)
        {
            return datetime == new DateTime() || datetime == DefaultDateTime;
        }

        /// <summary>
        /// Gets the default DateTime
        /// </summary>
        public static DateTime DefaultDateTime {
            get
            {
                return _defDttm;
            }
        }

        /// <summary>
        /// Checks if the DateTime is in its default value
        /// </summary>
        /// <param name="datetime">The DateTime object to which this method will be extented to</param>
        /// <returns>Returns true if default; otherwise false</returns>
        public static bool IsDefaultDate(this DateTime datetime)
        {
            return datetime == _defDttm;
        }

        /// <summary>
        /// Constructs and returns the date part of this object to string
        /// </summary>
        /// <param name="datetime">The DateTime object to which this method will be extented to</param>
        /// <returns>The date part of this DateTime in String</returns>
        public static String ToDateString(this DateTime datetime, bool checkDefault = true)
        {
            if (checkDefault && (datetime.IsEmpty() || datetime == new DateTime() || datetime == DefaultDateTime || datetime.Date.Year <= 1970))
            {
                return "--/--";
            }
            else
            {
                return datetime.Date.ToString("MM/dd");
            }
        }

        /// <summary>
        /// Checks if the TimeSpan objecct is empty or not
        /// </summary>
        /// <param name="time">The TimeSpan object to which this method will be extented to</param>
        /// <returns>If empty, returns true,; otherwise false</returns>
        public static bool IsEmpty(this TimeSpan time)
        {
            return time == new TimeSpan();
        }

        /// <summary>
        /// Construts and returns the duration value of this TimeSpan
        /// </summary>
        /// <param name="time">The TimeSpan object to which this method will be extented to</param>
        /// <returns>The duration of this TimeSpan in String</returns>
        public static String ToDurationString(this TimeSpan time, bool checkDefault = true)
        {
            if ((checkDefault && time == new TimeSpan()) || time.Hours < 0 || time.Minutes < 0)
            {
                return "--:--";
            }
            else
            {
                return time.Hours.ToString("D2") + ":" + time.Minutes.ToString("D2");
            }
        }

        /// <summary>
        /// Constructs and returns the time part of this object to string
        /// </summary>
        /// <param name="time">The TimeSpan object to which this method will be extented to</param>
        /// <param name="checkDefault">Specity to check its default value too</param>
        /// <returns>The time part of this TimeSpan in string</returns>
        public static String ToTimeString(this TimeSpan time, bool checkDefault = true)
        {
            if (checkDefault && time == new TimeSpan())
            {
                return "--:--";
            }
            else
            {
                var tmpDt = new DateTime(2000, 1, 1).Add(time);
                return string.Format("{0:hh:mm tt}", tmpDt);
            }
        }

        /// <summary>
        /// Checks if this String contains at least a number of symbols
        /// </summary>
        /// <param name="str">The String object to which this method will be extented to</param>
        /// <param name="atleastCount">Number of symobols to check for</param>
        /// <returns>Returns true if the string contains at least atleastCount symbols; otherwise false</returns>
        public static bool ContainsSymbol(this String str, int atleastCount = 1)
        {
            return CharCountCheck(str, c => { return Char.IsSymbol(c) || Char.IsPunctuation(c); }, atleastCount);
        }

        /// <summary>
        /// Checks if this String contains at least a number of captial characters
        /// </summary>
        /// <param name="str">The String object to which this method will be extented to</param>
        /// <param name="atleastCount">Number of capital character to check for</param>
        /// <returns>Returns true if the string contains at least atleastCount capital characters; otherwise false</returns>
        public static bool ContainsUpper(this String str, int atleastCount = 1)
        {
            return CharCountCheck(str, c => { return Char.IsUpper(c); }, atleastCount);
        }

        /// <summary>
        /// Checks if this String contains at least a number of characters
        /// </summary>
        /// <param name="str">The String object to which this method will be extented to</param>
        /// <param name="atleastCount">Number of character to check for</param>
        /// <returns>Returns true if the string contains at least atleastCount characters; otherwise false</returns>
        public static bool ContainsChar(this String str, int atleastCount = 2)
        {
            return CharCountCheck(str, c => { return Char.IsLetter(c); }, atleastCount);
        }

        /// <summary>
        /// Checks if this String contains at least a number of digits
        /// </summary>
        /// <param name="str">The String object to which this method will be extented to</param>
        /// <param name="atleastCount">Number of digits to check for</param>
        /// <returns>Returns true if the string contains at least atleastCount digits; otherwise false</returns>
        public static bool ContainsDigit(this String str, int atleastCount = 2)
        {
            return CharCountCheck(str, c => { return Char.IsDigit(c); }, atleastCount);
        }

        /// <summary>
        /// Checks if this String contains at least a number of small characters
        /// </summary>
        /// <param name="str">The String object to which this method will be extented to</param>
        /// <param name="atleastCount">Number of small character to check for</param>
        /// <returns>Returns true if the string contains at least atleastCount small characters; otherwise false</returns>
        public static bool ContainsLower(this String str, int atleastCount = 1)
        {
            return CharCountCheck(str, c => { return Char.IsLower(c); }, atleastCount);
        }

        /// <summary>
        /// Checks if the string contains at least a number of given character checking functions satisfy
        /// </summary>
        /// <param name="str">The String object to which this method will be extented to</param>
        /// <param name="func">Func object to execute to check for a character</param>
        /// <param name="atleastCount">Number of small character to check for</param>
        /// <returns>Returns true if the string contains at least atleastCount func characters; otherwise false</returns>
        public static bool CharCountCheck(this String str, Func<Char, bool> func, int atleastCount = 1)
        {
            if (String.IsNullOrEmpty(str))
            {
                return false;
            }


            if (atleastCount <= 0)
            {
                return true;
            }

            int counter = 0;

            foreach (Char c in str)
            {
                if (func(c))
                {
                    counter++;

                    if (counter == atleastCount)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Finds a templated element
        /// </summary>
        /// <typeparam name="T">Generic type of which type the template to be searched for</typeparam>
        /// <param name="page">The Page object to which this method will be extented to</param>
        /// <param name="name">Name of the templated element to be searched for</param>
        /// <returns>If finds. returns the element of type T; otherwise returns null</returns>
        public static T FindTemplateElementByName<T>(this Page page, string name) where T : Element
        {
            var pc = page as IPageController;
            if (pc == null)
            {
                return null;
            }

            foreach (var child in pc.InternalChildren)
            {
                var result = child.FindByName<T>(name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Invalidates the size of this ContentPage
        /// </summary>
        /// <param name="page">The ContentPage object to which this method will be extented to</param>
        public static void InvalidateSize(this ContentPage page)
        {
            IEnumerable<MethodInfo> methods = page.GetType().GetTypeInfo().DeclaredMethods;
            MethodInfo method = methods.FirstOrDefault(m => m.Name == "InvalidateMeasure");

            if (method != null)
            {
                method.Invoke(page, null);
            }
            else
            {
                var layout = page.Content as Layout;
                if (layout != null)
                {
                    layout.ForceLayout();
                }
            }
        }

        /// <summary>
        /// Invalidates the size of this ContentView
        /// </summary>
        /// <param name="page">The ContentView object to which this method will be extented to</param>
        public static void InvalidateSize(this ContentView view)
        {
            var method = view.GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(m => m.Name == "InvalidateMeasure");

            if (method != null)
            {
                method.Invoke(view, null);
            }
            else
            {
                var layout = view.Content as Layout;
                if (layout != null)
                {
                    layout.ForceLayout();
                }
            }
        }

        /// <summary>
        /// Converts the color to hex string
        /// </summary>
        /// <param name="color">The Color to which this method will be extented to</param>
        /// <returns>The hex string of this color</returns>
        public static string ToHexString(this Xamarin.Forms.Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        public static int GetDecimalPart(this Double value, int pad)
        {
            return (int)(((decimal)value % 1) * (0 < pad ? pad : 10));
        }
    }
}
