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
        //List<ViewRoute> _routes = new List<ViewRoute>();
        //List<ViewRoutePoint> _points = new List<ViewRoutePoint>();
        private List<ViewPoi> _pois = new List<ViewPoi>();

        public MapOverviewViewModel()
        {
            //OpenPointPropertiesCommand = new Command(openPointPropertiesCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
        }

        public async void StartDialog()
        {
            //_points.Clear();
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
            
            await updateLocationAsync();
            await refreshPoisAsync();
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
                /*var lst = new ObservableCollection<ViewPoi>();

                var routess = _routeManager.GetAllRoutes();
                foreach (ViewRoute route in routess)
                {
                    var firstAndLastPoints = _routePointManager.GetFirstAndLastViewRoutePoints(route.Id);

                    var testPoi = new ViewPoi();
                    if (!string.IsNullOrEmpty(route.CoverImage))
                    {
                        testPoi.ImgFilename = route.CoverImage;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(firstAndLastPoints.Item1.ImagePreviewPath))
                        {
                            testPoi.ImgFilename = firstAndLastPoints.Item1.ImagePreviewPath;
                        }
                        else
                        {
                            testPoi.ImgFilename = firstAndLastPoints.Item2.ImagePreviewPath;
                        }
                    }
                    testPoi.Name = route.Name;
                    testPoi.Location = new Position(firstAndLastPoints.Item1.Latitude, firstAndLastPoints.Item1.Longitude);
                    lst.Add(testPoi);
                }*/

                //var pois = _points.Select(p => new POI() { Name = !string.IsNullOrEmpty(p.NameText) ? p.NameText : "Empty", Address = p.Address, Position = new Position(p.Latitude, p.Longitude), Description = p.Description, PathToPicture = p.ImagePreviewPath });
                //return new ObservableCollection<ViewPoi>(lst);
            }
        }

        public Position CurrentLocation { get => _currentLocation; set => _currentLocation = value; }

    }
}
