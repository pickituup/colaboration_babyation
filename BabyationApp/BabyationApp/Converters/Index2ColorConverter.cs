using System;
using System.Globalization;
using Xamarin.Forms;
namespace BabyationApp.Converters
{
    public class IndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if( null == value ) return Application.Current.Resources["Peach"];

            return (0 == (int)value % 2 ? Application.Current.Resources["Peach"]: Application.Current.Resources[" Peach30"]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
