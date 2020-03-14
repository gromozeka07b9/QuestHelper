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

        private async Task refreshPoisAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string token = await tokenService.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                PoiApiRequest poiApi = new PoiApiRequest(token);
                var pois = await poiApi.GetMyPoisAsync();
                _pois = pois.Select(p=>new ViewPoi(p)).ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
            }
        }

        public List<ViewPoi> POIs
        {
            get
            {
                return _pois;
            }
        }

        public Position CurrentLocation { get => _currentLocation; set => _currentLocation = value; }

    }
}
