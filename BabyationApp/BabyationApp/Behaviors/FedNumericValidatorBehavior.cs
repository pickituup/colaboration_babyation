using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using System.Linq;

namespace BabyationApp.Behaviors
{
    /// <summary>
    /// A custom behavior for the Xamarin.Forms Entry control to 
    /// restrict the input to be numeric only in the form of a double or integer.
    /// </summary>
    public class FedNumericValidatorBehavior : Behavior<Entry>
    {
        /// <summary>
        /// Backing storage for the boolean flag which decides between integer vs. double validation.
        /// </summary>
        public static BindableProperty AllowDecimalProperty = BindableProperty.Create("AllowDecimal", typeof(bool), typeof(FedNumericValidatorBehavior), true, BindingMode.OneWay);

        /// <summary>
        /// Bindable property to hold the boolean flag which decides whether we test for integer vs. double values.
        /// </summary>
        /// <value>The selected item.</value>
        public bool AllowDecimal 
        {
            get { return (bool)base.GetValue (AllowDecimalProperty); }
            set { base.SetValue (AllowDecimalProperty, value); }
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        
        public static readonly BindableProperty MaxDecimalLengthProperty = BindableProperty.Create("MaxDecimalLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);

        public int MaxDecimalLength
        {
            get { return (int)GetValue(MaxDecimalLengthProperty); }
            set { SetValue(MaxDecimalLengthProperty, value); }
        }

        public static readonly BindableProperty DelimiterSignProperty = BindableProperty.Create("DelimiterSign", typeof(char), typeof(MaxLengthValidatorBehavior), '.');

        public char DelimiterSign
        {
            get { return (char)GetValue(DelimiterSignProperty); }
            set { SetValue(DelimiterSignProperty, value); }
        }

        #region InvalidColorProperty
        /// <summary>
        /// Backing storage for the color used when the Entry has invalid data (non-numeric).
        /// </summary>
        public static BindableProperty InvalidColorProperty = BindableProperty.Create("InvalidColor", typeof(Color), typeof(FedNumericValidatorBehavior), Color.Red, BindingMode.OneWay);

        /// <summary>
        /// Bindable property to hold the color used when the Entry has invalid data (non-numeric).
        /// </summary>
        /// <value>The selected item.</value>
        public Color InvalidColor 
        {
            get { return (Color) base.GetValue (InvalidColorProperty); }
            set { base.SetValue (InvalidColorProperty, value); }
        }
        #endregion

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
            if( AllowDecimal )
            {
                ProcessDouble(sender, args);
            }
            else
            {
                ProcessLong(sender, args);
            }
        }

        private void ProcessLong(object sender, TextChangedEventArgs args)
        {
            string newValue = args.NewTextValue;

            bool isValid = long.TryParse(newValue, out long result);

            Entry entry = (Entry)sender;
            string text = entry.Text;
            string oldText = 0 < (args.OldTextValue?.Length ?? 0) ? args.OldTextValue : String.Empty;

            if( !isValid )
            {
                entry.Text = oldText;
                return;
            }

            if( 0 < MaxLength && text.Length > MaxLength)
            {
                entry.Text = oldText;
            }
        }

        private void ProcessDouble(object sender, TextChangedEventArgs args)
        {
            string newValue = 0 < (args.NewTextValue?.Length ?? 0) ? args.NewTextValue : String.Empty;

            if (String.IsNullOrEmpty(newValue))
                return;
                
            var parts = newValue.Split(DelimiterSign);
            string left = parts.Length > 0 ? parts[0] : String.Empty;
            string right = parts.Length > 1 ? parts[1] : String.Empty;

            bool leftValid = String.IsNullOrEmpty(left) || double.TryParse(left, out double resultLeft);
            bool rightValid = String.IsNullOrEmpty(right) || double.TryParse(right, out double resultRight);

            Entry entry = (Entry)sender;
            string text = entry.Text;
            string oldText = 0 < (args.OldTextValue?.Length ?? 0) ? args.OldTextValue : String.Empty;

            var oldParts = oldText.Split(DelimiterSign);
            string oldLeft = oldParts.Length > 0 ? oldParts[0] : String.Empty;
            string oldRight = oldParts.Length > 1 ? oldParts[1] : String.Empty;

            if( !leftValid )
            {
                left = oldLeft;
            }

            if( !rightValid )
            {
                right = oldRight;
            }

            if( 0 < MaxLength && left.Length > MaxLength )
            {
                left = oldLeft;
            }

            if( 0 < MaxDecimalLength && right.Length > MaxDecimalLength)
            {
                right = oldRight;
            }

            string result = null;
            if( 0 < left.Length )
            {
                result = left;
            }

            if( 2 == parts.Length )
            {
                result += DelimiterSign;
            }

            if( 0 < right.Length )
            {
                result += right;
            }

            entry.Text = result;
        }
    }
}