using System;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class TypeMediaIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var sourceValue = (FileImageSource)value;
            if(sourceValue.File.EndsWith(".3gp"))
            {
                return new FileImageSource() { File = "sound.png" };
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
