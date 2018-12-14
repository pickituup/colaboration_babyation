using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Resources;
using Xamarin.Forms;

namespace BabyationApp.Models
{
    public enum MilkType
    {
        Formula,
        BreastMilk
    }

    public enum StorageType
    {
        Unspecified,
        Fridge,
        Freezer,
        Feed,
        Trash,
        Other
    }


    public class StorageTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StorageType st = (StorageType) value;
            String icon = String.Empty;
            switch (st)
            {
                case StorageType.Feed:
                    icon = "btn_breast_milk.png";
                    break;
                case StorageType.Freezer:
                    icon = "btn_freezer.png";
                    break;
                case StorageType.Fridge:
                    icon = "btn_fridge.png";
                    break;
                case StorageType.Trash:
                    icon = "btn_formula.png";
                    break;
                default:
                    icon = "btn_other.png";
                    break;
            }
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("StorageTypeToIconConverter.ConvertBack");
        }
    }

    public class StorageTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var st = (StorageType)value;

            string typeDescription = string.Empty;

            switch (st)
            {
                case StorageType.Feed:
                    typeDescription = AppResource.BreastMilk;
                    break;
                case StorageType.Freezer:
                    typeDescription = AppResource.Freezer;
                    break;
                case StorageType.Fridge:
                    typeDescription = AppResource.Fridge;
                    break;
                case StorageType.Trash:
                    typeDescription = AppResource.Trash;
                    break;
                default:
                    typeDescription = AppResource.Other;
                    break;
            }

            return typeDescription;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
            => throw new NotImplementedException("StorageTypeToIconConverter.ConvertBack");
    }
}
