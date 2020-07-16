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
                try
                {                    
                    //List<string> imgPathArray = (List<string>)value;
                    List<string> imgPathArray = new List<string>((IEnumerable<string>)value);
                    int index = Int32.Parse((string)parameter);
                    if ((imgPathArray.Count > 0) && (parameter != null) && (index < imgPathArray.Count))
                    {

                        imgPath = imgPathArray[index];
                    }
                }
                catch
                {

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
