using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
using QuestHelper.View;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuestHelper.Model.Messages;
using QuestHelper.Model;
using Acr.UserDialogs;
using Xamarin.Essentials;

namespace QuestHelper.ViewModel
{
    class RoutesViewModel : INotifyPropertyChanged
    {
        private string _routeId;
        private IEnumerable<ViewRoute> _routes;
        private ViewRoute _routeItem;
        private RouteManager _routeManager = new RouteManager();

        //private RoutesApiRequest _api = new RoutesApiRequest("http://questhelperserver.azurewebsites.net");
        private bool _noRoutesWarningIsVisible = false;
        private bool _isRefreshing = false;
        private int _countOfUpdateListByTimer = 0;
        private ShareFromGoogleMapsMessage _sharePointMessage;
        private bool _syncProgressIsVisible = false;
        private string _syncProgressDetailText = string.Empty;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddNewRouteCommand { get; private set; }
        public ICommand RefreshListRoutesCommand { get; private set; }
        public ICommand SyncStartCommand { get; private set; }

        public RoutesViewModel()
        {
            AddNewRouteCommand = new Command(addNewRouteCommandAsync);
            RefreshListRoutesCommand = new Command(refreshListRoutesCommand);
            SyncStartCommand = new Command(syncStartCommand);
        }
        public void startDialog()
        {
            MessagingCenter.Subscribe<SyncProgressMessage>(this, string.Empty, (sender) =>
                {
                    SyncProgressIsVisible = sender.SyncInProgress;
                    SyncProgressDetailText = sender.SyncDetailText;
                });
        }
        internal void closeDialog()
        {
            MessagingCenter.Unsubscribe<SyncProgressMessage>(this, string.Empty);
        }

        internal void AddSharedPoint(ShareFromGoogleMapsMessage msg)
        {
            _sharePointMessage = msg;
        }

        void refreshListRoutesCommand()
        {
            IsRefreshing = true;
            Routes = _routeManager.GetRoutes();
            if (Routes.Count() == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), OnTimerForUpdate);
            }
            NoRoutesWarningIsVisible = Routes.Count() == 0;
            IsRefreshing = false;
        }

        private bool OnTimerForUpdate()
        {
            bool continueTimer = true;
            _countOfUpdateListByTimer++;
            if ((_countOfUpdateListByTimer > 10)||(Routes.Count() > 0))
            {
                continueTimer = false;
                _countOfUpdateListByTimer = 0;
            }
            else
            {
                refreshListRoutesCommand();
            }
            return continueTimer;
        }

        async void addNewRouteCommandAsync()
        {
            await Navigation.PushAsync(new NewRoutePage(!Routes.Any()));
        }
        private void syncStartCommand(object obj)
        {
            Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
        }

        public bool SyncProgressIsVisible
        {
            get
            {
                return _syncProgressIsVisible;
            }
            set
            {
                if (_syncProgressIsVisible != value)
                {
                    _syncProgressIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SyncProgressIsVisible"));
                    }
                }
            }
        }
        public string SyncProgressDetailText
        {
            get
            {
                return _syncProgressDetailText;
            }
            set
            {
                if (_syncProgressDetailText != value)
                {
                    _syncProgressDetailText = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SyncProgressDetailText"));
                    }
                }
            }
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
        public bool NoRoutesWarningIsVisible
        {
            set
            {
                if (_noRoutesWarningIsVisible != value)
                {
                    _noRoutesWarningIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoRoutesWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noRoutesWarningIsVisible;
            }
        }
        public string RouteId
        {
            set
            {
                if (_routeId != value)
                {
                    _routeId = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RouteId"));
                    }
                }
            }
            get
            {
                return _routeId;
            }
        }
        public ViewRoute SelectedRouteItem
        {
            set
            {
                if (_routeItem != value)
                {
                    _routeItem = value;

                    var routePage = new RoutePage(value.RouteId, false);
                    Navigation.PushAsync(routePage);
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRouteItem"));
                    addNewPointFromShareAsync(_routeItem.Name);
                    _routeItem = null;
                }
            }
        }

        private async void addNewPointFromShareAsync(string routeName)
        {
            if (_sharePointMessage != null)
            {
                ViewRoutePoint newPoint = new ViewRoutePoint(_routeItem.RouteId, string.Empty);
                newPoint.Version++;
                if (string.IsNullOrEmpty(_sharePointMessage.Subject))
                {
                    string name = _sharePointMessage.Description.Substring(0,15);
                    if (_sharePointMessage.Description.Length > 15)
                    {
                        name += "...";
                    }
                    newPoint.Name = name;
                }
                else
                {
                    newPoint.Name = _sharePointMessage.Subject;
                }
                newPoint.Description = _sharePointMessage.Description;
                CustomGeocoding geo = new CustomGeocoding(newPoint.Description);
                if (await geo.GetCoordinatesAsync())
                {
                    newPoint.Longitude = geo.Longtitude;
                    newPoint.Latitude = geo.Latitude;
                }
                if (newPoint.Save())
                {
                    _sharePointMessage = null;
                    UserDialogs.Instance.Alert($"Новая точка '{newPoint.Name}' в маршрут '{routeName}' добавлена", "Добавление точки");
                }
            }
        }

        public IEnumerable<ViewRoute> Routes
        {
            set
            {
                if (_routes != value)
                {
                    _routes = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Routes"));
                        NoRoutesWarningIsVisible = _routes.Count() > 0;
                    }
                }
            }
            get
            {
                return _routes;
            }
        }
    }
}
