using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    class RoutePointViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        RoutePoint _point;
        Route _route;
        string _currentPositionString = string.Empty;

        public RoutePointViewModel(Route route, RoutePoint routePoint)
        {
            _route = route;
            _point = routePoint;
            SaveCommand = new Command(saveRoutePoint);
            DeleteCommand = new Command(deleteRoutePoint);
            if((_point.Latitude == 0)&&(_point.Longitude==0))
                fillCurrentPositionAsync();
            Coordinates = Latitude + "," + Longitude;
        }

        private async void fillCurrentPositionAsync()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            Latitude = position.Latitude;
            Longitude = position.Longitude;
            Coordinates = Latitude + "," + Longitude;
            Address = await getPositionAddress(locator, position);
            Name = Address;
        }

        private async Task<string> getPositionAddress(IGeolocator locator, Position position)
        {
            string address = string.Empty;
            try
            {
                IEnumerable<Address> addresses = await locator.GetAddressesForPositionAsync(position);
                var addressItem = addresses.FirstOrDefault();
                address = $"{addressItem.SubThoroughfare}, {addressItem.Thoroughfare}, {addressItem.Locality}, {addressItem.CountryName}";
            }
            catch (Exception)
            {
                //лог
            }
            return address;
        }

        void saveRoutePoint()
        {
            if(_point.MainRoute == null)
            {
                _point.MainRoute = _route;
                RoutePointManager manager = new RoutePointManager();
                if (!manager.Add(_point, _route))
                {
                    //куда-то ошибку надо фиксировать
                };
            }
            Navigation.PopAsync();
        }

        async void deleteRoutePoint()
        {

        }

        public double Latitude
        {
            set
            {
                if (_point.Latitude != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Latitude = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                }
            }
            get
            {
                return _point.Latitude;
            }
        }
        public double Longitude
        {
            set
            {
                if (_point.Longitude != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Longitude = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
                }
            }
            get
            {
                return _point.Longitude;
            }
        }
        public string Coordinates
        {
            set
            {
                if (_currentPositionString != value)
                {
                    _currentPositionString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                }
            }
            get
            {
                return _currentPositionString;
            }
        }
        public string Name
        {
            set
            {
                if (_point.Name != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Name = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _point.Name;
            }
        }

        public string Address
        {
            set
            {
                if (_point.Address != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Address = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
                }
            }
            get
            {
                return _point.Address;
            }
        }
        public string Description
        {
            set
            {
                if (_point.Description != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Description = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
            get
            {
                return _point.Description;
            }
        }
    }
}
