﻿using System;
using Xamarin.Forms;

namespace QuestHelper.View.Converters
{
    public class LockImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool) value ? "lockopen.png" : "lock.png";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
