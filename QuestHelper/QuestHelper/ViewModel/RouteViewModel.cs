using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    public class RouteViewModel : INotifyPropertyChanged
    {
        private bool _splashStartScreenIsVisible;
        private bool _routeScreenIsVisible;
        private ObservableCollection<RoutePoint> _pointsOfRoute = new ObservableCollection<RoutePoint>();
        private Route _route;
        private RoutePoint _point;
        private RoutePointManager _routePointManager = new RoutePointManager();
        private bool _listIsRefreshing;
        private bool _noPointWarningIsVisible;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShowNewRouteDialogCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StartDialogCommand { get; private set; }

        public RouteViewModel(Route route)
        {
            _route = route;
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            AddNewRoutePointCommand = new Command(addNewRoutePointAsync);
            StartDialogCommand = new Command(startDialog);
            //startDialog();
        }

        public void startDialog()
        {
            if (!string.IsNullOrEmpty(_route.Name))
            {
                showRouteData();
            }
            else
            {
                _route.Name = "Неизвестный маршрут";
                showNewRouteWarningDialog();
            }
            ListIsRefreshing = false;
        }

        private void showNewRouteWarningDialog()
        {
            SplashStartScreenIsVisible = true;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
        }

        private void showRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            /*var _points = _routePointManager.GetPointsByRoute(_route);
            var newItemCollection = new List<RoutePoint>();
            newItemCollection.Add(new RoutePoint());
            _pointsOfRoute = _points.Concat(newItemCollection);*/
            var points = _routePointManager.GetPointsByRoute(_route);
            if (points.Count() == 0)
            {
                PointsOfRoute = new ObservableCollection<RoutePoint>();
            }
            else
            {
                PointsOfRoute = new ObservableCollection<RoutePoint>(points);
            }
            NoPointWarningIsVisible = PointsOfRoute.Count == 0;
        }
        void showNewRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            PointsOfRoute = new ObservableCollection<RoutePoint>() { new RoutePoint() };
        }

        async void addNewRoutePointAsync()
        {
            var routePointPage = new RoutePointPage(_route, new RoutePoint());
            await Navigation.PushAsync(routePointPage, true);
        }

        public RoutePoint SelectedRoutePointItem
        {
            set
            {
                if (_point != value)
                {
                    Navigation.PushAsync(new RoutePointPage(_route, value));
                    _point = null;
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRoutePointItem"));
                }
            }
        }

        public bool ListIsRefreshing
        {
            set
            {
                if (_listIsRefreshing != value)
                {
                    _listIsRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ListIsRefreshing"));
                    }
                }
            }
            get
            {
                return _listIsRefreshing;
            }
        }
        public bool NoPointWarningIsVisible
        {
            set
            {
                if (_noPointWarningIsVisible != value)
                {
                    _noPointWarningIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoPointWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noPointWarningIsVisible;
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
        public ObservableCollection<RoutePoint> PointsOfRoute
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
