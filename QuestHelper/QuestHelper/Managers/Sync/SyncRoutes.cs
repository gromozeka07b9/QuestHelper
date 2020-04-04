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
using Autofac;
using Microsoft.AppCenter.Analytics;

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
        private ITextfileLogger _log;

        public SyncRoutes(string authToken)
        {
            _authToken = authToken;
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
            _log = App.Container.Resolve<ITextfileLogger>();
        }

        public async Task<bool> Sync()
        {
            bool result = true;
            int mainIndex = 4;//совершенно притянуто за уши, количество этапов, но нужно как-то показать что прогресс идет
            int currentIndex = 0;

            TokenStoreService tokenService = new TokenStoreService();
            string _userId = await tokenService.GetUserIdAsync();
            var notify = DependencyService.Get<INotificationService>();
            sendProgress(string.Empty,++currentIndex, mainIndex);
            var listRoutesVersions = await _routesApi.GetRoutesVersions(true);
            sendProgress(string.Empty, ++currentIndex, mainIndex);
            AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (!AuthRequired)
            {
                sendProgress(string.Empty, ++currentIndex, mainIndex);
                _log.AddStringEvent("GetRoutesVersions, count:" + listRoutesVersions?.Count.ToString());
                if (listRoutesVersions?.Count > 0)
                {
                    var routesLocal = _routeManager.GetRoutesForSync().Select(x => new { x.RouteId, x.Version, x.ObjVerHash, x.IsPublished });
                    var differentRoutes = listRoutesVersions.Where(r => (!routesLocal.Any(l => (l.RouteId == r.Id && l.ObjVerHash == r.ObjVerHash))));
                    var newClientRoutes = routesLocal.Where(r => !r.IsPublished && !listRoutesVersions.Any(d => d.Id == r.RouteId)).Select(r => r.RouteId).ToList();

                    sendProgress(string.Empty, ++currentIndex, mainIndex);

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
                        sendProgress(string.Empty,currentCountRoutesForSync + currentIndex, countRoutesForSync + mainIndex);
                    }

                    foreach (var logRoute in newClientRoutes)
                    {
                        _log.AddStringEvent($"newRoute:{logRoute}");
                    }
                    foreach (var localRouteId in newClientRoutes)
                    {
                        SyncRoute syncRouteContext = new SyncRoute(localRouteId, _authToken);
                        syncRouteContext.SyncImages = true;
                        _log.AddStringEvent($"start sync new route {localRouteId}");
                        result = await syncRouteContext.SyncAsync(string.Empty);
                        _log.AddStringEvent($"new route result, {localRouteId} :" + result);
                        currentCountRoutesForSync++;
                        sendProgress(string.Empty, currentCountRoutesForSync + currentIndex, countRoutesForSync + mainIndex);
                    }
                    Xamarin.Forms.MessagingCenter.Send<SyncRouteCompleteMessage>(new SyncRouteCompleteMessage() { RouteId = string.Empty, SuccessSync = result }, string.Empty);
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static void sendProgress(string routeId, int currentCountRoutesForSync, int countRoutesForSync)
        {
            double percent = (double) currentCountRoutesForSync * 100 / (double) countRoutesForSync / 100;
            MessagingCenter.Send<SyncProgressRouteLoadingMessage>(
                new SyncProgressRouteLoadingMessage() {RouteId = routeId, ProgressValue = percent}, string.Empty);
        }

        public async Task<bool> Sync(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                Analytics.TrackEvent("Sync by routeId error");
                return false;
            }

            bool result = true;
            int mainIndex = 3;//количество этапов
            int currentIndex = 0;
            sendProgress(routeId, ++currentIndex, mainIndex);
            var notify = DependencyService.Get<INotificationService>();
            var serverRoutesVersionsFull = await _routesApi.GetRoutesVersions(false);
            sendProgress(routeId, ++currentIndex, mainIndex);
            var serverRouteVersion = serverRoutesVersionsFull?.Where(r => r.Id.Equals(routeId)).FirstOrDefault();
            AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (!AuthRequired)
            {
                var localRoute = _routeManager.GetRouteById(routeId);
                sendProgress(routeId, ++currentIndex, mainIndex);
                if ((localRoute == null) || (serverRouteVersion != null && !serverRouteVersion.ObjVerHash.Equals(localRoute.ObjVerHash)))
                {
                    SyncRoute syncRouteContext = new SyncRoute(serverRouteVersion?.Id, _authToken);
                    syncRouteContext.SyncImages = true;
                    _log.AddStringEvent($"start sync diff route {serverRouteVersion?.Id}");
                    result = await syncRouteContext.SyncAsync(serverRouteVersion?.ObjVerHash, true);
                    _log.AddStringEvent($"diff route result, {serverRouteVersion?.Id} :" + result);
                }
            }
            else
            {
                result = false;
            }
            if (result)
            {
                Xamarin.Forms.MessagingCenter.Send<SyncRouteCompleteMessage>(new SyncRouteCompleteMessage() { RouteId = routeId, SuccessSync = result }, string.Empty);
            }
            return result;
        }
    }
}
