using System;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class AspectImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imageName = value.ToString();
            return (imageName.Contains("empty") || (imageName == "camera1.png") || (imageName == "mount1.png") ||
                   (imageName.Contains(".3gp")) ? Aspect.AspectFit : Aspect.AspectFill);
            /*if ((imageName == "camera1.png") || (imageName == "mount1.png") || (imageName == "emptyphoto.png") || (imageName == "emptylist.png") || (imageName.Contains(".3gp")))
            {
                return Aspect.AspectFit;
            }
            
            return Aspect.AspectFill;*/
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
