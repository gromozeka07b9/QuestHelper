using Plugin.Geolocator;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    class NewRouteViewModel : INotifyPropertyChanged
    {
        private bool _splashStartScreenIsVisible;
        private bool _routeScreenIsVisible;
        private List<string> _pointsOfNewRoute;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartNewRouteCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StopRecordRouteCommand { get; private set; }

        public NewRouteViewModel()
        {
            StartNewRouteCommand = new Command(startNewRoute);
            AddNewRoutePointCommand = new Command(addNewRoutePoint);
            StopRecordRouteCommand = new Command(stopRecordRoute);
            SplashStartScreenIsVisible = true;
            RouteScreenIsVisible = false;
            _pointsOfNewRoute = new List<string>();
            _pointsOfNewRoute.Add("Царь-пушка");
            _pointsOfNewRoute.Add("Оружейная палата");
            _pointsOfNewRoute.Add("ЦУМ");
            _pointsOfNewRoute.Add("Мавзолей Ленина");
            _pointsOfNewRoute.Add("Спасская башня");
            _pointsOfNewRoute.Add("Сенатская башня");
            _pointsOfNewRoute.Add("Храм Василия Блаженного");
        }
        async void addNewRoutePoint()
        {
            var locator = CrossGeolocator.Current;
            var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            var cache = _pointsOfNewRoute;
            _pointsOfNewRoute = new List<string>();
            _pointsOfNewRoute.AddRange(cache);
            _pointsOfNewRoute.Add("Широта:" + currentPosition.Latitude + " Долгота:" + currentPosition.Longitude);
            PropertyChanged(this, new PropertyChangedEventArgs("PointsOfNewRoute"));
        }
        void stopRecordRoute()
        {
            _pointsOfNewRoute = new List<string>();
            PropertyChanged(this, new PropertyChangedEventArgs("PointsOfNewRoute"));
        }

        void startNewRoute()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
        }
        public bool SplashStartScreenIsVisible
        {
            set
            {
                if (_splashStartScreenIsVisible != value)
                {
                    _splashStartScreenIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SplashStartScreenIsVisible"));
                    }
                }
            }
            get
            {
                return _splashStartScreenIsVisible;
            }
        }
        public bool RouteScreenIsVisible
        {
            set
            {
                if (_routeScreenIsVisible != value)
                {
                    _routeScreenIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RouteScreenIsVisible"));
                    }
                }
            }
            get
            {
                return _routeScreenIsVisible;
            }
        }
        public List<string> PointsOfNewRoute
        {
            set
            {
                if (_pointsOfNewRoute != value)
                {
                    _pointsOfNewRoute = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PointsOfNewRoute"));
                    }
                }
            }
            get
            {
                return _pointsOfNewRoute;
            }
        }
    }
}
