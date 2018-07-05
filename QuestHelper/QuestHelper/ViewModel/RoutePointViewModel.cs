using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        private async void fillCurrentPositionAsync()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            _point.Latitude = position.Latitude;
            _point.Longitude = position.Longitude;
            Coordinates = _point.Latitude + "," + _point.Longitude;
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
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _point.Name;
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
