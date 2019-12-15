using QuestHelper.Managers;
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

        private bool _noRoutesWarningIsVisible = false;
        private bool _isRefreshing = false;
        private bool _isVisibleProgress = false;
        private bool _isFireworksMode = false;
        private int _countOfUpdateListByTimer = 0;
        private ShareFromGoogleMapsMessage _sharePointMessage;
        private bool _syncProgressIsVisible = false;
        private string _syncProgressDetailText = string.Empty;
        private double _progressValue = 0;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddNewRouteCommand { get; private set; }
        public ICommand RefreshListRoutesCommand { get; private set; }
        public ICommand SyncStartCommand { get; private set; }
        public ICommand AuthorizationCommand { get; private set; }

        public RoutesViewModel()
        {
            AddNewRouteCommand = new Command(addNewRouteCommandAsync);
            RefreshListRoutesCommand = new Command(refreshListRoutesCommandAsync);
            SyncStartCommand = new Command(syncStartCommand);
            AuthorizationCommand = new Command(authorizationCommand);
        }

        private void authorizationCommand(object obj)
        {
            Navigation.PushAsync(new LoginPage());
        }

        public void startDialog()
        {
            MessagingCenter.Subscribe<SyncProgressRouteLoadingMessage>(this, string.Empty, (sender) =>
            {
                if (string.IsNullOrEmpty(sender.RouteId))
                {
                    if (!IsVisibleProgress) IsVisibleProgress = true;
                    ProgressValue = sender.ProgressValue;
                    refreshListRoutesCommandAsync();
                }
            });

            MessagingCenter.Subscribe<SyncRouteCompleteMessage>(this, string.Empty, (sender) =>
            {
                if (string.IsNullOrEmpty(sender.RouteId))
                {
                    if (IsVisibleProgress) IsVisibleProgress = false;
                }
            });

        }

        private bool OnTimerForUpdateFireworks()
        {
            IsFireworksMode = false;
            return false;
        }

        internal void closeDialog()
        {
            MessagingCenter.Unsubscribe<SyncProgressRouteLoadingMessage>(this, string.Empty);
            MessagingCenter.Unsubscribe<SyncRouteCompleteMessage>(this, string.Empty);
        }

        internal void AddSharedPoint(ShareFromGoogleMapsMessage msg)
        {
            _sharePointMessage = msg;
        }

        async void refreshListRoutesCommandAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string currentUserId = await tokenService.GetUserIdAsync();
            Routes = _routeManager.GetRoutes(currentUserId);
            if (Routes.Count() == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), OnTimerForUpdate);
            }
            NoRoutesWarningIsVisible = Routes.Count() == 0;
        }

        private bool OnTimerForUpdate()
        {
            _countOfUpdateListByTimer++;
            if ((_countOfUpdateListByTimer > 2)||(Routes.Count() > 0))
            {
                _countOfUpdateListByTimer = 0;
            }
            else
            {
                refreshListRoutesCommandAsync();
            }
            return false;
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

        public double ProgressValue
        {
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ProgressValue"));
                    }
                }
            }
            get
            {
                return _progressValue;
            }
        }

        public bool IsVisibleProgress
        {
            set
            {
                if (_isVisibleProgress != value)
                {
                    _isVisibleProgress = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsVisibleProgress"));
                    }
                }
            }
            get
            {
                return _isVisibleProgress;
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
                //return _isRefreshing;
                return false;
            }
        }

        public bool IsFireworksMode
        {
            set
            {
                if (_isFireworksMode != value)
                {
                    _isFireworksMode = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsFireworksMode"));
                    }
                }
            }
            get
            {
                return _isFireworksMode;
            }
        }
        public bool IsAutorizedMode
        {
            get
            {
                bool authorizedMode = true;
                ParameterManager par = new ParameterManager();
                string guestMode = string.Empty;
                if (par.Get("GuestMode", out guestMode))
                {
                    authorizedMode = !guestMode.Equals("1");
                }
                return authorizedMode;
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
                        if ((_noRoutesWarningIsVisible) && (IsAutorizedMode))
                        {
                            IsFireworksMode = true;
                            Device.StartTimer(TimeSpan.FromSeconds(10), OnTimerForUpdateFireworks);
                        }
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
                        NoRoutesWarningIsVisible = _routes.Count() == 0;
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
