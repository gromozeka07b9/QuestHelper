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

namespace QuestHelper.Managers.Sync
{
    public class SyncRoutes : SyncBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutesApiRequest _routesApi;
        private readonly RouteManager _routeManager = new RouteManager();
        private readonly string _authToken = string.Empty;

        public SyncRoutes(string authToken)
        {
            _authToken = authToken;
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
        }

        public async Task<bool> Sync()
        {
            bool result = false;

            var routes = _routeManager.GetRoutesForSync().Select(x => new Tuple<string, int>(x.RouteId, x.Version));
            SyncObjectStatus routeServerStatus = await _routesApi.GetSyncStatus(routes);
            AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (routeServerStatus != null)
            {
                result = true;
                List<string> routesForUpload = new List<string>();
                List<string> routesForDownload = new List<string>();

                FillListsObjectsForProcess(routes, routeServerStatus, routesForUpload, routesForDownload);
                if (routesForUpload.Count > 0)
                {
                    result = await UploadAsync(GetJsonStructures(routesForUpload), _routesApi);
                    if (!result) return result;
                }

                if (routesForDownload.Count > 0)
                {
                    result = await DownloadAsync(routesForDownload, _routesApi);
                }
            }

            return result;
        }

        private List<string> GetJsonStructures(List<string> routesForUpload)
        {            
            List<string> jsonStructures = new List<string>();
            foreach (var routeId in routesForUpload)
            {
                var uploadedObject = _routeManager.GetRouteById(routeId);
                if (uploadedObject != null)
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        RouteId = uploadedObject.RouteId,
                        Name = uploadedObject.Name,
                        CreateDate = uploadedObject.CreateDate.DateTime,
                        Version = uploadedObject.Version,
                        IsPublished = uploadedObject.IsPublished,
                        CreatorId = uploadedObject.CreatorId,
                        UserId = 0
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }
    }
}
