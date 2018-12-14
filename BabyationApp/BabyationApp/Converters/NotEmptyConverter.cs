using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;
using System.Linq;

namespace BabyationApp.Converters
{
    public class NotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is IList)
            {
                return (value as IList).Count > 0;
            }
            else if (value is ICollection)
            {
                return (value as ICollection).Count > 0;
            }
            else
            {
                return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value;
        }
    }

    public class IntGreaterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int convertedParam;
            if (value is int && int.TryParse((string)parameter, out convertedParam))
            {
                return (int) value > convertedParam;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("IntGreaterConverter.ConvertBack");
        }
    }
}
