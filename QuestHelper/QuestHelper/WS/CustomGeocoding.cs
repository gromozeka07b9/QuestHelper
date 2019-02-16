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
                return _locations.FirstOrDefault().Longitude;
            }
        }

        public double Latitude
        {
            get
            {
                return _locations.FirstOrDefault().Latitude;
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
