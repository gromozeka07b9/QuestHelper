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

namespace QuestHelper.ViewModel
{
    public class MapOverviewViewModel : INotifyPropertyChanged, IDialogEvents
    {
        RoutePointManager _routePointManager;
        RouteManager _routeManager;
        //IEnumerable<RoutePoint> _pointsForOverview;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        //public ICommand OpenPointPropertiesCommand { get; private set; }
        List<ViewRoute> _routes = new List<ViewRoute>();
        List<ViewRoutePoint> _points = new List<ViewRoutePoint>();

        public MapOverviewViewModel()
        {
            //OpenPointPropertiesCommand = new Command(openPointPropertiesCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
        }

        public void StartDialog()
        {
            _points.Clear();
            var routesIds = _routeManager.GetAllRoutes().Select(r=>r.RouteId);
            foreach (string routeId in routesIds)
            {
                var firstAndLastPoints = _routePointManager.GetFirstAndLastViewRoutePoints(routeId);
                _points.Add(firstAndLastPoints.Item2);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("POIs"));
        }

        public void CloseDialog()
        {
        }

        public ObservableCollection<POI> POIs
        {
            get
            {
                var pois = _points.Select(p => new POI() { Name = !string.IsNullOrEmpty(p.NameText) ? p.NameText : "Empty", Address = p.Address, Position = new Position(p.Latitude, p.Longitude), Description = p.Description, PathToPicture = p.ImagePreviewPath });
                return new ObservableCollection<POI>(pois);
            }
        }
        /*internal void openPointPropertiesCommand()
        {

        }*/
        /*internal async void OpenPointPropertiesAsync(double latitude, double longitude)
        {
            RoutePoint point = _routePointManager.GetPointByCoordinates(latitude, longitude);
            if(point != null && point.MainRoute != null)
            {
                var routePointPage = new RoutePointPage(point.MainRoute.RouteId, point.RoutePointId);
                await Navigation.PushAsync(routePointPage, true);
            }
        }*/
    }
}
