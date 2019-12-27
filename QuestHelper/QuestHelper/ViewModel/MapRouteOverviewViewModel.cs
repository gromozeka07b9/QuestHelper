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
        string _routeId = string.Empty;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }

        public MapRouteOverviewViewModel(string routeId)
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
            _routeId = routeId;
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        internal async void OpenPointPropertiesAsync(double latitude, double longitude)
        {
            RoutePoint point = _routePointManager.GetPointByCoordinates(latitude, longitude);
            if(point.MainRoute != null)
            {
                var routePointPage = new RoutePointPage(point.MainRoute.RouteId, point.RoutePointId);
                await Navigation.PushModalAsync(routePointPage, true);
            }
        }

        internal async Task<IEnumerable<ViewRoutePoint>> GetPointsForOverviewRouteAsync()
        {
            var resultPoints = new List<ViewRoutePoint>();
            IEnumerable<ViewRoute> routes;
            if (string.IsNullOrEmpty(_routeId))
            {
                TokenStoreService tokenService = new TokenStoreService();
                string userId = await tokenService.GetUserIdAsync();
                routes = _routeManager.GetRoutes(userId);
            }
            else
            {
                ViewRoute route = _routeManager.GetViewRouteById(_routeId);
                if (route != null)
                {
                    var routesList = new List<ViewRoute>();
                    routesList.Add(route);
                    routes = routesList;
                }
                else
                {
                    routes = new List<ViewRoute>();
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
