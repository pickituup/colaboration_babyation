using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BabyationApp.Resources;

namespace BabyationApp.Converters
{

    /// <summary>
    /// The converter to convert TimeSpan to string
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts TimeSpan to string
        /// </summary>
        /// <param name="value">The TimeSpan value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>returns the converted string</returns>
        public object Convert(object value, //the TimeSpan variable? if so im unsure how to get
                                            //that info without binding to picker then pulling from picker
                                            //that always ends up being zero once extracted from picker
                              Type targetType, //String?
                              object parameter, //??
                              System.Globalization.CultureInfo culture) //passing in the language? like en-US?
        {
            TimeSpan span = (TimeSpan)value;
            return TimeSpanToString(span);
        }

        /// <summary>
        /// Converts a given TimeSpan to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns>returns the converted string</returns>
        public static string TimeSpanToString(TimeSpan value)
        {
            TimeSpan span = (TimeSpan)value;
            if (span == TimeSpan.MinValue)
            {
                return "00:00";
            }
            string formatted = string.Format("{0}:{1}",
                string.Format("{0}", span.Minutes).PadLeft(2, '0'),
                string.Format("{0}", span.Seconds).PadLeft(2, '0'));
            if (formatted.EndsWith(", ", StringComparison.Ordinal)) formatted = formatted.Substring(0, formatted.Length - 2);
            if (string.IsNullOrEmpty(formatted)) formatted = "00:00";
            return formatted;
        }

        //not really sure, seems to be for exception, but i am unsure how it gets called or what exactly it does
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("TimeSpanToTimeStringConverter.ConvertBack");
        }
    }

    /// <summary>
    /// Converts a DateTime to String
    /// </summary>
    public class DatetimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime to string
        /// </summary>
        /// <param name="value">DateTime to convert to string</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>returns the converted string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "__/__/____";

            return DateTimeToString((DateTime) value);
        }

        public static string DateTimeToString(DateTime value)
        {
            return (value != DateTime.MinValue ? value.ToString("MM/dd/yyyy") : "__/__/____");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("DatetimeToStringConverter.ConvertBack");
        }
    }


    /// <summary>
    /// Converts a DateTime to String
    /// </summary>
    public class DatetimeHasValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime to string
        /// </summary>
        /// <param name="value">DateTime to convert to string</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>returns the converted string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;

            return dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue ? false : (object)true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("DatetimeHasValueConverter.ConvertBack");
        }
    }

    /// <summary>
    /// Converts a DateTime to String
    /// </summary>
    public class TimeSpanToHMValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime to string
        /// </summary>
        /// <param name="value">DateTime to convert to string</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>returns the converted string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string hours = "0";
            string minutes = "0";

            if (null == value )
            {
                //return string.Format("0 {0} 0 {1}", AppResource.HoursLower, AppResource.MinutesLower);
                value = "0";
            }
            else
            {
                TimeSpan span = TimeSpan.Zero;

                if (value.GetType().Equals(typeof(double)))
                {
                    span = TimeSpan.FromMinutes((double)value);
                }
                else if (value.GetType().Equals(typeof(String)))
                {
                    span = TimeSpan.Parse(value as String);
                }

                if (span == TimeSpan.MinValue)
                {
                    span = TimeSpan.Zero;
                }
                hours = span.Hours.ToString();
                minutes = span.Minutes.ToString();
            }

            string formatted = string.Format("{0} {1} {2} {3}", hours, AppResource.HoursLower, minutes, AppResource.MinutesLower);

            return formatted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("TimeSpanToHMValueConverter.ConvertBack");
        }
    }

    public class DatetimeToReminderDateConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime to string
        /// </summary>
        /// <param name="value">DateTime to convert to string</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>returns the converted string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                value = DateTime.Now;

            return DateTimeToString((DateTime)value);
        }

        public static string DateTimeToString(DateTime value)
        {
            return (value != DateTime.MinValue ? value.ToString("ddd d/M/y") : "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("DatetimeToStringConverter.ConvertBack");
        }
    }

    public class DatetimeToReminderTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime to string
        /// </summary>
        /// <param name="value">DateTime to convert to string</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>returns the converted string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                value = DateTime.Now;

            return DateTimeToString((DateTime)value);
        }

        public static string DateTimeToString(DateTime value)
        {
            return (value != DateTime.MinValue ? value.ToString("H:mm") : "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("DatetimeToStringConverter.ConvertBack");
        }
    }
}
