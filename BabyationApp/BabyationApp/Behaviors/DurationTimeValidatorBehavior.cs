using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Linq;
using System.Globalization;

namespace BabyationApp.Behaviors
{
    /// <summary>
    /// A custom behavior for the Xamarin.Forms Entry control to 
    /// restrict the input to be numeric only in the form of a double or integer.
    /// </summary>
    public class DurationTimeValidatorBehavior : Behavior<Entry>
    {
        const string timeRegexp = @"^([0-1]?[0-9]|[0-5]([0-9]?))$";

        public static BindableProperty AllowDotDelimiterProperty = BindableProperty.Create("AllowDotDelimiter", typeof(bool), typeof(DurationTimeValidatorBehavior), true, BindingMode.OneWay);

        /// <summary>
        /// Bindable property to hold the boolean flag which decides whether we test for integer vs. double values.
        /// </summary>
        /// <value>The selected item.</value>
        public bool AllowDotDelimiter
        {
            get { return (bool)base.GetValue(AllowDotDelimiterProperty); }
            set { base.SetValue(AllowDotDelimiterProperty, value); }
        }

        public static readonly BindableProperty DelimiterSignProperty = BindableProperty.Create("DelimiterSign", typeof(char), typeof(DurationTimeValidatorBehavior), ':');

        public char DelimiterSign
        {
            get { return (char)GetValue(DelimiterSignProperty); }
            set { SetValue(DelimiterSignProperty, value); }
        }

        /// <summary>
        /// Called when this behavior is attached to a visual.
        /// </summary>
        /// <param name="bindable">Visual owner</param>
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        /// <summary>
        /// Called when this behavior is detached from a visual
        /// </summary>
        /// <param name="bindable">Visual owner</param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        /// <summary>
        /// Called when the associated Entry has new text.
        /// This changes the text color to reflect whether the data is valid.
        /// </summary>
        /// <param name="sender">Entry control</param>
        /// <param name="args">TextChanged event arguments</param>
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            string newValue = 0 < (args.NewTextValue?.Length ?? 0) ? args.NewTextValue : String.Empty;

            if (String.IsNullOrEmpty(newValue))
                return;

            if( newValue.Contains(".") )
            {
                newValue = newValue.Replace('.', DelimiterSign);
            }

            var parts = newValue.Split(DelimiterSign);
            string left = parts.Length > 0 ? parts[0] : String.Empty;
            string right = parts.Length > 1 ? parts[1] : String.Empty;

            double resultLeft = -1;
            double resultRight = -1;

            bool leftValid = String.IsNullOrEmpty(left) || double.TryParse(left, out resultLeft);
            bool rightValid = String.IsNullOrEmpty(right) || double.TryParse(right, out resultRight);

            Entry entry = (Entry)sender;
            string text = entry.Text;
            string oldText = 0 < (args.OldTextValue?.Length ?? 0) ? args.OldTextValue : String.Empty;

            var oldParts = oldText.Split(DelimiterSign);
            string oldLeft = oldParts.Length > 0 ? oldParts[0] : String.Empty;
            string oldRight = oldParts.Length > 1 ? oldParts[1] : String.Empty;

            if (!leftValid)
            {
                left = oldLeft;
            }

            if (2 < left.Length )
            {
                left = oldLeft;
            }

            if (!rightValid)
            {
                right = oldRight;
            }

            if (2 < right.Length )
            {
                right = oldRight;
            }

            string result = null;
            if (0 < left.Length)
            {
                if (!Regex.IsMatch(left, timeRegexp))
                {
                    left = oldLeft;
                }

                result = left;
            }

            if (1 < parts.Length)
            {
                result += DelimiterSign;
            }

            if (0 < right.Length)
            {
                if (!Regex.IsMatch(right, timeRegexp))
                {
                    right = oldRight;
                }

                result += right;
            }

            entry.Text = result;
        }
    }

    public class DurationTimeNoLeadingZerosValidatorBehavior : Behavior<Entry>
    {
        const string timeRegexp = @"^([0-1 ]?[0-9]|[0-5]([0-9]?))$";

        public static BindableProperty AllowDotDelimiterProperty = BindableProperty.Create("AllowDotDelimiter", typeof(bool), typeof(DurationTimeValidatorBehavior), true, BindingMode.OneWay);

        /// <summary>
        /// Bindable property to hold the boolean flag which decides whether we test for integer vs. double values.
        /// </summary>
        /// <value>The selected item.</value>
        public bool AllowDotDelimiter
        {
            get { return (bool)base.GetValue(AllowDotDelimiterProperty); }
            set { base.SetValue(AllowDotDelimiterProperty, value); }
        }

        public static readonly BindableProperty DelimiterSignProperty = BindableProperty.Create("DelimiterSign", typeof(char), typeof(DurationTimeValidatorBehavior), ':');

        public char DelimiterSign
        {
            get { return (char)GetValue(DelimiterSignProperty); }
            set { SetValue(DelimiterSignProperty, value); }
        }

        /// <summary>
        /// Called when this behavior is attached to a visual.
        /// </summary>
        /// <param name="bindable">Visual owner</param>
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        /// <summary>
        /// Called when this behavior is detached from a visual
        /// </summary>
        /// <param name="bindable">Visual owner</param>
        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        /// <summary>
        /// Called when the associated Entry has new text.
        /// This changes the text color to reflect whether the data is valid.
        /// </summary>
        /// <param name="sender">Entry control</param>
        /// <param name="args">TextChanged event arguments</param>
        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            string newValue = 0 < (args.NewTextValue?.Length ?? 0) ? args.NewTextValue : String.Empty;

            if (String.IsNullOrEmpty(newValue))
                return;

            if (newValue.Contains("."))
            {
                newValue = newValue.Replace('.', DelimiterSign);
            }

            var parts = newValue.Split(DelimiterSign);
            string left = parts.Length > 0 ? parts[0] : String.Empty;
            string right = parts.Length > 1 ? parts[1] : String.Empty;

            double resultLeft = -1;
            double resultRight = -1;

            bool leftValid = String.IsNullOrEmpty(left) || double.TryParse(left, out resultLeft);
            bool rightValid = String.IsNullOrEmpty(right) || double.TryParse(right, out resultRight);

            Entry entry = (Entry)sender;
            string text = entry.Text;
            string oldText = 0 < (args.OldTextValue?.Length ?? 0) ? args.OldTextValue : String.Empty;

            var oldParts = oldText.Split(DelimiterSign);
            string oldLeft = oldParts.Length > 0 ? oldParts[0] : String.Empty;
            string oldRight = oldParts.Length > 1 ? oldParts[1] : String.Empty;

            if (!leftValid)
            {
                left = oldLeft;
            }

            if (2 < left.Length)
            {
                left = oldLeft;
            }

            if (!rightValid)
            {
                right = oldRight;
            }

            if (2 < right.Length)
            {
                right = oldRight;
            }

            string result = null;
            if (0 < left.Length)
            {
                if (!Regex.IsMatch(left, timeRegexp))
                {
                    left = oldLeft;
                }

                result = left;
            }

            if (1 < parts.Length)
            {
                result += DelimiterSign;
            }

            if (0 < right.Length)
            {
                if (!Regex.IsMatch(right, timeRegexp))
                {
                    right = oldRight;
                }

                result += right;
            }

            if (!string.IsNullOrEmpty(result) && result.Length >= 2 && result.First() == '0')
            {
                result = string.Format(" {0}", result.Substring(1));
            }


            entry.Text = result;
        }
    }
}