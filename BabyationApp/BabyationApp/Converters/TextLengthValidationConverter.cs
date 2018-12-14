using System;
using System.Globalization;
using Xamarin.Forms;

namespace BabyationApp.Converters
{
    public class TextLengthValidationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null != parameter && parameter.GetType().Equals(typeof(int)))
            {
                int wordCount = (int)parameter;

                return 0 <= wordCount ? wordCount - System.Convert.ToInt32(value) : value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
