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
    public class SyncPoints : SyncBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly string _authToken = string.Empty;
        private readonly RoutePointsApiRequest _routePointsApi;
        private readonly RoutePointManager _routePointManager = new RoutePointManager();

        public SyncPoints(string authToken)
        {
            _authToken = authToken;
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
        }

        public async Task<bool> Sync()
        {
            bool result = false;

            var points = _routePointManager.GetPoints().Select(x=>new Tuple<string, int>(x.RoutePointId, x.Version));
            SyncObjectStatus pointsServerStatus = await _routePointsApi.GetSyncStatus(points);
            if (pointsServerStatus != null)
            {
                result = true;

                List<string> forUpload = new List<string>();
                List<string> forDownload = new List<string>();

                FillListsObjectsForProcess(points, pointsServerStatus, forUpload, forDownload);

                if (forUpload.Count > 0)
                {
                    result = await UploadAsync(GetJsonStructures(forUpload), _routePointsApi);
                    if (!result) return result;
                }

                if (forDownload.Count > 0)
                {
                    result = await DownloadAsync(forDownload, _routePointsApi);
                }
            }

            return result;
        }

        private List<string> GetJsonStructures(List<string> pointsForUpload)
        {
            List<string> jsonStructures = new List<string>();

            foreach (var pointId in pointsForUpload)
            {
                var uploadedObject = _routePointManager.GetPointById(pointId);
                if ((uploadedObject != null)&&(uploadedObject.CreateDate.Year > 2000))
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        uploadedObject.RoutePointId,
                        uploadedObject.MainRoute.RouteId,
                        uploadedObject.Name,
                        CreateDate = uploadedObject.CreateDate.DateTime,
                        UpdateDate = uploadedObject.CreateDate.DateTime,
                        UpdatedUserId = "",
                        uploadedObject.Latitude,
                        uploadedObject.Longitude,
                        uploadedObject.Address,
                        uploadedObject.Description,
                        uploadedObject.Version
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }
    }
}
