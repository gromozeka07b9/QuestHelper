using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using Xamarin.Forms;
using Route = QuestHelper.SharedModelsWS.Route;

namespace QuestHelper.Managers.Sync
{
    public class SyncRoutes
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutesApiRequest _routesApi;
        private readonly RoutePointsApiRequest _routePointsApi;
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private readonly RouteManager _routeManager = new RouteManager();
        private readonly RoutePointManager _routePointManager = new RoutePointManager();
        private readonly RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();
        private readonly string _authToken = string.Empty;
        public bool AuthRequired { get; internal set; }
        private Logger _log = new Logger(true);

        public SyncRoutes(string authToken)
        {
            _authToken = authToken;
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
        }

        public async Task<bool> Sync()
        {
            bool result = true;
            var notify = DependencyService.Get<INotificationService>();
            var listRoutesVersions = await _routesApi.GetRoutesVersions();
            AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (!AuthRequired)
            {
                _log.AddStringEvent("GetRoutesVersions, count:" + listRoutesVersions?.Count.ToString());
                if ((!AuthRequired) && (listRoutesVersions?.Count > 0))
                {
                    var routesLocal = _routeManager.GetRoutesForSync().Select(x => new { x.RouteId, x.Version, x.ObjVerHash });
                    var differentRoutes = listRoutesVersions.Where(r => (!routesLocal.Any(l => (l.RouteId == r.Id && l.ObjVerHash == r.ObjVerHash))));
                    var newClientRoutes = routesLocal.Where(r => !listRoutesVersions.Any(d => d.Id == r.RouteId)).Select(r => r.RouteId).ToList();

                    int countRoutesForSync = differentRoutes.Count() + newClientRoutes.Count();
                    int currentCountRoutesForSync = 0;

                    _log.AddStringEvent($"--------------------------------------------------------");
                    foreach (var logRoute in differentRoutes)
                    {
                        _log.AddStringEvent($"differentRoute:{logRoute.Id}");
                    }
                    foreach (var serverRouteVersion in differentRoutes)
                    {
                        SyncRoute syncRouteContext = new SyncRoute(serverRouteVersion.Id, _authToken);
                        syncRouteContext.SyncImages = true;
                        _log.AddStringEvent($"start sync diff route {serverRouteVersion.Id}");
                        result = await syncRouteContext.SyncAsync(serverRouteVersion.ObjVerHash);
                        _log.AddStringEvent($"diff route result, {serverRouteVersion.Id} :" + result);
                        currentCountRoutesForSync++;
                    }

                    foreach (var logRoute in newClientRoutes)
                    {
                        _log.AddStringEvent($"differentRoute:{logRoute}");
                    }
                    foreach (var localRouteId in newClientRoutes)
                    {
                        SyncRoute syncRouteContext = new SyncRoute(localRouteId, _authToken);
                        syncRouteContext.SyncImages = false;
                        _log.AddStringEvent($"start sync new route {localRouteId}");
                        result = await syncRouteContext.SyncAsync(string.Empty);
                        _log.AddStringEvent($"new route result, {localRouteId} :" + result);
                        currentCountRoutesForSync++;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
        public async Task<bool> Sync(string routeId)
        {
            bool result = true;
            var notify = DependencyService.Get<INotificationService>();
            var serverRouteVersion = (await _routesApi.GetRoutesVersions()).Where(r => r.Id.Equals(routeId)).FirstOrDefault();
            AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (!AuthRequired)
            {
                var localRoute = _routeManager.GetRouteById(routeId);
                if ((localRoute == null) || !serverRouteVersion.ObjVerHash.Equals(localRoute.ObjVerHash))
                {
                    SyncRoute syncRouteContext = new SyncRoute(serverRouteVersion.Id, _authToken);
                    syncRouteContext.SyncImages = true;
                    _log.AddStringEvent($"start sync diff route {serverRouteVersion.Id}");
                    result = await syncRouteContext.SyncAsync(serverRouteVersion.ObjVerHash);
                    _log.AddStringEvent($"diff route result, {serverRouteVersion.Id} :" + result);
                    Xamarin.Forms.MessagingCenter.Send<SyncRouteCompleteMessage>(new SyncRouteCompleteMessage() { RouteId = routeId, SuccessSync = result }, string.Empty);
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}
