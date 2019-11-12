using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<(string PositionAddress, string PointName)> GetPositionAddress(Position position)
        {
            var locator = CrossGeolocator.Current;
            string address = string.Empty;
            string pointName = string.Empty;
            try
            {
                IEnumerable<Address> addresses = await locator.GetAddressesForPositionAsync(position);
                var addressItem = addresses.FirstOrDefault();
                if (addressItem != null)
                {
                    address = $"{addressItem.SubThoroughfare}, {addressItem.Thoroughfare}, {addressItem.Locality}, {addressItem.CountryName}";
                    if (!string.IsNullOrEmpty(addressItem.SubLocality))
                    {
                        pointName = $"{addressItem?.SubLocality}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(addressItem.Locality))
                        {
                            pointName = $"{addressItem.CountryName},{addressItem.Locality}";
                        }
                        else
                        {
                            pointName = $"{addressItem.CountryName},{addressItem.SubAdminArea}";
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "GeolocatorManager", "GetPositionAddress" } };
                Crashes.TrackError(exception, properties);
            }
            return (address, pointName);
        }

    }
}
