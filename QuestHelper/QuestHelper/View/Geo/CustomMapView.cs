using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using QuestHelper.Managers;

namespace QuestHelper.View.Geo
{
    public class CustomMapView
    {
        IGeolocator _locator = CrossGeolocator.Current;
        CustomMap _customMapView;
        int _timeoutInSeconds = 15;
        private string _lastError = string.Empty;

        public string LastError { get => _lastError;}
        public List<Xamarin.Forms.Maps.Position> RouteCoordinates { get; set; }

        public CustomMapView(CustomMap customMapView, int timeoutInSeconds)
        {
            _customMapView = customMapView;
            _timeoutInSeconds = timeoutInSeconds;
        }
        public bool CenterMapToPosition(double Latitude, double Longitude, double ScaleKilometers)
        {
            bool result = false;
            _lastError = string.Empty;
            try
            {
                _customMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(Latitude, Longitude), Distance.FromKilometers(ScaleKilometers)));
                result = true;
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "Action", "centerMapToPosition" } };
                Crashes.TrackError(exception, properties);
                _lastError = "Ошибка ";
            }
            return result;
        }

        public void ClearPins()
        {
            _customMapView.Pins.Clear();
        }

        public void AddPin(double latitude, double longitude, string name, string address)
        {
            _lastError = string.Empty;
            var position = new Xamarin.Forms.Maps.Position(latitude, longitude);
            var pointPin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = name,
                Address = address
            };
            _customMapView.Pins.Add(pointPin);
        }

    }
}
