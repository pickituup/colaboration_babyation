using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BabyationApp.Controls.Buttons;
using BabyationApp.Controls.Views;
using BabyationApp.Helpers;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace BabyationApp.Controls.Pickers
{
    public partial class MaskedTimePicker : ContentView
    {
        private TimePickerEx _timePicker;
        private ButtonEx _button;

        private Regex _placeholdeRegex = new Regex("\\w");

        public event EventHandler OkClicked;
        public MaskedTimePicker()
        {
            InitializeComponent();

            _timePicker = Helpers.VisualTreeHelper.GetTemplateChild<TimePickerEx>(this, "_timePicker");
            _button = Helpers.VisualTreeHelper.GetTemplateChild<ButtonEx>(this, "_btnDtPicker");

            if (_timePicker != null)
            {
                _timePicker.OKClicked += (s, e) => { FireOkClicked(e); };
            }

            HandleButtonClick = new Command(() =>
            {
                if (IsEnabled)
                {
                    _timePicker?.Focus();
                }
            });
        }

        public Func<TimeSpan, bool> ValidationFunc { get; set; }
        public Action AfterAction { get; set; }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create("HeaderText", typeof(string), typeof(MaskedTimePicker), string.Empty, BindingMode.OneWay);

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly BindableProperty HeaderStyleProperty = BindableProperty.Create("HeaderStyle", typeof(Style), typeof(MaskedTimePicker));

        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public static readonly BindableProperty BottomTextProperty = BindableProperty.Create("BottomText", typeof(string), typeof(MaskedTimePicker), string.Empty, BindingMode.OneWay);

        public string BottomText
        {
            get { return (string)GetValue(BottomTextProperty); }
            set { SetValue(BottomTextProperty, value); }
        }

        public static readonly BindableProperty BottomStyleProperty = BindableProperty.Create("BottomStyle", typeof(Style), typeof(MaskedTimePicker));

        public Style BottomStyle
        {
            get { return (Style)GetValue(BottomStyleProperty); }
            set { SetValue(BottomStyleProperty, value); }
        }

        public static readonly BindableProperty TimeFormatProperty = BindableProperty.Create("TimeFormat", typeof(string), typeof(MaskedTimePicker), "t", BindingMode.OneWay, propertyChanged: OnFormatChanged);

        public string TimeFormat
        {
            get { return (string)GetValue(TimeFormatProperty); }
            set { SetValue(TimeFormatProperty, value); }
        }

        public static readonly BindableProperty CalendarTimeProperty = BindableProperty.Create("CalendarTime", typeof(TimeSpan), typeof(MaskedTimePicker), new TimeSpan(0L), BindingMode.TwoWay);

        public TimeSpan CalendarTime
        {
            get { return (TimeSpan)GetValue(CalendarTimeProperty); }
            set { SetValue(CalendarTimeProperty, value); }
        }

        public static readonly BindableProperty TimeProperty = BindableProperty.Create("Time", typeof(TimeSpan?), typeof(MaskedTimePicker), (DateTime?)null, BindingMode.TwoWay, propertyChanged: OnDateChanged);

        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public TimeSpan? Time
        {
            get { return (TimeSpan?)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly BindableProperty ValueTextProperty = BindableProperty.Create("ValueText", typeof(string), typeof(MaskedTimePicker), string.Empty, BindingMode.TwoWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }

        public static readonly BindableProperty PlaceHolderTextProperty = BindableProperty.Create("PlaceHolderText", typeof(string), typeof(MaskedTimePicker), string.Empty, BindingMode.TwoWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }

        public static readonly BindableProperty ValueStyleProperty = BindableProperty.Create("ValueStyle", typeof(Style), typeof(MaskedTimePicker));
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public Style ValueStyle
        {
            get { return (Style)GetValue(ValueStyleProperty); }
            set { SetValue(ValueStyleProperty, value); }
        }

        public static readonly BindableProperty ValuePlaceholderStyleProperty = BindableProperty.Create("ValuePlaceholderStyle", typeof(Style), typeof(MaskedTimePicker));
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public Style ValuePlaceholderStyle
        {
            get { return (Style)GetValue(ValuePlaceholderStyleProperty); }
            set { SetValue(ValuePlaceholderStyleProperty, value); }
        }

        public static readonly BindableProperty HandleButtonClickProperty = BindableProperty.Create("HandleButtonClick", typeof(Command), typeof(MaskedTimePicker));
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public Command HandleButtonClick
        {
            get { return (Command)GetValue(HandleButtonClickProperty); }
            private set { SetValue(HandleButtonClickProperty, value); }
        }

        protected void FireOkClicked(EventArgs e)
        {
            if (ValidationFunc == null || ValidationFunc.Invoke(CalendarTime))
            {
                Time = CalendarTime;
                AfterAction?.Invoke();
            }
        }

        //public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(ImageButton), Color.Black, propertyChanged: OnTextColorChanged);
        ///// <summary>
        ///// Normal state text color for the button
        ///// </summary>
        //public Color TextColor
        //{
        //    get { return (Color)GetValue(TextColorProperty); }
        //    set { SetValue(TextColorProperty, value);}
        //}

        static void OnFormatChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as MaskedTimePicker;
            if (self != null)
            {
                self.PlaceHolderText = self._placeholdeRegex.Replace(self.TimeFormat, "_");
            }
        }

        static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as MaskedTimePicker;
            if (self != null && newValue is TimeSpan timeSpan)
            {
                self.ValueText = new DateTime(timeSpan.Ticks).ToString(self.TimeFormat);
            }
            //var self = bindable as MaskedTimePicker;
            //if (self != null)
            //{
            //    self.ValueText = new DateTime(self.CalendarTime.Ticks).ToString(self.TimeFormat);
            //}
        }
    }
}
