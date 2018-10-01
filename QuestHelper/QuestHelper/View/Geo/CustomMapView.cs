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

namespace QuestHelper.View.Geo
{
    public class CustomMapView
    {
        IGeolocator _locator = CrossGeolocator.Current;
        CustomMap _customMapView;
        Plugin.Geolocator.Abstractions.Position _currentPosition;
        int _timeoutInSeconds = 15;
        private string _lastError = string.Empty;

        public string LastError { get => _lastError;}
        public Plugin.Geolocator.Abstractions.Position CurrentPosition { get => _currentPosition;}

        public CustomMapView(CustomMap customMapView, int timeoutInSeconds)
        {
            _customMapView = customMapView;
            _timeoutInSeconds = timeoutInSeconds;
        }
        public bool CenterMapToPosition(double Latitude, double Longitude)
        {
            bool result = false;
            _lastError = string.Empty;
            try
            {
                _customMapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(Latitude, Longitude), Distance.FromKilometers(1)));
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
        public async Task<bool> GetPositionAsync()
        {
            bool result = false;
            _lastError = string.Empty;
            if ((_locator.IsGeolocationAvailable)&&(_locator.IsGeolocationEnabled))
            {
                try
                {
                    _currentPosition = await _locator.GetPositionAsync(TimeSpan.FromSeconds(_timeoutInSeconds));
                    result = true;
                }
                catch (Exception)
                {
                    _lastError = "Ошибка определения позции. Убедитесь, что сервис геолокации включен и попробуйте еще раз.";
                }
            } else
            {
                _lastError = "Сервис геолокации недоступен. Убедитесь, что он включен и попробуйте еще раз.";
            }
            return result;
        }

        public void AddPin(double latitude, double longitude, string name, string address, EventHandler pin_clicked)
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
            pointPin.Clicked += pin_clicked;
            _customMapView.Pins.Add(pointPin);
        }
    }
}
