using QuestHelper.Managers;
using QuestHelper.View;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows.Input;
using Xamarin.Forms;
using QuestHelper.Model.Messages;
using QuestHelper.Model;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Autofac;
using QuestHelper.Managers.Sync;
using QuestHelper.Resources;
using QuestHelper.SharedModelsWS;
using Xamarin.Essentials;

namespace QuestHelper.ViewModel
{
    class RoutesViewModel : INotifyPropertyChanged
    {
        private string _routeId;
        private ObservableCollection<ViewRoute> _routes;
        private ViewRoute _routeItem;
        private RouteManager _routeManager = new RouteManager();
        private IMediaFileManager _mediaFileManager = App.Container.Resolve<IMediaFileManager>();

        private bool _noRoutesWarningIsVisible = false;
        private bool _isRefreshing = false;
        //private bool _isVisibleProgress = false;
        private bool _isFireworksMode = false;
        private int _countOfUpdateListByTimer = 0;
        private ShareFromGoogleMapsMessage _sharePointMessage;
        //private bool _syncProgressIsVisible = false;
        private string _syncProgressDetailText = string.Empty;
        private string _currentUserId = string.Empty;
        private string _currentUserToken = string.Empty;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _userRole;
        private string _userImgUrl;
        private double _progressValue = 0;
        private int _countRoutesCreatedMe = 0;
        private int _countRoutesPublishedMe = 0;
        private int _countLikesMe = 0;
        private int _countViewsMe = 0;
        private int maxPageSize = 1000;//Сколько максимально элементов маршрутов можем получить с сервера. Актуально если пейджинг используется, но пока что нет.
        private bool _isServerRequestsOk = false;
        private List<Route> _serverRoutes = new List<Route>();


        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddNewRouteCommand { get; private set; }
        public ICommand RefreshListRoutesCommand { get; private set; }
        //public ICommand SyncStartCommand { get; private set; }//Lottie не работает с MVVM, пришлось из формы запускать
        public ICommand AuthorizationCommand { get; private set; }

        public RoutesViewModel()
        {
            AddNewRouteCommand = new Command(addNewRouteCommandAsync);
            RefreshListRoutesCommand = new Command(refreshListRoutesCommandAsync);
            AuthorizationCommand = new Command(authorizationCommand);

        }

        private void authorizationCommand(object obj)
        {
            Navigation.PushModalAsync(new LoginPage());
        }

