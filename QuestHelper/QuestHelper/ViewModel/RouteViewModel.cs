using Plugin.Geolocator;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
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
        private ObservableCollection<ViewRoutePoint> _viewPointsOfRoute = new ObservableCollection<ViewRoutePoint>();

        private bool _isFirstRoute;

        private ViewRoute _vroute;
        private ViewRoutePoint _selectedPoint;
        private RoutePointManager _routePointManager = new RoutePointManager();
        private bool _listIsRefreshing;
        private bool _noPointWarningIsVisible;
        private bool _isRefreshing;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShowNewRouteDialogCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StartDialogCommand { get; private set; }

        public RouteViewModel(string routeId, bool isFirstRoute)
        {
            _vroute = new ViewRoute();
            _vroute.Load(routeId);
            _isFirstRoute = isFirstRoute;
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            AddNewRoutePointCommand = new Command(addNewRoutePointAsync);
            StartDialogCommand = new Command(startDialog);
        }

        public void startDialog()
        {
            if (!string.IsNullOrEmpty(_vroute.Name))
            {
                refreshRouteData();
            }
            else
            {
                _vroute.Name = "Неизвестный маршрут";
                if (_isFirstRoute)
                    showNewRouteWarningDialog();
                else showNewRouteData();
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
            var points = _routePointManager.GetPointsByRouteId(_vroute.Id);
            if (!points.Any())
            {
                PointsOfRoute = new ObservableCollection<ViewRoutePoint>();
            }
            else
            {
                PointsOfRoute = new ObservableCollection<ViewRoutePoint>(points);
            }
            NoPointWarningIsVisible = PointsOfRoute.Count == 0;
            IsRefreshing = false;
        }
        void showNewRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            PointsOfRoute = new ObservableCollection<ViewRoutePoint>() { };
        }

        async void addNewRoutePointAsync()
        {
            var routePointPage = new RoutePointPage(_vroute.Id, string.Empty);
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
                if (_vroute.Name != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _vroute.Name = value;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _vroute.Name;
            }
        }

        public ViewRoutePoint SelectedRoutePointItem
        {
            set
            {
                if(_selectedPoint != value)
                {
                    ViewRoutePoint point = value;
                    var page = new RoutePointPage(_vroute.Id, point.Id);
                    Navigation.PushAsync(page);
                    _selectedPoint = null;
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
        public ObservableCollection<ViewRoutePoint> PointsOfRoute
        {
            set
            {
                if (_viewPointsOfRoute != value)
                {
                    _viewPointsOfRoute = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    }
                }
            }
            get
            {
                return _viewPointsOfRoute;
            }
        }
    }
}
