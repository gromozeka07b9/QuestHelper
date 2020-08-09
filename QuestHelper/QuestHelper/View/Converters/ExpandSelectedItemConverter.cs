using System;
using QuestHelper.Consts;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class ExpandSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return (bool)value ? ((int)(DeviceSize.FullScreenHeight / 5)) : ((int)(DeviceSize.FullScreenHeight / 10));
            return (bool)value ? 285 : 60;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
