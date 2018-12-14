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
   public partial class MaskedDatePicker : ContentView
    {   
        private DatePickerEx _datePicker;
        private ButtonEx _button;

        private Regex _placeholdeRegex = new Regex("\\w");

        public event EventHandler OkClicked;
        public MaskedDatePicker()
        {
            InitializeComponent();

            _datePicker = Helpers.VisualTreeHelper.GetTemplateChild<DatePickerEx>(this, "_dtPicker");
            _button = Helpers.VisualTreeHelper.GetTemplateChild<ButtonEx>(this, "_btnDtPicker");

            if (_datePicker != null)
            {
                _datePicker.OKClicked += (s, e) => { FireOkClicked(e); };
            }

            HandleButtonClick = new Command(() => _datePicker?.Focus());
        }

        public Func<DateTime, bool> ValidationFunc { get; set; }
        public Action AfterAction { get; set; }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create("HeaderText", typeof(string), typeof(MaskedDatePicker), string.Empty, BindingMode.OneWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }
        
        public static readonly BindableProperty HeaderStyleProperty = BindableProperty.Create("HeaderStyle", typeof(Style), typeof(MaskedDatePicker));
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }
        
        public static readonly BindableProperty DateFormatProperty = BindableProperty.Create("DateFormat", typeof(string), typeof(MaskedDatePicker), "d", BindingMode.OneWay, propertyChanged: OnFormatChanged);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public string DateFormat
        {
            get { return (string)GetValue(DateFormatProperty); }
            set { SetValue(DateFormatProperty, value); }
        }
        
        public static readonly BindableProperty CalendarDateProperty = BindableProperty.Create("CalendarDate", typeof(DateTime), typeof(MaskedDatePicker), new DateTime(), BindingMode.TwoWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public DateTime CalendarDate
        {
            get { return (DateTime)GetValue(CalendarDateProperty); }
            set { SetValue(CalendarDateProperty, value); }
        }
        
        public static readonly BindableProperty DateProperty = BindableProperty.Create("Date", typeof(DateTime?), typeof(MaskedDatePicker), (DateTime?)null, BindingMode.TwoWay, propertyChanged: OnDateChanged);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public DateTime? Date
        {
            get { return (DateTime?)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        
        public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create("MinimumDate", typeof(DateTime), typeof(MaskedDatePicker), new DateTime(1900, 1, 1), BindingMode.OneWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public DateTime MinimumDate
        {
            get { return (DateTime)GetValue(MinimumDateProperty); }
            set { SetValue(MinimumDateProperty, value); }
        }
        
        public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create("MaximumDate", typeof(DateTime), typeof(MaskedDatePicker), new DateTime(2100, 12, 31), BindingMode.OneWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public DateTime MaximumDate
        {
            get { return (DateTime)GetValue(MaximumDateProperty); }
            set { SetValue(MaximumDateProperty, value); }
        }
        
        public static readonly BindableProperty ValueTextProperty = BindableProperty.Create("ValueText", typeof(string), typeof(MaskedDatePicker), string.Empty, BindingMode.TwoWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }
        
        public static readonly BindableProperty PlaceHolderTextProperty = BindableProperty.Create("PlaceHolderText", typeof(string), typeof(MaskedDatePicker), string.Empty, BindingMode.TwoWay);
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }
        
        public static readonly BindableProperty ValueStyleProperty = BindableProperty.Create("ValueStyle", typeof(Style), typeof(MaskedDatePicker));
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public Style ValueStyle
        {
            get { return (Style)GetValue(ValueStyleProperty); }
            set { SetValue(ValueStyleProperty, value); }
        }
        
        public static readonly BindableProperty ValuePlaceholderStyleProperty = BindableProperty.Create("ValuePlaceholderStyle", typeof(Style), typeof(MaskedDatePicker));
        /// <summary>
        /// Horizontal layout options for the image and text
        /// </summary>
        public Style ValuePlaceholderStyle
        {
            get { return (Style)GetValue(ValuePlaceholderStyleProperty); }
            set { SetValue(ValuePlaceholderStyleProperty, value); }
        }
        
        public static readonly BindableProperty HandleButtonClickProperty = BindableProperty.Create("HandleButtonClick", typeof(Command), typeof(MaskedDatePicker));
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
            if (ValidationFunc == null || ValidationFunc.Invoke(CalendarDate.Date))
            {
                Date = CalendarDate.Date;
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
            var self = bindable as MaskedDatePicker;
            if (self != null)
            {
                self.PlaceHolderText = self._placeholdeRegex.Replace(self.DateFormat, "_");
            }
        }

        static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = bindable as MaskedDatePicker;
            if (self != null)
            {
                self.ValueText = self.CalendarDate.ToString(self.DateFormat);
            }
        }
    }
}
