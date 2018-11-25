using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;

namespace QuestHelper.Managers.Sync
{
    public class SyncRoutes : SyncBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutesApiRequest _routesApi = new RoutesApiRequest(_apiUrl);
        private RoutePointsApiRequest _routePointsApi = new RoutePointsApiRequest(_apiUrl);
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl);
        private readonly RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();

        public SyncRoutes()
        {

        }

        public async Task Sync()
        {
            var routes = _routeManager.GetRoutes().Select(x => new Tuple<string, int>(x.RouteId, x.Version));
            SyncObjectStatus routeServerStatus = await _routesApi.GetSyncStatus(routes);
            if (routeServerStatus != null)
            {
                List<string> routesForUpload = new List<string>();
                List<string> routesForDownload = new List<string>();

                FillListsObjectsForProcess(routes, routeServerStatus, routesForUpload, routesForDownload);
                if (routesForUpload.Count > 0)
                {
                    bool result = await UploadAsync(GetJsonStructures(routesForUpload), _routesApi);
                    //bool result = await uploadRoutesAsync(routesForUpload);
                    //if (!result) _showWarning("Ошибка передачи данных");
                }

                if (routesForDownload.Count > 0)
                {
                    bool result = await DownloadAsync(routesForDownload, _routesApi);
                    //bool result = await downloadRoutesAsync(routesForDownload);
                    //if (!result) _showWarning("Ошибка загрузки данных");
                }
            }
            //else _showWarning("Ошибка загрузки маршрутов");
        }

        private List<string> GetJsonStructures(List<string> routesForUpload)
        {            
            List<string> jsonStructures = new List<string>();
            foreach (var routeId in routesForUpload)
            {
                var uploadedObject = _routeManager.GetRouteById(routeId);
                JObject jsonObject = JObject.FromObject(new
                {
                    RouteId = uploadedObject.RouteId,
                    Name = uploadedObject.Name,
                    CreateDate = uploadedObject.CreateDate.DateTime,
                    Version = uploadedObject.Version,
                    UserId = 0
                });
                jsonStructures.Add(jsonObject.ToString());
            }

            return jsonStructures;
        }

        /*private async System.Threading.Tasks.Task<bool> uploadRoutesAsync(List<string> routesIdsForUpload)
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
        }*/
    }
}
