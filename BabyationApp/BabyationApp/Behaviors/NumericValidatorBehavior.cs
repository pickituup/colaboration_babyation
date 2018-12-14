using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace BabyationApp.Behaviors
{
    /// <summary>
    /// A custom behavior for the Xamarin.Forms Entry control to 
    /// restrict the input to be numeric only in the form of a double or integer.
    /// </summary>
    public class NumericValidatorBehavior : Behavior<Entry>
    {
        #region AllowDecimalProperty
        /// <summary>
        /// Backing storage for the boolean flag which decides between integer vs. double validation.
        /// </summary>
        public static BindableProperty AllowDecimalProperty = BindableProperty.Create("AllowDecimal", typeof(bool), typeof(NumericValidatorBehavior), true, BindingMode.OneWay);

        /// <summary>
        /// Bindable property to hold the boolean flag which decides whether we test for integer vs. double values.
        /// </summary>
        /// <value>The selected item.</value>
        public bool AllowDecimal 
        {
            get { return (bool)base.GetValue (AllowDecimalProperty); }
            set { base.SetValue (AllowDecimalProperty, value); }
        }
        #endregion

        #region MaxLenghValidationProperty
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        #endregion


        #region InvalidColorProperty
        /// <summary>
        /// Backing storage for the color used when the Entry has invalid data (non-numeric).
        /// </summary>
        public static BindableProperty InvalidColorProperty = BindableProperty.Create("InvalidColor", typeof(Color), typeof(NumericValidatorBehavior), Color.Red, BindingMode.OneWay);

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
            bool isValid = false;
            if (AllowDecimal)
            {
                double result;
                isValid = double.TryParse(args.NewTextValue, out result);
            }
            else
            {
                long result;
                isValid = long.TryParse(args.NewTextValue, out result);
            }

            string text = ((Entry)sender).Text;

            if (!isValid && !string.IsNullOrEmpty(text))
            {
                ((Entry)sender).Text = text.Remove(text.Length - 1);
            }

            if (0 < MaxLength)
            {
                int maxL = (text.Contains(".") ? MaxLength + 1 : MaxLength);
                if (text.Length > maxL)
                {
                    ((Entry)sender).Text = text.Remove(text.Length - 1);
                }
            }
        }
    }}

