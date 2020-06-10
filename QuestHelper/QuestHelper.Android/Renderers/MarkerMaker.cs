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
        internal static MarkerOptions MakeMarkerByPOI(Pin poi, int imageSize)
        {
            string imgPath = ((OverViewMapPin)poi).ImagePath;
            var latlng = new LatLng(poi.Position.Latitude, poi.Position.Longitude);
            var marker = new MarkerOptions();
            marker.Anchor(0.5f, 0.5f);
            marker.SetPosition(latlng);
            //BitmapDescriptor pic = getBitmap(imgPath, imageSize, poi.Label) ;
            BitmapDescriptor pic = null;
            if (pic == null)
            {
                //pic = BitmapDescriptorFactory.FromResource(Resource.Drawable.place_unknown);
                pic = BitmapDescriptorFactory.FromPath(imgPath);
            }
            marker.SetIcon(pic);
            return marker;
        }

        private static BitmapDescriptor getBitmap(string pathToPicture, int imageSize, string name)
        {
            if (!string.IsNullOrEmpty(pathToPicture) && File.Exists(pathToPicture))
            {
                try
                {
                    Android.Graphics.Bitmap bm = BitmapFactory.DecodeFile(pathToPicture);
                    if (bm != null)
                    {
                        var croppedBitmap = BitmapConverter.Crop(bm, imageSize);
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