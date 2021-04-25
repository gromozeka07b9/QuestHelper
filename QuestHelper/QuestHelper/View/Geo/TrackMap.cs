using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Managers;
using QuestHelper.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace QuestHelper.View.Geo
{
    public class TrackMap : Map
    {
        public bool UseInterceptTouchEvent { get; set; }
        public TrackMap()
        {
            UseInterceptTouchEvent = true;
        }
        
        public async Task CenterMapOnLastPosition()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    MapSpan currentPosition = MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude),
                        Distance.FromKilometers(1));
                    MainThread.BeginInvokeOnMainThread(() => { this.MoveToRegion(currentPosition); });
                }
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> {{"Action", "centerMapToPosition"}};
                Crashes.TrackError(exception, properties);
            }
        }
        
        public async Task CenterMap(double latitude, double longitude)
        {
            try
            {
                if ((latitude != 0) && (longitude != 0))
                {
                    MapSpan currentPosition = MapSpan.FromCenterAndRadius(new Position(latitude, longitude),
                        Distance.FromKilometers(10));
                    MainThread.BeginInvokeOnMainThread(() => { this.MoveToRegion(currentPosition); });
                }
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> {{"Action", "centerMapToPosition"}};
                Crashes.TrackError(exception, properties);
            }
        }

        public async Task UpdateTrackOnMap(IEnumerable<Tuple<double?, double?>> trackPlaces)
        {
            if (trackPlaces.Any())
            {
                Polyline trace = new Polyline()
                {
                    StrokeColor = Color.Blue,
                    StrokeWidth = 12,
                };
                foreach (var place in trackPlaces)
                {
                    trace.Geopath.Add(new Position(place.Item1??0, place.Item2??0));
                }
                this.MapElements.Add(new Circle()
                {
                    Center = new Position(trackPlaces.FirstOrDefault().Item1??0, trackPlaces.FirstOrDefault().Item2??0),
                    Radius = new Distance(250),
                    StrokeColor = Color.FromHex("#88FF0000"),
                    StrokeWidth = 8,
                    FillColor = Color.Red
                });
                this.MapElements.Add(trace);
                this.MapElements.Add(new Circle()
                {
                    Center = new Position(trackPlaces.LastOrDefault().Item1??0, trackPlaces.LastOrDefault().Item2??0),
                    Radius = new Distance(250),
                    StrokeColor = Color.FromHex("#88FF0000"),
                    StrokeWidth = 8,
                    FillColor = Color.Black
                });
                await this.CenterMap(trackPlaces.FirstOrDefault().Item1??0, trackPlaces.FirstOrDefault().Item2??0);
            }
        }

        public async Task UpdatePointsOnMap(IEnumerable<ViewRoutePoint> getRoutePoints, EventHandler<PinClickedEventArgs> routePointMarkerClicked)
        {
            string pathToPictures = ImagePathManager.GetPicturesDirectory();
            foreach (var routePoint in getRoutePoints)
            {
                var pin = new RoutePointPin()
                {
                    RoutePointId = routePoint.Id,
                    Position = new Position(routePoint.Latitude, routePoint.Longitude),
                    Label = routePoint.Name,
                    //ImageMarkerPath = !string.IsNullOrEmpty(routePoint.ImagePreviewPath) ? $"{_pathToPictures}/map_{routePoint.ImagePreviewPath}" : string.Empty
                    ImageMarkerPath = !string.IsNullOrEmpty(routePoint.ImagePreviewPath) ? routePoint.ImagePreviewPath : string.Empty
                };
                pin.MarkerClicked += routePointMarkerClicked;
                this.Pins.Add(pin);
            }
        }

    }
}