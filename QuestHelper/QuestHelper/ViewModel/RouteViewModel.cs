using Plugin.Geolocator;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
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
using QuestHelper.Model.WS;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;
using Point = System.Drawing.Point;
using Acr.UserDialogs;

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
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
        private bool _listIsRefreshing;
        private bool _noPointWarningIsVisible;
        private bool _isRefreshing;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShowNewRouteDialogCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StartDialogCommand { get; private set; }

        public ICommand EditRouteCommand { get; private set; }
        public ICommand AddPersonCommand { get; private set; }
        public ICommand ShareRouteCommand { get; private set; }
        public ICommand FullScreenMapCommand { get; private set; }
        public ICommand PhotoAlbumCommand { get; private set; }
        
        public RouteViewModel(string routeId, bool isFirstRoute)
        {
            _vroute = new ViewRoute(routeId);
            _isFirstRoute = isFirstRoute;
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            AddNewRoutePointCommand = new Command(addNewRoutePointAsync);
            StartDialogCommand = new Command(startDialog);
            EditRouteCommand = new Command(editRouteCommandAsync);
            AddPersonCommand = new Command(addPersonCommandAsync);
            ShareRouteCommand = new Command(shareRouteCommandAsync);
            FullScreenMapCommand = new Command(fullScreenMapCommandAsync);
            PhotoAlbumCommand = new Command(photoAlbumCommandAsync);
        }

        private void photoAlbumCommandAsync(object obj)
        {
            //var page = new RoutePointCarouselPage(_vroute.RouteId, string.Empty);
            var page = new RouteCarouselRootPage(_vroute.RouteId);
            Navigation.PushAsync(page);
        }

        private void shareRouteCommandAsyncOLD(object obj)
        {
            var points = _routePointManager.GetPointsByRouteId(_vroute.RouteId);
            if (points.Any())
            {
                ViewRoutePoint vp = new ViewRoutePoint(_vroute.RouteId,points.First().RoutePointId);
                var instagramShareService = DependencyService.Get<IInstagramShareService>();
                instagramShareService.Share(vp.ImagePath);

                /*Share.RequestAsync(new ShareTextRequest()
                {
                    //Text = $"Маршрут:{_vroute.Name}",
                    Text = (new Uri($"{vp.ImagePath}")).AbsoluteUri,
                    //Text = "http://mediad.publicbroadcasting.net/p/wunc/files/styles/x_large/public/201705/standardized_test.jpg",
                    Subject = "Название маршрута из GoSh!"
                });*/

            }
        }
        private async void addPersonCommandAsync(object obj)
        {
            var shareRoutePage = new ShareRoutePage(_vroute.RouteId);
            await Navigation.PushAsync(shareRoutePage, true);
        }
        internal async Task<bool> UserCanShareAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string userId = await tokenService.GetUserIdAsync();
            return userId.Equals(_vroute.CreatorId);
        }
        private async void shareRouteCommandAsync(object obj)
        {
            bool answerYesIsNo = await Application.Current.MainPage.DisplayAlert("Внимание", "После публикации маршрут будет доступен всем пользователям в альбоме. Вы уверены?", "Нет", "Да");
            if (!answerYesIsNo) //порядок кнопок - хардкод, и непонятно, почему именно такой
            {
                if (await UserCanShareAsync())
                {
                    _vroute.IsPublished = true;
                    _vroute.Version++;
                    _vroute.Save();
                    await Application.Current.MainPage.DisplayAlert("Внимание!", "После синхронизации маршрут будет опубликован", "Ok");
                    Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { ShowErrorMessageIfExist = false }, string.Empty);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Внимание!", "Публиковать можно только созданные вами маршруты", "Ok");
                }
            }
        }

        private void fullScreenMapCommandAsync(object obj)
        {
            var mapRoutePage = new MapRouteOverviewPage(_vroute.RouteId);
            Navigation.PushAsync(mapRoutePage, true);
        }

        private void editRouteCommandAsync(object obj)
        {
        }

        public void startDialog()
        {
            /*MessagingCenter.Subscribe<ShareFromGoogleMapsMessage>(this, string.Empty, (sender) =>
            {
                ViewRoutePoint _vpoint = new ViewRoutePoint(_vroute.RouteId, string.Empty);
                _vpoint.Name = sender.Subject;
                _vpoint.Description = sender.Description;
                if (_vpoint.Save())
                {
                    UserDialogs.Instance.Alert($"В маршрут '{_vroute.Name}' успешно добавлена новая точка", "Создание новой точки");
                }
                UserDialogs.Instance.Alert($"В маршрут '{_vroute.Name}' успешно добавлена новая точка", "Создание новой точки");
            });*/

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
        internal void closeDialog()
        {
        }

        private void showNewRouteWarningDialog()
        {
            SplashStartScreenIsVisible = true;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
        }

        private void refreshRouteData()
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
                    /*var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _vroute.Name = value;
                    });*/
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _vroute.Name;
            }
        }
        /*public string RouteLength
        {
            set
            {
                if (_routeLength != value)
                {
                    _routeLength = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RouteLength"));
                }
            }
            get
            {
                return _routeLength;
            }
        }
        public string RouteLengthSteps
        {
            set
            {
                if (_routeLengthSteps != value)
                {
                    _routeLengthSteps = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RouteLengthSteps"));
                }
            }
            get
            {
                return _routeLengthSteps;
            }
        }
        public int CountOfPhotos
        {
            set
            {
                if (_countOfPhotos != value)
                {
                    _countOfPhotos = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotos"));
                }
            }
            get
            {
                return _countOfPhotos;
            }
        }
        public int CountOfPoints
        {
            set
            {
                if (_countOfPoints != value)
                {
                    _countOfPoints = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPoints"));
                }
            }
            get
            {
                return _countOfPoints;
            }
        }*/

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
