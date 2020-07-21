using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using QuestHelper.Managers;
using QuestHelper.Model;
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
                    ObservableCollection<ViewLocalFile> imgPathArray = (ObservableCollection<ViewLocalFile>)value;
                    int index = Int32.Parse((string)parameter);
                    if ((imgPathArray.Count > 0) && (parameter != null) && (index < imgPathArray.Count))
                    {
                        imgPath = ImagePathManager.GetImagePath(imgPathArray[index].Id, LocalDB.Model.MediaObjectTypeEnum.Image, true);
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
