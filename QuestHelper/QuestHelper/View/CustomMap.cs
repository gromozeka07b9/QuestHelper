using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace QuestHelper.View
{
    public class CustomMap : Map
    {
        public List<Xamarin.Forms.Maps.Position> RouteCoordinates { get; set; }

        public CustomMap()
        {
            RouteCoordinates = new List<Xamarin.Forms.Maps.Position>();
        }
    }
}
