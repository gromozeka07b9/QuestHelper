using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using QuestHelper.View.Geo;
using System;
using QuestHelper.Consts;
using QuestHelper.Droid.Renderers.TrackMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(TrackMap), typeof(TrackMapRenderer))]
namespace QuestHelper.Droid.Renderers.TrackMap
{
    public class TrackMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private readonly int _maxWidthImage = Convert.ToInt32(DeviceSize.FullScreenWidth * 0.75);

        public TrackMapRenderer(Context context) : base(context)
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
            return RoutePointMarkerMaker.Make(pin as RoutePointPin, _maxWidthImage);
        }
    }
}