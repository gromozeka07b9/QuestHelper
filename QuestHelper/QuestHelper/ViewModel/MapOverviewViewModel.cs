using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
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
using QuestHelper.Model;
using Xamarin.Forms.Maps;
using Plugin.Geolocator.Abstractions;
using Position = Xamarin.Forms.Maps.Position;
using QuestHelper.WS;
using System.IO;
using System.Threading;
using QuestHelper.Resources;
using Xamarin.Essentials;
using QuestHelper.Managers.Sync;
using QuestHelper.Model.Messages;
using QuestHelper.Consts;
using System.Net;

namespace QuestHelper.ViewModel
{
    public class MapOverviewViewModel : INotifyPropertyChanged, IDialogEvents
    {
        RoutePointManager _routePointManager;
        RouteManager _routeManager;
        GeolocatorManager _geolocatorManager = new GeolocatorManager();
        TokenStoreService _tokenService = new TokenStoreService();
        Position _currentLocation;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private List<ViewPoi> _pois = new List<ViewPoi>();
        private bool _isLoadingPoi;
        private bool _isPoiDialogVisible;
        private string _currentPoiName = string.Empty;
        private string _currentPoiImage = string.Empty;
        private bool _isShowingUser;
        private string _сurrentPoiCreatorName;
        private string _currentPoiDescription;
        private ViewPoi _currentViewPoi;
        private bool _isPoisLoaded;
        private string _currentPoiCreatorImg;
        private int _poiImageHeight;
        private int _poiImageWidth;

        public ICommand UpdatePOIsCommand { get; private set; }
        public ICommand HidePoiDialogCommand { get; private set; }
        public ICommand StartShowAlbumCommand { get; private set; }
        public ICommand RequestToLocationAccessCommand { get; private set; }
        


        public MapOverviewViewModel()
        {
            UpdatePOIsCommand = new Command(updatePOIsCommand);
            HidePoiDialogCommand = new Command(hidePoiDialogCommand);
            StartShowAlbumCommand = new Command(startShowAlbumCommand);
            RequestToLocationAccessCommand = new Command(requestToLocationAccessCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
            PoiImageWidth = Convert.ToInt32(DeviceSize.FullScreenWidth * 0.9);
            PoiImageHeight = Convert.ToInt32(DeviceSize.FullScreenHeight * 0.5);
        }

        private async void requestToLocationAccessCommand(object obj)
        {
            PermissionManager permissions = new PermissionManager();
            //await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
            await permissions.PermissionLocationGrantedAsync(CommonResource.Permission_Position);
        }

        private async void startShowAlbumCommand(object obj)
        {
            if (!string.IsNullOrEmpty(_currentViewPoi?.ByRouteId))
            {
                var routesApi = new RoutesApiRequest(DefaultUrls.ApiUrl, await _tokenService.GetAuthTokenAsync());
                var routeRoot = await routesApi.GetRouteRoot(_currentViewPoi?.ByRouteId);
                if((routesApi.GetLastHttpStatusCode() == HttpStatusCode.OK) && (!string.IsNullOrEmpty(routeRoot.Route.Id)))
                {
                    var vRoute = new ViewRoute(_currentViewPoi.ByRouteId);
                    if (!string.IsNullOrEmpty(routeRoot.Route.Id))
                    {
                        //Может быть неопубликованный маршрут, все-таки покажем его
                        vRoute.FillFromWSModel(routeRoot, string.Empty);
                    }
                    if (!string.IsNullOrEmpty(routeRoot.Route.ImgFilename))
                    {
                        bool resultDownloadCover = await routesApi.DownloadCoverImage(_currentViewPoi?.ByRouteId, routeRoot.Route.ImgFilename);
                    }
                    else
                    {
                        vRoute.ImgFilename = _currentPoiImage;
                    }
                    await Navigation.PushModalAsync(new RouteCoverPage(vRoute));
                }
            }
        }

        private void hidePoiDialogCommand(object obj)
        {
            IsPoiDialogVisible = false;
            _currentViewPoi = null;
        }

        private async void updatePOIsCommand(object obj)
        {
            await refreshPoisAsync();
        }

        public async void StartDialog()
        {
            IsPoisLoaded = _pois.Any();
            PermissionManager permissions = new PermissionManager();
            IsShowingUser = await permissions.PermissionGetCoordsGrantedAsync();
            
            if(IsShowingUser && (CurrentLocation.Latitude == 0) && (CurrentLocation.Longitude == 0))
            {
                await updateLocationAsync();
            }
            else
            {
                if(!IsShowingUser)
                {
                    CurrentLocation = new Position(0, 0);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentLocation"));
                }
            }

        }

        private async Task updateLocationAsync()
        {
            var cacheCoordinates = await _geolocatorManager.GetLastKnownPosition();
            if ((cacheCoordinates.Latitude != 0) && (cacheCoordinates.Longtitude != 0))
            {
                CurrentLocation = new Position(cacheCoordinates.Latitude, cacheCoordinates.Longtitude);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentLocation"));
            }

            var coordinates = await _geolocatorManager.GetCurrentLocationAsync();
            if ((coordinates.Latitude != 0) && (coordinates.Longtitude != 0))
            {
                CurrentLocation = new Position(coordinates.Latitude, coordinates.Longtitude);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentLocation"));
            }
        }

        public void CloseDialog()
        {
        }

        public async void SelectedPin(string poiId)
        {
            IsPoiDialogVisible = true;
            var poi = _pois.Single(p => p.Id.Equals(poiId));
            _currentViewPoi = poi;
            CurrentPoiName = poi.Name;
            CurrentPoiImage = Path.Combine(ImagePathManager.GetPicturesDirectory(), poi.ImgFilename);
            CurrentPoiDescription = poi.Description;
            if (!string.IsNullOrEmpty(_currentViewPoi?.ByRouteId))
            {
                var creator = new ViewUserInfo();
                creator.Load(_currentViewPoi.CreatorId);
                CurrentPoiCreatorName = creator?.Name;
                CurrentPoiCreatorImg = creator?.ImgUrl;
                if (string.IsNullOrEmpty(CurrentPoiCreatorName))
                {
                    if (await creator.UpdateFromServerAsync())
                    {
                        CurrentPoiCreatorName = creator.Name;
                        CurrentPoiCreatorImg = creator.ImgUrl;
                    }
                }
            }
        }

        private async Task refreshPoisAsync()
        {
            string token = await _tokenService.GetAuthTokenAsync();
            string userId = await _tokenService.GetUserIdAsync();
            if (!string.IsNullOrEmpty(token))
            {
                IsPoisLoaded = false;
                IsLoadingPoi = true;
                PoiApiRequest poiApi = new PoiApiRequest(token);
                var pois = await poiApi.GetMyPoisAsync();
                PoiManager poiManager = new PoiManager();
                pois.ForEach(p =>
                {
                    ViewPoi poi = new ViewPoi(p);
                    poi.Save();
                });
                _pois = poiManager.GetAllAvailablePois(userId);
                _pois.AsParallel().ForAll(async p => await downloadPoiImgAsync(poiApi, p));
                //IsLoadingPoi = false;
                //IsPoisLoaded = _pois.Any();
                Device.StartTimer(TimeSpan.FromMilliseconds(10), onTimerForUpdate);
            }
        }

        private bool onTimerForUpdate()
        {
            MainThread.BeginInvokeOnMainThread(() => {
                IsPoisLoaded = !IsPoisLoaded;
                IsLoadingPoi = !IsLoadingPoi;
            });
            return false;
        }

        private async Task<bool> downloadPoiImgAsync(PoiApiRequest api, ViewPoi viewPoi)
        {
            bool result = false;
            string pathToImg = Path.Combine(ImagePathManager.GetPicturesDirectory(), viewPoi.ImgFilename);
            if (!File.Exists(pathToImg))
            {
                result = await api.DownloadImg(viewPoi.Id, pathToImg);
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
            });
            return result;
        }

