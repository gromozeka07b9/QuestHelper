using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace QuestHelper.WS
{
    public class CustomGeocoding
    {
        private string _address = string.Empty;
        private IEnumerable<Location> _locations;

        public double Longtitude
        {
            get
            {
                var location = _locations.FirstOrDefault();
                return location != null ? location.Longitude : 0;
            }
        }

        public double Latitude
        {
            get
            {
                var location = _locations.FirstOrDefault();
                return location != null ? location.Latitude : 0;
            }
        }

        public CustomGeocoding(string address)
        {
            _address = address;
        }

        internal async Task<bool> GetCoordinatesAsync()
        {
            try
            {
                _locations = await Geocoding.GetLocationsAsync(_address);
                return true;
            }
            catch (Exception e)
            {
                HandleError.Process("CustomGeocoding", "GetCoordinatesAsync", e, false);
            }
            return false;
        }
    }
}
