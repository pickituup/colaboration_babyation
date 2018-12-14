using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public enum SessionType
    {
        Pump,
        Nurse,
        Breastmilk,
        Formula,
        BottleFeed,
        Max
    }

    public class SessionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SessionType st = (SessionType)value;
            String text = String.Empty;
            switch (st)
            {
                case SessionType.Pump:
                    text = "Pump";
                    break;
                case SessionType.Breastmilk:
                    text = "Breastmilk";
                    break;
                case SessionType.Formula:
                    text = "Formula";
                    break;
                case SessionType.Nurse:
                    text = "Nurse";
                    break;
                case SessionType.BottleFeed:
                    text = "Bottle";
                    break;
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("SessionTypeToIconConverter.ConvertBack");
        }
    }

    public class SessionTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SessionType st = (SessionType)value;
            String icon = String.Empty;
            switch (st)
            {
                case SessionType.Pump:
                    icon = "btn_fridge.png";
                    break;
                case SessionType.Breastmilk:
                    icon = "btn_breast_milk.png";
                    break;
                case SessionType.Formula:
                    icon = "btn_formula.png";
                    break;
                case SessionType.Nurse:
                    icon = "icon_nurse.png";
                    break;
                case SessionType.BottleFeed:
                    icon = "btn_breast_milk.png";
                    break;
            }
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("SessionTypeToIconConverter.ConvertBack");
        }
    }
}
