using Android.Gms.Maps.Model;
using Android.Graphics;
using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace QuestHelper.Droid.Renderers
{
    internal class MarkerMaker
    {
        internal static IEnumerable<MarkerOptions> MakeMarkersByPOI(List<POI> pois, int imageSize)
        {
            List<MarkerOptions> markers = new List<MarkerOptions>();
            foreach (var poi in pois)
            {
                var latlng = new LatLng(poi.Position.Latitude, poi.Position.Longitude);
                var marker = new MarkerOptions();
                marker.Anchor(0.5f, 0.5f);
                marker.SetPosition(latlng);
                BitmapDescriptor pic = getBitmap(poi.PathToPicture, imageSize);
                if (pic == null)
                {
                    pic = BitmapDescriptorFactory.FromResource(Resource.Drawable.place_unknown);
                }
                marker.SetIcon(pic);
                markers.Add(marker);
            }
            return markers;
        }

        private static BitmapDescriptor getBitmap(string pathToPicture, int imageSize)
        {
            if (!string.IsNullOrEmpty(pathToPicture) && File.Exists(pathToPicture))
            {
                Android.Graphics.Bitmap bm = BitmapFactory.DecodeFile(pathToPicture);
                if (bm != null)
                {
                    var croppedBitmap = BitmapConverter.Crop(bm, imageSize);
                    return BitmapDescriptorFactory.FromBitmap(croppedBitmap);
                }
            }
            return null;
        }
    }
}