        public List<ViewPoi> POIs
        {
            get
            {
                return _pois;
            }
        }

        public string CurrentPoiCreatorImg
        {
            get
            {
                return string.IsNullOrEmpty(_currentPoiCreatorImg) ? "avatar1.png": _currentPoiCreatorImg;
            }
            set
            {
                if (!value.Equals(_currentPoiCreatorImg))
                {
                    _currentPoiCreatorImg = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPoiCreatorImg"));
                }
            }
        }

        public string CurrentPoiCreatorName
        {
            get
            {
                return _сurrentPoiCreatorName;
            }
            set
            {
                if (!value.Equals(_сurrentPoiCreatorName))
                {
                    _сurrentPoiCreatorName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPoiCreatorName"));
                }
            }
        }

        public string CurrentPoiName
        {
            get
            {
                return _currentPoiName;
            }
            set
            {
                if (!value.Equals(_currentPoiName))
                {
                    _currentPoiName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPoiName"));
                }
            }
        }

        public string CurrentPoiDescription
        {
            get
            {
                return _currentPoiDescription;
            }
            set
            {
                if (!value.Equals(_currentPoiDescription))
                {
                    _currentPoiDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPoiDescription"));
                }
            }
        }

        public string CurrentPoiImage
        {
            get
            {
                return _currentPoiImage;
            }
            set
            {
                if (!value.Equals(_currentPoiImage))
                {
                    _currentPoiImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPoiImage"));
                }
            }
        }
        public int PoiImageWidth
        {
            get
            {
                return _poiImageWidth;
            }
            set
            {
                if (!value.Equals(_poiImageWidth))
                {
                    _poiImageWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PoiImageWidth"));
                }
            }
        }
        public int PoiImageHeight
        {
            get
            {
                return _poiImageHeight;
            }
            set
            {
                if (!value.Equals(_poiImageHeight))
                {
                    _poiImageHeight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PoiImageHeight"));
                }
            }
        }

        public bool IsShowingUser 
        {
            get 
            {
                return _isShowingUser;
            }

            set 
            {
                if (value != _isShowingUser)
                {
                    _isShowingUser = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsShowingUser"));
                }
            }
        }

        
        public bool IsPoisLoaded
        {
            get
            {
                return _isPoisLoaded;
            }

            set
            {
                if (value != _isPoisLoaded)
                {
                    _isPoisLoaded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPoisLoaded"));
                }
            }
        }
        public bool IsLoadingPoi
        {
            get
            {
                return _isLoadingPoi;
            }

            set
            {
                if (value != _isLoadingPoi)
                {
                    _isLoadingPoi = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoadingPoi"));
                }
            }
        }

        public bool IsPoiDialogVisible
        {
            get
            {
                return _isPoiDialogVisible;
            }

            set
            {
                if (value != _isPoiDialogVisible)
                {
                    _isPoiDialogVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPoiDialogVisible"));
                }
            }
        }

        public Position CurrentLocation { get => _currentLocation; set => _currentLocation = value; }

    }
}
