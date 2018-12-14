using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace BabyationApp.Behaviors
{
    public class USDateFormatterBehavior : Behavior<Entry>
    {
        private int _cursorPosition = 0;
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnTextChanged;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnTextChanged;

            base.OnDetachingFrom(bindable);
        }

        void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = (Entry)sender;

            entry.Text = FormatUSDate(entry.Text);
            entry.CursorPosition = _cursorPosition;
        }

        /// <summary>
        /// Formats the phone number as MM/DD/YYYY
        /// </summary>
        /// <returns>The phone number.</returns>
        /// <param name="input">Input.</param>
        private string FormatUSDate(string input)
        {
            var digitsRegex = new Regex(@"[^\d]");
            var digits = digitsRegex.Replace(input, "");

            if( 8 < digits.Length )
            {
                digits = digits.Substring(0, 8);
            }

            switch(digits.Length)
            {
                case 1: _cursorPosition = 1; return $"{digits}_/__/____";
                case 2: _cursorPosition = 2; return $"{digits}/__/____";
                case 3: _cursorPosition = 3; return $"{digits.Substring(0,2)}/{digits.Substring(2,1)}_/____";
                case 4: _cursorPosition = 4; return $"{digits.Substring(0,2)}/{digits.Substring(2,2)}/____";
                case 5: _cursorPosition = 5; return $"{digits.Substring(0,2)}/{digits.Substring(2,2)}/{digits.Substring(4, 1)}___";
                case 6: _cursorPosition = 6; return $"{digits.Substring(0,2)}/{digits.Substring(2,2)}/{digits.Substring(4, 2)}__";
                case 7: _cursorPosition = 7; return $"{digits.Substring(0,2)}/{digits.Substring(2,2)}/{digits.Substring(4, 3)}_";
                case 8: _cursorPosition = 8; return $"{digits.Substring(0,2)}/{digits.Substring(2,2)}/{digits.Substring(4, 4)}";
                default: _cursorPosition = 0; return $"__/__/____";
            }
        }
    }
}