        public async void startDialog()
        {
            MessagingCenter.Subscribe<SyncProgressRouteLoadingMessage>(this, string.Empty, (sender) =>
            {
                if (string.IsNullOrEmpty(sender.RouteId))
                {
                    /*if (!IsVisibleProgress) IsVisibleProgress = true;
                    ProgressValue = sender.ProgressValue;
                    refreshListRoutesCommandAsync();*/
                }
            });

            MessagingCenter.Subscribe<SyncRouteCompleteMessage>(this, string.Empty, (sender) =>
            {
                if (string.IsNullOrEmpty(sender.RouteId))
                {
                    //if (IsVisibleProgress) IsVisibleProgress = false;
                    //StopAnimateCallback.Invoke();
                }
            });
            MessagingCenter.Subscribe<AuthResultMessage>(this, string.Empty, (sender) =>
            {
                if (sender.IsAuthenticated)
                {
                    Task taskInnerUpdate = Task.Run(updateUserInfo);
                    taskInnerUpdate.Wait();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAutorizedMode"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Username"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Email"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserImgUrl"));
                    if(AuthenticateOnly) Navigation.PopAsync();
                }
            });
            await updateUserInfo();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAutorizedMode"));
        }

        private async Task updateUserInfo()
        {
            TokenStoreService tokenService = new TokenStoreService();
            _currentUserId = await tokenService.GetUserIdAsync();
            _currentUserToken = await tokenService.GetAuthTokenAsync();
            Username = await tokenService.GetUsernameAsync();
            Email = await tokenService.GetEmailAsync();
            UserRole = await tokenService.GetRoleAsync();
            UserImgUrl = await tokenService.GetImgUrlAsync();
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
            MessagingCenter.Unsubscribe<AuthResultMessage>(this, string.Empty);
        }

        internal void AddSharedPoint(ShareFromGoogleMapsMessage msg)
        {
            _sharePointMessage = msg;
        }

        private async void refreshListRoutesCommandAsync()
        {
            NoRoutesWarningIsVisible = !Routes?.Any()??false;
            IsRefreshing = true;
            RoutesApiRequest api = new RoutesApiRequest(_currentUserToken);
            await api.GetPrivateRoutes(maxPageSize, 0, maxPageSize).ContinueWith(requestResult =>
            {
                if (api.LastHttpStatusCode == HttpStatusCode.OK)
                {
                    _serverRoutes = requestResult.Result;
                    _routeManager.MergeRoutes(_currentUserId, _serverRoutes);
                }    
                Routes = getViewStateRoute(_routeManager.GetRoutes(_currentUserId), _serverRoutes, api.LastHttpStatusCode == HttpStatusCode.OK);
            }).ContinueWith(async (result) =>
            {
                IsRefreshing = false;
                if (Routes.Count > 0)
                {
                    var routeWithoutImg = Routes.Where(r => !string.IsNullOrEmpty(r.ImgFilename) && !fileExist(r.ImgFilename)).Select(r=>r.RouteId);
                    await api.DownloadRoutesCovers(routeWithoutImg).ContinueWith((resultDownloadCovers) =>
                    {
                        CountRoutesCreatedMe = _routeManager.GetCountRoutesByCreator(_currentUserId);
                        CountRoutesPublishedMe = _routeManager.GetCountPublishedRoutesByCreator(_currentUserId);
                        NoRoutesWarningIsVisible = !Routes.Any();
                    });
                }
            
                CountRoutesCreatedMe = _routeManager.GetCountRoutesByCreator(_currentUserId);
                CountRoutesPublishedMe = _routeManager.GetCountPublishedRoutesByCreator(_currentUserId);
                NoRoutesWarningIsVisible = !Routes.Any();
            });
        }

        //Определяет тип состояния для отображения
        //Могут быть варианты:
        //На сервере есть, локально нет
        //На сервере есть, локально есть, версии разные
        //На сервере есть, локально есть, версия одна
        //Нет сети
        private ObservableCollection<ViewRoute> getViewStateRoute(ObservableCollection<ViewRoute> localRoutes, List<Route> serverRoutes, bool IsServerRequestOk)
        {
            foreach (var route in localRoutes)
            {
                if (IsServerRequestOk)
                {
                    var serverRoute = serverRoutes.Where(r => r.Id.Equals(route.Id))?.FirstOrDefault();
                    if (serverRoute != null)
                    {
                        if (serverRoute.VersionsHash.Equals(route.ObjVerHash))
                        {
                            route.SetViewStateSynced();
                        }
                        else
                        {
                            if (route.Version == 0 && !route.ServerSynced && string.IsNullOrEmpty(route.ObjVerHash))
                            {
                                route.SetViewStateNeedLoad();
                            } else if (route.Version < serverRoute.Version)
                            {
                                route.SetViewStateNeedLoad();
                            }
                            else
                            {
                                route.SetViewStateNeedSync();
                            }
                        }
                    }
                    else
                    {
                        route.SetViewStateNeedSync();
                    }
                }
                else
                {
                    route.SetViewStateNoNetwork();
                }
            }

            return localRoutes;
        }

        private bool fileExist(string imgFilename)
        {
            return _mediaFileManager.FileExistInMediaCatalog(imgFilename);
        }

        async void addNewRouteCommandAsync()
        {
            await Navigation.PushAsync(new NewRoutePage(!Routes.Any()));
        }

        public bool AuthenticateOnly { get; set; }

        public double ProgressValue
        {
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressValue"));
                }
            }
            get
            {
                return _progressValue;
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
                bool authorizedMode = false;
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
        public string Username
        {
            set
            {
                if (_username != value)
                {
                    _username = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Username"));
                }
            }
            get
            {
                return !string.IsNullOrEmpty(_username) ? _username: "";
            }
        }

        public string Email
        {
            set
            {
                if (_email != value)
                {
                    _email = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Email"));
                }
            }
            get
            {
                return !string.IsNullOrEmpty(_email) ? _email : "";
            }
        }
        public string UserRole
        {
            set
            {
                if (_userRole != value)
                {
                    _userRole = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UserRole"));
                }
            }
            get
            {
                return _userRole;
            }
        }

        public string UserImgUrl
        {
            set
            {
                if (_userImgUrl != value)
                {
                    _userImgUrl = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UserImgUrl"));
                }
            }
            get
            {
                return !string.IsNullOrEmpty(_userImgUrl) ? _userImgUrl : "avatar1.png";
            }
        }

        public ViewRoute SelectedRouteItem
        {
            set
            {
                _routeItem = value;
                if (value.IsShowNeedLoadRoute)
                {
                    value.SetViewStateLoadingProcess();
                    string routeId = value.Id;
                    SyncServer syncSrv = new SyncServer();
                    Task.Factory.StartNew(async () =>
                    {
                        await syncSrv.Sync(value.RouteId, false).ContinueWith(syncResult =>
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                if (!syncResult.Result)
                                {
                                    if(value.IsNeedSyncRoute)
                                        value.SetViewStateNeedSync();
                                    else value.SetViewStateNeedLoad();
                                    UserDialogs.Instance.Alert("Sync error, check network", "Warning", "Ok");
                                }
                                else
                                {
                                    value.SetViewStateSynced();
                                    var routePage = new RoutePage(value.RouteId, false, false);
                                    Navigation.PushModalAsync(routePage);
                                }
                            });

                        }, TaskContinuationOptions.OnlyOnRanToCompletion);
                    });
                }
                else
                {
                    var routePage = new RoutePage(value.RouteId, false, value.IsNeedSyncRoute);
                    Navigation.PushModalAsync(routePage);
                }

                //addNewPointFromShareAsync(value.Name);
            }
        }

        public int CountRoutesCreatedMe
        {
            set
            {
                if (_countRoutesCreatedMe != value)
                {
                    _countRoutesCreatedMe = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CountRoutesCreatedMe"));
                }
            }
            get
            {
                return _countRoutesCreatedMe;
            }
        }

        public int CountRoutesPublishedMe
        {
            set
            {
                if (_countRoutesPublishedMe != value)
                {
                    _countRoutesPublishedMe = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CountRoutesPublishedMe"));
                }
            }
            get
            {
                return _countRoutesPublishedMe;
            }
        }

        public int CountLikesMe
        {
            set
            {
                if (_countLikesMe != value)
                {
                    _countLikesMe = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CountLikesMe"));
                }
            }
            get
            {
                return _countLikesMe;
            }
        }

        public int CountViewsMe
        {
            set
            {
                if (_countViewsMe != value)
                {
                    _countViewsMe = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CountViewsMe"));
                }
            }
            get
            {
                return _countViewsMe;
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

        public ObservableCollection<ViewRoute> Routes
        {
            set
            {
                if (_routes != value)
                {
                    _routes = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Routes"));
                        NoRoutesWarningIsVisible = !_routes.Any();
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
