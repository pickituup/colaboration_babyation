using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Resources;
using Xamarin.Forms;
using BabyationApp.Helpers;
using System.Globalization;

namespace BabyationApp.Converters
{
    /// <summary>
    /// Convert a boolean to its inverse
    /// </summary>
    public class BooleanInverseConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to its inverse
        /// </summary>
        /// <param name="value">the given bool value to convert to its inverse</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>the inverted value</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value) return false;

            bool state = (bool) value;
            return !state;
        }

        /// <summary>
        /// Converts back a boolean to its inverse
        /// </summary>
        /// <param name="value">the given bool value to convert to its inverse</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>the inverted value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool state = (bool)value;
            return !state;
        }
    }

    /// <summary>
    /// Converts an object to boolean; If object is null, converts to false; otherwise true
    /// </summary>
    public class ObjectNotNullCheckConverter : IValueConverter
    {
        /// <summary>
        /// Converts the object to boolean
        /// </summary>
        /// <param name="value">the given object to convert to boolean</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>the converted bool value</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }
            else
            {
                return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("ObjectNotNullCheckConverter.ConvertBack");
        }
    }

    /// <summary>
    /// Converts an object to boolean inverse; If object is null, converts to true; otherwise false
    /// </summary>
    public class ObjectIsNullCheckConverter : IValueConverter
    {
        /// <summary>
        /// Converts the object to inverse boolean
        /// </summary>
        /// <param name="value">the given object to convert to inverse boolean</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">extra parameter</param>
        /// <param name="culture">culture to use to do the conversion</param>
        /// <returns>the converted inverse bool value</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("ObjectIsNullCheckConverter.ConvertBack");
        }
    }

    /// <summary>
    /// Converts a DateTime to String
    /// </summary>
    public class DoubleToHMValueConverter : IValueConverter
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

            if (null == value)
            {
                //return string.Format("0 {0} 0 {1}", AppResource.HoursLower, AppResource.MinutesLower);
                value = "0";
            }
            else
            {
                double dValue = -1;

                if (value.GetType().Equals(typeof(String)))
                {
                    dValue = Double.Parse(value as String);
                }
                else if (value.GetType().Equals(typeof(Double)))
                {
                    dValue = (Double)value;
                }

                if( 0 <= dValue  )
                {
                    hours = String.Format("{0:F0}", dValue);
                    minutes = String.Format("{0}", dValue.GetDecimalPart(100));
                }
            }

            string formatted = string.Format("{0} {1} {2} {3}", hours, AppResource.HoursLower, minutes, AppResource.MinutesLower);

            return formatted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("DoubleToHMValueConverter.ConvertBack");
        }
    }

    public class BoolToGenericObjectConverter<T> : IValueConverter
    {

        public T TrueObject { get; set; }

        public T FalseObject { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? TrueObject : FalseObject;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((T)value).Equals(TrueObject);
        }
    }
}
