using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Model;
using Xamarin.Forms.Maps;

namespace QuestHelper.View.Geo
{
    public class CustomOverviewMap : Map
    {
        public List<POI> POIs { get; set; } = new List<POI>();

        public CustomOverviewMap()
        {
        }
    }
}
