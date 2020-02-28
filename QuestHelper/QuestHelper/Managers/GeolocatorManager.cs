using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.Managers.Sync;
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
            Position currentPosition = new Position();
            var locator = CrossGeolocator.Current;
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
            return (currentPosition != null ? currentPosition.Latitude : 0, currentPosition != null ? currentPosition.Longitude : 0);
        }

        public async Task<(double Latitude, double Longtitude)> GetLastKnownPosition()
        {
            Position cachePosition = new Position();
            var locator = CrossGeolocator.Current;
            if ((locator.IsGeolocationAvailable) && (locator.IsGeolocationEnabled))
            {
                cachePosition = await locator.GetLastKnownLocationAsync();
            }
            else
            {
                UserDialogs.Instance.Alert(title: CommonResource.CommonMsg_GeolocationNotEnabled,
                    message: CommonResource.CommonMsg_ForGettingCoordinatesNeedGeolocationEnabled, okText: "Ok");
                Analytics.TrackEvent("Geolocation: off", new Dictionary<string, string> { { "GeolocatorManager", "GetLastKnownPosition" } });
            }
            return (cachePosition != null ? cachePosition.Latitude:0, cachePosition != null ? cachePosition.Longitude : 0);
        }

        public async Task<(string PositionAddress, string PointName)> GetPositionAddress(Position position)
        {
            string address = string.Empty;
            string pointName = string.Empty;
            SyncPossibility possibility = new SyncPossibility();
            bool networkAvailable = await possibility.CheckAsync(false);
            if (networkAvailable)
            {
                var locator = CrossGeolocator.Current;
                try
                {
                    IEnumerable<Address> addresses = await locator.GetAddressesForPositionAsync(position);
                    var addressItem = addresses.FirstOrDefault();
                    if (addressItem != null)
                    {
                        address = $"{addressItem.SubThoroughfare}, {addressItem.Thoroughfare}, {addressItem.Locality}, {addressItem.CountryName}";
                        if (!string.IsNullOrEmpty(addressItem.SubLocality))
                        {
                            pointName = $"{addressItem.SubLocality}";
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
            }
            return (address, pointName);
        }
    }
}
