using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Model;
using QuestHelper.View.Geo;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms.Maps;

namespace QuestHelper.Droid.Renderers
{
    internal class RoutePointMarkerMaker : BaseMarkerMaker
    {
        public static MarkerOptions Make(RoutePointPin poi, int maxWidthImage)
        {
            return Make(poi, maxWidthImage, poi.ImageMarkerPath);
        }
        
    }
}