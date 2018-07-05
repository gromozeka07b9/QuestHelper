using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private Route _route;
        private RoutePoint _point;
        private RoutePointManager _routePointManager = new RoutePointManager();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartNewRouteCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StopRecordRouteCommand { get; private set; }

        public EditRouteViewModel(Route route)
        {
            StartNewRouteCommand = new Command(startNewRoute);
            AddNewRoutePointCommand = new Command(addNewRoutePoint);
            StopRecordRouteCommand = new Command(stopRecordRoute);
            _route = route;
            if (!string.IsNullOrEmpty(_route.Name))
            {
                showDetailRouteData();
            }
            else
            {
                _route.Name = "Неизвестный маршрут";
                showNewRouteData();
            }
        }

        private void showNewRouteData()
        {
            SplashStartScreenIsVisible = true;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
        }

        private void showDetailRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            var _points = _routePointManager.GetPointsByRoute(_route);//.ToList().Add(new RoutePoint() { });
            var newItemCollection = new List<RoutePoint>();
            newItemCollection.Add(new RoutePoint());
            _pointsOfRoute = _points.Concat(newItemCollection);
        }

        /*private void addPossibleNewPoint(IEnumerable<RoutePoint> pointsOfRoute)
        {
            pointsOfRoute.ToList().Add(new RoutePoint() { });
        }*/

        async void addNewRoutePoint()
        {
            var routePointPage = new RoutePointPage(_route, new RoutePoint());
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
        public RoutePoint SelectedRoutePointItem
        {
            set
            {
                if (_point != value)
                {
                    _point = value;
                    Navigation.PushAsync(new RoutePointPage(_route, value));

                }
            }
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
