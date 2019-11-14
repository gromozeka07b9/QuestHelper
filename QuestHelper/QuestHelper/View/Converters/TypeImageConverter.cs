using System;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class TypeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imgPath = value.ToString();
            if (!string.IsNullOrEmpty(imgPath))
            {
                if(imgPath.Contains(".3gp")) return "sound.png";
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
