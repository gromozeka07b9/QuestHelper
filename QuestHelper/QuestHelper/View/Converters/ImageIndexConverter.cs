using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class ImageIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imgPath = string.Empty;
            if(value != null)
            {
                List<string> imgPathArray = (List<string>)value;
                if ((imgPathArray.Count > 0) && (parameter != null))
                {
                    imgPath = imgPathArray[Int32.Parse((string)parameter)];
                }
            }
            return imgPath;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
