using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper.Managers
{
    public class GeolocatorManager
    {
        public async Task<(double Latitude, double Longtitude)> GetCurrentLocationAsync()
        {
            var locator = CrossGeolocator.Current;
            Position currentPosition = new Position();
            if ((locator.IsGeolocationAvailable) && (locator.IsGeolocationEnabled))
            {
                currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            }
            else
            {
                UserDialogs.Instance.Alert(title: CommonResource.CommonMsg_GeolocationNotEnabled,
                    message: CommonResource.CommonMsg_ForGettingCoordinatesNeedGeolocationEnabled, okText: "Ok");
                Analytics.TrackEvent("Geolocation: off", new Dictionary<string, string> { { "GeolocatorManager", "GetCurrentLocationAsync" } });
            }
            return (currentPosition.Latitude, currentPosition.Longitude);
        }
    }
}
