using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Model;
using Xamarin.Forms.Maps;

namespace QuestHelper.View
{
    public class CustomMap : Map
    {
        public List<PointForMap> Points { get; set; }
        public bool UseInterceptTouchEvent { get; set; }

        public CustomMap()
        {
            Points = new List<PointForMap>();
            UseInterceptTouchEvent = true;
        }
    }
}
