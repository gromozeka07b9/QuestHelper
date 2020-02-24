using Android.Gms.Maps.Model;
using QuestHelper.Model;
using System;
using System.Collections.Generic;

namespace QuestHelper.Droid.Renderers
{
    internal class MarkerMaker
    {
        internal static IEnumerable<MarkerOptions> MakeMarkersByPOI(List<POI> pois)
        {
            List<MarkerOptions> markers = new List<MarkerOptions>();
            foreach (var poi in pois)
            {
                var marker = new MarkerOptions();
                markers.Add(marker);
            }
            return markers;
        }
    }
}