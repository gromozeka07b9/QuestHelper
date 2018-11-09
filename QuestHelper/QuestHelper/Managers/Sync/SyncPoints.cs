using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.Model;

namespace QuestHelper.Managers.Sync
{
    public class SyncPoints
    {
        private static SyncPoints _instance;
        private Action<string> _showWarning;
        private RoutesApiRequest _routesApi = new RoutesApiRequest("http://questhelperserver.azurewebsites.net");
        private RoutePointsApiRequest _routePointsApi = new RoutePointsApiRequest("http://questhelperserver.azurewebsites.net");
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest("http://questhelperserver.azurewebsites.net");
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();

        public static SyncPoints GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SyncPoints();
            }

            return _instance;
        }
        internal void SetWarningDialogContext(Action<string> showWarning)
        {
            _showWarning = showWarning;
        }
        internal async System.Threading.Tasks.Task StartAsync()
        {
            IEnumerable<Route> routes = _routeManager.GetRoutes();
            SyncObjectStatus routeServerStatus = await _routesApi.GetSyncStatus(routes);

            List<string> routesForUpload = new List<string>();
            List<string> routesForDownload = new List<string>();

            fillListsRoutesForProcess(routes, routeServerStatus, routesForUpload, routesForDownload);
            if (routesForUpload.Count > 0)
            {
                bool result = await uploadRoutesAsync(routesForUpload);
                if (!result) _showWarning("Ошибка передачи данных");
            }
            if (routesForDownload.Count > 0)
            {
                bool result = await downloadRoutesAsync(routesForDownload);
                if (!result) _showWarning("Ошибка загрузки данных");
            }
            /*IEnumerable<Route> notSyncedRoutes = _routeManager.GetNotSynced();
            foreach(var route in notSyncedRoutes)
            {
                bool added = await _routesApi.UpdateRoute(route);
                if(added)
                {
                    _routeManager.SetSyncStatus(route.RouteId, added);
                } else
                {
                    break;
                    //Пока непонятно что делать если загрузка не проходит
                }
            }

            IEnumerable<RoutePoint> notSyncedPoints = _routePointManager.GetNotSynced();
            foreach (var point in notSyncedPoints)
            {
                bool added = await _routePointsApi.AddRoutePoint(point);
                if (added)
                {
                    _routePointManager.SetSyncStatus(point.RoutePointId, added);
                }
                else
                {
                    break;
                    //Пока непонятно что делать если загрузка не проходит
                }
            }
            IEnumerable<RoutePointMediaObject> notSyncedMedia = _routePointMediaManager.GetNotSynced();
            foreach (var media in notSyncedMedia)
            {
                bool added = await _routePointMediaObjectsApi.AddRoutePointMediaObject(media);
                if (added)
                {
                    SyncFiles.UploadMedia(media);
                    _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, added);
                }
                else
                {
                    break;
                    //Пока непонятно что делать если загрузка не проходит
                }
            }*/
            /*await _routesApi.AddRoute(_route);
            await _routePointsApi.AddRoutePoint(_point);
            if (_point.MediaObjects.Count > 0)
            {
                await _routePointMediaObjectsApi.AddRoutePointMediaObject(_point.MediaObjects[0]);
            }*/

        }

        private async System.Threading.Tasks.Task<bool> uploadRoutesAsync(List<string> routesIdsForUpload)
        {
            bool result = false;
            foreach (var routeId in routesIdsForUpload)
            {
                var route = _routeManager.GetRouteById(routeId);
                if (route != null)
                {
                    result = await _routesApi.UpdateRoute(route);
                    if (!result)
                    {
                        break;
                    }
                }
                else break;
            }
            return result;
        }
        private async System.Threading.Tasks.Task<bool> downloadRoutesAsync(List<string> routesIdsForDownload)
        {
            bool result = false;
            foreach (var routeId in routesIdsForDownload)
            {
                var route = await _routesApi.GetRoute(routeId);
                result = route.Save();
                if (!result)
                {
                    break;
                }
            }
            return result;
        }

        private void fillListsRoutesForProcess(IEnumerable<Route> routes, SyncObjectStatus routeServerStatus, List<string> routesForUpload, List<string> routesForDownload)
        {
            foreach (var routeServer in routeServerStatus.Statuses)
            {
                //если сервер вернул версию 0, значит на сервере маршрута еще нет, его надо будет отправить
                if (routeServer.Version == 0)
                {
                    routesForUpload.Add(routeServer.ObjectId);
                }
                else
                {
                    //если версия на сервере уже есть, значит есть расхождение версий между сервером и клиентом
                    //тут варианты: если на сервере более старшая версия, клиент должен ее забрать себе
                    //если на сервере младшая версия, то клиент должен отправить свою версию на сервер
                    //Одинаковыми версии быть не могут, если нет изменений, сервер не должен вернуть информацию по маршруту
                    //Если на клиенте маршрута вообще нет, значит грузим его с сервера
                    Route routeClient = routes.SingleOrDefault(r => r.RouteId == routeServer.ObjectId);
                    if (routeClient != null)
                    {
                        if (routeServer.Version > routeClient.Version)
                        {
                            routesForDownload.Add(routeServer.ObjectId);
                        }
                        else
                        {
                            routesForUpload.Add(routeServer.ObjectId);
                        }
                    }
                    else
                    {
                        routesForDownload.Add(routeServer.ObjectId);
                    }
                }
            }
        }
    }
}
