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
    internal class MarkerMaker
    {
        internal static MarkerOptions MakeMarkerByPOI(Pin poi, float zoomLevel)
        {
            string imgPath = ((OverViewMapPin)poi).ImageMarkerPath;
            var latlng = new LatLng(poi.Position.Latitude, poi.Position.Longitude);
            var marker = new MarkerOptions();
            marker.Anchor(0.5f, 0.5f);
            marker.SetPosition(latlng);

            BitmapDescriptor pic = null;
            if(zoomLevel > 0)
            {
                pic = getBitmap(imgPath, poi.Label);
            }
            else
            {
                //pic = BitmapDescriptorFactory.FromResource(Resource.Drawable.place_unknown);
            }
            marker.SetIcon(pic);
            return marker;
        }

        private static BitmapDescriptor getBitmap(string pathToPicture, string name)
        {
            if (!string.IsNullOrEmpty(pathToPicture) && File.Exists(pathToPicture))
            {
                try
                {
                    Android.Graphics.Bitmap bm = BitmapFactory.DecodeFile(pathToPicture);
                    if (bm != null)
                    {
                        var croppedBitmap = BitmapConverter.Crop(bm, bm.Width);
                        return BitmapDescriptorFactory.FromBitmap(croppedBitmap);
                    }
                }
                catch(Java.Lang.OutOfMemoryError excp)
                {
                    Crashes.TrackError(excp);
                }
            }
            return null;
        }
    }
}