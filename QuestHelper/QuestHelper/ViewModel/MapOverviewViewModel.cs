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

        public ICommand UpdatePOIsCommand { get; private set; }

        public MapOverviewViewModel()
        {
            UpdatePOIsCommand = new Command(updatePOIsCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
        }

        private async void updatePOIsCommand(object obj)
        {
            await refreshPoisAsync();
        }

        public async void StartDialog()
        {
            PermissionManager permissions = new PermissionManager();
            IsShowingUser = await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
            IsPoiDialogVisible = false;
            await updateLocationAsync();
            if (!_pois.Any())
            {
                await refreshPoisAsync();
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
            CurrentPoiName = poi.Name;
            CurrentPoiImage = Path.Combine(ImagePathManager.GetPicturesDirectory(), poi.ImgFilename);
            /*TokenStoreService tokenService = new TokenStoreService();
            string token = await tokenService.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                PoiApiRequest poiApi = new PoiApiRequest(token);
                var pois = await poiApi.GetMyPoisAsync();
                _pois = pois.Select(p => new ViewPoi(p)).ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
                var coverPage = new RouteCoverPage(value);
                Navigation.PushModalAsync(coverPage);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRouteItem"));
                if (!_feedItem.IsUserViewed)
                {
                    Xamarin.Forms.MessagingCenter.Send<AddRouteViewedMessage>(new AddRouteViewedMessage() { RouteId = _feedItem.Id }, string.Empty);
                    Analytics.TrackEvent($"Set route viewed");
                }
            }*/
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
                IsLoadingPoi = false;
            }
        }

        public List<ViewPoi> POIs
        {
            get
            {
                return _pois;
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
