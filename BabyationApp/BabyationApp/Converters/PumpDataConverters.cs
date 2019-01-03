using System;
using BabyationApp.Models;
using Xamarin.Forms;
using BabyationApp.Resources;
using BabyationApp.Helpers;

namespace BabyationApp.Converters
{
    public class PumpPhaseToToggleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null || !value.GetType().Equals(typeof(PumpPhase)))
                return false;

            PumpPhase pumpPhase = (PumpPhase)value;
            return (pumpPhase == PumpPhase.Expression); // true : false
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("PumpPhaseToToggleConverter.ConvertBack");
        }
    }

    public class PumpPhaseToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !value.GetType().Equals(typeof(PumpPhase)))
                return "icon_simulation";

            PumpPhase pumpPhase = (PumpPhase)value;
            return pumpPhase == PumpPhase.Stimulation ? "icon_simulation" : "icon_phase";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("PumpPhaseToImageConverter.ConvertBack");
        }
    }

    public class PumpPhaseToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !value.GetType().Equals(typeof(PumpPhase)))
                return AppResource.Stimulation;

            PumpPhase pumpPhase = (PumpPhase)value;
            return pumpPhase == PumpPhase.Stimulation ? AppResource.Stimulation : AppResource.Expression;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("PumpPhaseToTextConverter.ConvertBack");
        }
    }

    public class PumpAmountConverter : IValueConverter
    {
        static readonly double MAX_AMOUNT = 49.9F;

        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (value == null )
        //        return value;

        //    string inStr = (string)value;

        //    if( String.IsNullOrEmpty(inStr) || !InputValidator.IsValidInput(inStr))
        //        return value;

        //    double val = System.Convert.ToDouble(inStr);

        //    return val > MAX_AMOUNT ? MAX_AMOUNT.ToString() : val.ToString();
        //}

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            string inStr = (string)value;

            if (String.IsNullOrEmpty(inStr) || !InputValidator.IsValidInput(inStr))
                return value;

            double result = 0;
            if (double.TryParse(inStr, out result))
            {
                double val = System.Convert.ToDouble(inStr);
                return val > MAX_AMOUNT ? MAX_AMOUNT.ToString() : value;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
