using System;
using Xamarin.Forms;

namespace BabyationApp.Converters
{
    public class DebugVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}
