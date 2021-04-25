using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace QuestHelper.View.Geo
{
    public class RoutePointPin : Pin
    {
        //public string Url { get; set; }
        public string RoutePointId { get; set; }
        //public string ImagePath { get; set; }
        public string ImageMarkerPath { get; set; }
    }
}
