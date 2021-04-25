using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using QuestHelper.Consts;
using QuestHelper.Droid;
using QuestHelper.Droid.Renderers;
using QuestHelper.Managers;
using QuestHelper.Resources;
using QuestHelper.View;
using QuestHelper.View.Geo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomOverviewMap), typeof(CustomOverviewMapRenderer))]
namespace QuestHelper.Droid.Renderers
{
    public class CustomOverviewMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private readonly int _maxWidthImage = Convert.ToInt32(DeviceSize.FullScreenWidth * 1);
        public CustomOverviewMapRenderer(Context context) : base(context)
        {
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }
        
        protected override MarkerOptions CreateMarker(Pin pin)
        {
            return PoiMarkerMaker.Make(pin as PoiPin, _maxWidthImage);
        }
    }
}