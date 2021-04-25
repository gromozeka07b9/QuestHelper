using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace QuestHelper.View.Geo
{
    public class PoiPin : Pin
    {
        public string PoiId { get; set; }
        //public string ImagePath { get; set; }
        public string ImageMarkerPath { get; set; }
    }
}
