using Xamarin.Forms;

namespace BabyationApp.Behaviors
{
    public class MaxLengthValidatorBehavior : Behavior<Entry>
    {
        #region MaxLenghValidationProperty
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidatorBehavior), 0);

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        #endregion

        #region InvalidColorProperty
        public static BindableProperty InvalidColorProperty = BindableProperty.Create("InvalidColor", typeof(Color), typeof(MaxLengthValidatorBehavior), Color.Red, BindingMode.OneWay);

        public Color InvalidColor
        {
            get { return (Color)base.GetValue(InvalidColorProperty); }
            set { base.SetValue(InvalidColorProperty, value); }
        }
        #endregion

        #region MessageProperty
        public static readonly BindableProperty MessageProperty = BindableProperty.Create("Message", typeof(string), typeof(MaxLengthValidatorBehavior));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        #endregion

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if( null != sender && null != args && null != args.NewTextValue)
            {
                if (args.NewTextValue.Length > 0 && args.NewTextValue.Length > MaxLength)
                {
                    ((Entry)sender).Text = args.NewTextValue.Substring(0, MaxLength);
                }
            }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);

        }
    }
}
