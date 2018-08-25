using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
using QuestHelper.WS;
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
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
        private bool _listIsRefreshing;
        private bool _noPointWarningIsVisible;
        private RoutePointsApiRequest _routePointsApi = new RoutePointsApiRequest("http://questhelperserver.azurewebsites.net");
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest("http://questhelperserver.azurewebsites.net");
        private bool _isRefreshing;

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
        }

        public void startDialog()
        {
            if (!string.IsNullOrEmpty(_route.Name))
            {
                refreshRouteData();
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

        private async void refreshRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            IsRefreshing = true;
            var points = _routePointManager.GetPointsByRoute(_route);
            if (!points.Any())
            {
                PointsOfRoute = new ObservableCollection<RoutePoint>();
            }
            else
            {
                PointsOfRoute = new ObservableCollection<RoutePoint>(points);
            }
            NoPointWarningIsVisible = PointsOfRoute.Count == 0;
            IsRefreshing = false;
            List<RoutePoint> loadedPoints = await _routePointsApi.GetRoutePoints(_route.RouteId);
            _routePointManager.UpdateLocalData(_route, loadedPoints);
            foreach(var point in loadedPoints)
            {
                List<RoutePointMediaObject> loadedMediaObjects = await _routePointMediaObjectsApi.GetRoutePointMediaObjects(point.RoutePointId);
                _routePointMediaObjectManager.UpdateLocalData(point, loadedMediaObjects);
            }
        }
        void showNewRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            PointsOfRoute = new ObservableCollection<RoutePoint>() { new RoutePoint() };
        }

        async void addNewRoutePointAsync()
        {
            if(!_route.IsManaged)
                _routeManager.Add(_route);
            var routePointPage = new RoutePointPage(_route, new RoutePoint());
            await Navigation.PushAsync(routePointPage, true);
        }

        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshing"));
                    }
                }
            }
            get
            {
                return _isRefreshing;
            }
        }
        public string Name
        {
            set
            {
                if (_route.Name != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _route.Name = value;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _route.Name;
            }
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
