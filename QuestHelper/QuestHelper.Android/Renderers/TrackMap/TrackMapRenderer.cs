using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using QuestHelper.View.Geo;
using System;
using System.Collections.Generic;
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
        private View.Geo.TrackMap trackMapContext;

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
        
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                trackMapContext = (View.Geo.TrackMap) e.NewElement;
                Control.GetMapAsync(this);
            }
        }
        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            //map.MapClick += Map_MapClick;
            
            if (trackMapContext.IsShowConnectedRoutePointsLines)
            {
                drawConnectedLines();
            }
            base.OnMapReady(map);
        }

        private void drawConnectedLines()
        {
            List<PatternItem> pattern_lines = new List<PatternItem>();
            pattern_lines.Add(new Gap(10));
            pattern_lines.Add(new Dash(15));
            PolylineOptions lineOptions = new PolylineOptions();
            
            foreach(var point in trackMapContext.RoutePoints)
            {
                lineOptions.Add(new LatLng(point.Latitude, point.Longitude));
            }
            lineOptions.InvokePattern(pattern_lines);
            lineOptions.InvokeWidth(10);
            NativeMap.AddPolyline(lineOptions);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            if(pin.Label.Equals("StartTrackPin") && pin.GetType() == typeof(Pin))
            {
                return BaseMarkerMaker.MakeStartMarker(pin, 40);
            } 
            if(pin.Label.Equals("FinishTrackPin") && pin.GetType() == typeof(Pin))
            {
                return BaseMarkerMaker.MakeFinishMarker(pin, 40);
            } 
            return RoutePointMarkerMaker.Make(pin as RoutePointPin, _maxWidthImage);
        }
    }
}