using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Model;

namespace QuestHelper.Managers.Sync
{
    public class SyncPoints
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private static SyncPoints _instance;
        private Action<string> _showWarning;
        private RoutesApiRequest _routesApi = new RoutesApiRequest(_apiUrl);
        private RoutePointsApiRequest _routePointsApi = new RoutePointsApiRequest(_apiUrl);
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl);
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
            await syncRoutes();
            await syncPoints();
            await syncMedia();
            var syncFiles = SyncFiles.GetInstance();
            syncFiles.Start();
        }

        private async Task syncMedia()
        {
            var medias = _routePointMediaManager.GetMediaObjects().Select(x => new Tuple<string, int>(x.RoutePointMediaObjectId, x.Version));
            SyncObjectStatus pointsServerStatus = await _routePointMediaObjectsApi.GetSyncStatus(medias);
            List<string> forUpload = new List<string>();
            List<string> forDownload = new List<string>();

            fillListsObjectsForProcess(medias, pointsServerStatus, forUpload, forDownload);

            if (forUpload.Count > 0)
            {
                await uploadMediasAsync(forUpload);
            }

            if (forDownload.Count > 0)
            {
                await downloadMediasAsync(forDownload);
            }
        }

        private async System.Threading.Tasks.Task<bool> uploadMediasAsync(List<string> idsForUpload)
        {
            bool result = false;
            foreach (var id in idsForUpload)
            {
                var media = _routePointMediaManager.GetMediaObjectById(id);
                if (media != null)
                {
                    result = await _routePointMediaObjectsApi.AddRoutePointMediaObject(media);
                    if (!result)
                    {
                        break;
                    }
                }
                else break;
            }
            return result;
        }

        private async System.Threading.Tasks.Task<bool> downloadMediasAsync(List<string> idsForDownload)
        {
            bool result = false;
            foreach (var id in idsForDownload)
            {
                var media = await _routePointMediaObjectsApi.GetRoutePointMediaObject(id);
                result = media.Save();
                if (!result)
                {
                    break;
                }
            }
            return result;
        }

        private async Task syncPoints()
        {
            var points = _routePointManager.GetPoints().Select(x=>new Tuple<string, int>(x.RoutePointId, x.Version));
            SyncObjectStatus pointsServerStatus = await _routePointsApi.GetSyncStatus(points);
            List<string> forUpload = new List<string>();
            List<string> forDownload = new List<string>();

            fillListsObjectsForProcess(points, pointsServerStatus, forUpload, forDownload);

            if (forUpload.Count > 0)
            {
                await uploadPointsAsync(forUpload);
            }

            if (forDownload.Count > 0)
            {
                await downloadPointsAsync(forDownload);
            }
        }
        private async System.Threading.Tasks.Task<bool> uploadPointsAsync(List<string> idsForUpload)
        {
            bool result = false;
            foreach (var id in idsForUpload)
            {
                var point = _routePointManager.GetPointById(id);
                if (point != null)
                {
                    result = await _routePointsApi.AddRoutePoint(point);
                    if (!result)
                    {
                        break;
                    }
                }
                else break;
            }
            return result;
        }

        private async System.Threading.Tasks.Task<bool> downloadPointsAsync(List<string> idsForDownload)
        {
            bool result = false;
            foreach (var id in idsForDownload)
            {
                var point = await _routePointsApi.GetRoutePoint(id);
                result = point.Save();
                if (!result)
                {
                    break;
                }
            }
            return result;
        }


        private async Task syncRoutes()
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

        private void fillListsObjectsForProcess(IEnumerable<Tuple<string,int>> clientObjects, SyncObjectStatus serverStatus, List<string> forUpload, List<string> forDownload)
        {
            foreach (var serverObject in serverStatus.Statuses)
            {
                //если сервер вернул версию 0, значит на сервере объекта еще нет, его надо будет отправить
                if (serverObject.Version == 0)
                {
                    forUpload.Add(serverObject.ObjectId);
                }
                else
                {
                    //если версия на сервере уже есть, значит есть расхождение версий между сервером и клиентом
                    //тут варианты: если на сервере более старшая версия, клиент должен ее забрать себе
                    //если на сервере младшая версия, то клиент должен отправить свою версию на сервер
                    //Одинаковыми версии быть не могут, если нет изменений, сервер не должен вернуть информацию по маршруту
                    //Если на клиенте маршрута вообще нет, значит грузим его с сервера
                    var objectClient = clientObjects.SingleOrDefault(r => r.Item1 == serverObject.ObjectId);
                    if (objectClient != null)
                    {
                        if (serverObject.Version > objectClient.Item2)
                        {
                            forDownload.Add(serverObject.ObjectId);
                        }
                        else
                        {
                            forUpload.Add(serverObject.ObjectId);
                        }
                    }
                    else
                    {
                        forDownload.Add(serverObject.ObjectId);
                    }
                }
            }
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
