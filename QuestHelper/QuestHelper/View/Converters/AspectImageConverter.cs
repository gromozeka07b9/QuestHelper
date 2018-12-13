﻿using System;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class AspectImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "camera1.png")
            {
                return Aspect.AspectFit;
            }
            else return Aspect.AspectFill;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}