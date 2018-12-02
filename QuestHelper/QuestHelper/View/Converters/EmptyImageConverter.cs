using System;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class EmptyImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return "defaultimg_small.png";
            }
            else return value;
            //return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
