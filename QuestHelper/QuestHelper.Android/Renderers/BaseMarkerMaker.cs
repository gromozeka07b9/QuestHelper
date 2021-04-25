using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Model;
using QuestHelper.View.Geo;
using System;
using System.Collections.Generic;
using System.IO;
using QuestHelper.Consts;
using Xamarin.Forms.Maps;

namespace QuestHelper.Droid.Renderers
{
    internal abstract class BaseMarkerMaker
    {
        public static MarkerOptions Make(Pin poi, int maxWidthImage, string imageMarkerPath)
        {
            var marker = new MarkerOptions();
            marker.Anchor(0.5f, 0.5f);
            marker.SetPosition(new LatLng(poi.Position.Latitude, poi.Position.Longitude));

            BitmapDescriptor pic = null;
            pic = !string.IsNullOrEmpty(imageMarkerPath) ? getBitmap(imageMarkerPath, maxWidthImage) : BitmapDescriptorFactory.FromResource(Resource.Drawable.place_unknown);
            marker.SetIcon(pic);
            return marker;
        }

        public static BitmapDescriptor getBitmap(string pathToPicture, int maxWidthImage)
        {
            if (!string.IsNullOrEmpty(pathToPicture) && File.Exists(pathToPicture))
            {
                try
                {
                    Bitmap bm = BitmapFactory.DecodeFile(pathToPicture);
                    if (bm != null)
                    {
                        var croppedBitmap = BitmapConverter.Crop(bm, bm.Width > maxWidthImage ? maxWidthImage : bm.Width);
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