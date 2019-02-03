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

namespace QuestHelper.ViewModel
{
    public class MapRouteOverviewViewModel : INotifyPropertyChanged
    {
        RoutePointManager _routePointManager;
        RouteManager _routeManager;
        IEnumerable<RoutePoint> _pointsForOverview;
        string _routeId = string.Empty;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        //public ICommand StartDialogCommand { get; private set; }
        //public ICommand OpenPointPropertiesCommand { get; private set; }

        //public IEnumerable<Route> VisibleRoutes = new List<Route>();
        public MapRouteOverviewViewModel(string routeId)
        {
            //OpenPointPropertiesCommand = new Command(openPointPropertiesCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
            _routeId = routeId;
        }

        /*internal void openPointPropertiesCommand()
        {

        }*/
        internal async void OpenPointPropertiesAsync(double latitude, double longitude)
        {
            RoutePoint point = _routePointManager.GetPointByCoordinates(latitude, longitude);
            if(point.MainRoute != null)
            {
                var routePointPage = new RoutePointPage(point.MainRoute.RouteId, point.RoutePointId);
                await Navigation.PushAsync(routePointPage, true);
            }
        }

        internal IEnumerable<ViewRoutePoint> GetPointsForOverviewRoute()
        {
            var resultPoints = new List<ViewRoutePoint>();
            IEnumerable<Route> routes;
            if (string.IsNullOrEmpty(_routeId))
            {
                routes = _routeManager.GetRoutes();
            }
            else
            {
                Route route = _routeManager.GetRouteById(_routeId);
                if (route != null)
                {
                    var routesList = new List<Route>();
                    routesList.Add(route);
                    routes = routesList;
                }
                else
                {
                    routes = new List<Route>();
                }
            }
            foreach (var route in routes)
            {
                var points = _routePointManager.GetPointsByRouteId(route.RouteId);
                foreach (var point in points)
                {
                    resultPoints.Add(point);
                }
            }
            return resultPoints;
        }

        public string RouteName
        {
            get
            {
                string name = string.Empty;
                Route route = _routeManager.GetRouteById(_routeId);
                if (route != null)
                {
                    name = route.Name;
                }
                return name;
            }
        }
    }
}
