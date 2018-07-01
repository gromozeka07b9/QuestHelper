using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    public class EditRouteViewModel : INotifyPropertyChanged
    {
        private bool _splashStartScreenIsVisible;
        private bool _routeScreenIsVisible;
        private IEnumerable<RoutePoint> _pointsOfRoute;
        private Route _routeItem;
        private RoutePointManager _routePointManager = new RoutePointManager();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartNewRouteCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StopRecordRouteCommand { get; private set; }

        public EditRouteViewModel(Route routeItem)
        {
            StartNewRouteCommand = new Command(startNewRoute);
            AddNewRoutePointCommand = new Command(addNewRoutePoint);
            StopRecordRouteCommand = new Command(stopRecordRoute);
            _routeItem = routeItem;
            if (!string.IsNullOrEmpty(routeItem.Name))
            {
                showDetailRouteData();
            }
            else
            {
                showNewRouteData();
            }
        }

        private void showNewRouteData()
        {
            SplashStartScreenIsVisible = true;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            /*_route = new Route();
            IEnumerable<RoutePoint> points = _routePointManager.GetPointsByRoute();
            _pointsOfNewRoute = new List<string>();
            foreach (var item in points)
            {
                _pointsOfNewRoute.Add($"name:{item.Name} latitude:{item.Latitude} longitude: {item.Longitude}");
            }*/
        }

        private void showDetailRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            _pointsOfRoute = _routePointManager.GetPointsByRoute(_routeItem);
        }

        async void addNewRoutePoint()
        {
            var route = new Route();
            route.Name = "new";
            var point = new RoutePoint();
            point.Name = "test1";
            point.Longitude = 1;
            point.Latitude = 2;
            point.MainRoute = route;
            _routePointManager.Save(point, route);
            /*var realm = Realm.GetInstance();
            realm.Write(() =>
            {
                realm.Add(point);
            }
            );*/
            /*var realm = Realm.GetInstance();
            var locator = CrossGeolocator.Current;
            var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            var cache = _pointsOfNewRoute;
            _pointsOfNewRoute = new List<string>();
            _pointsOfNewRoute.AddRange(cache);
            _pointsOfNewRoute.Add("Широта:" + currentPosition.Latitude + " Долгота:" + currentPosition.Longitude);

            realm.Write(()=>
                {
                    realm.Add(new Model.DB.RoutePoint() { Name="test_" + DateTime.Now, Latitude= currentPosition.Latitude , Longitude = currentPosition.Longitude });
                }
            );

            PropertyChanged(this, new PropertyChangedEventArgs("PointsOfNewRoute"));*/
            var routePointPage = new RoutePointPage(_routeItem);
            await Navigation.PushAsync(routePointPage);
        }
        void stopRecordRoute()
        {
            _pointsOfRoute = new List<RoutePoint>();
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
        public IEnumerable<RoutePoint> PointsOfRoute
        {
            set
            {
                if (_pointsOfRoute != value)
                {
                    _pointsOfRoute = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    }
                }
            }
            get
            {
                return _pointsOfRoute;
            }
        }
    }
}
