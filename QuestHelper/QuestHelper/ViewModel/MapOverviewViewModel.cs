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

namespace QuestHelper.ViewModel
{
    public class MapOverviewViewModel : INotifyPropertyChanged, IDialogEvents
    {
        RoutePointManager _routePointManager;
        RouteManager _routeManager;
        GeolocatorManager _geolocatorManager = new GeolocatorManager();
        Position _currentLocation;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        //public ICommand OpenPointPropertiesCommand { get; private set; }
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

        public ICommand UpdatePOIsCommand { get; private set; }
        public ICommand HidePoiDialogCommand { get; private set; }
        public ICommand StartShowAlbumCommand { get; private set; }


        public MapOverviewViewModel()
        {
            UpdatePOIsCommand = new Command(updatePOIsCommand);
            HidePoiDialogCommand = new Command(hidePoiDialogCommand);
            StartShowAlbumCommand = new Command(startShowAlbumCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
        }

        private void startShowAlbumCommand(object obj)
        {
            //var viewPoint = new ViewRoutePoint();
            //viewPoint.Refresh(_currentViewPoi.ByRoutePointId);
            RoutePointManager manager = new RoutePointManager();
            var point = manager.RealmInstance.All<RoutePoint>().Where(p => p.RoutePointId.Equals(_currentViewPoi.ByRoutePointId)).SingleOrDefault();
            if(point != null)
            {
                Navigation.PushModalAsync(new RouteCoverPage(new ViewRoute(point.RouteId)));
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
            IsPoiDialogVisible = false;
            IsPoisLoaded = _pois.Any();
            PermissionManager permissions = new PermissionManager();
            IsShowingUser = await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
            /*await updateLocationAsync();*/
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
            CurrentPoiCreatorName = poi.CreatorId;
            CurrentPoiDescription = poi.Description;
        }

        private async Task refreshPoisAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string token = await tokenService.GetAuthTokenAsync();
            string userId = await tokenService.GetUserIdAsync();
            if (!string.IsNullOrEmpty(token))
            {
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
                _pois.ForEach(async p  => await downloadPoiImgAsync(poiApi, p));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
                IsLoadingPoi = false;
                IsPoisLoaded = _pois.Any();

            }
        }

        private async Task<bool> downloadPoiImgAsync(PoiApiRequest api, ViewPoi viewPoi)
        {
            bool result = false;
            string pathToImg = Path.Combine(ImagePathManager.GetPicturesDirectory(), viewPoi.ImgFilename);
            if (!File.Exists(pathToImg))
            {
                result = await api.DownloadImg(viewPoi.Id, pathToImg);
            }
            return result;
        }

        public List<ViewPoi> POIs
        {
            get
            {
                return _pois;
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
