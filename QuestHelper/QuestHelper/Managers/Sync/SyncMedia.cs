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
using System.Net;
using Microsoft.AppCenter.Analytics;

namespace QuestHelper.Managers.Sync
{
    public class SyncMedia : SyncBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutePointsApiRequest _routePointsApi;
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private readonly RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();
        private readonly string _authToken = string.Empty;

        public SyncMedia(string authToken)
        {
            _authToken = authToken;
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
        }
        public async Task<bool> Sync()
        {
            bool result = false;

            var medias = _routePointMediaManager.GetAllMediaObjects().Select(x => new Tuple<string, int>(x.RoutePointMediaObjectId, x.Version));
            SyncObjectStatus mediasServerStatus = await _routePointMediaObjectsApi.GetSyncStatus(medias);
            AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (mediasServerStatus != null)
            {
                result = true;

                List<string> forUpload = new List<string>();
                List<string> forDownload = new List<string>();

                FillListsObjectsForProcess(medias, mediasServerStatus, forUpload, forDownload);

                if (forUpload.Count > 0)
                {
                    result = await UploadAsync(GetJsonStructures(forUpload), _routePointMediaObjectsApi);
                    if (!result) return result;
                }

                if (forDownload.Count > 0)
                {
                    result = await DownloadAsync(forDownload, _routePointMediaObjectsApi);
                    if (!result) return result;
                }

                result = await SendNotSyncedFiles();
                if (!result) return result;

                SyncFiles syncFiles = new SyncFiles(_authToken);
                await syncFiles.CheckExistsAllImageAndDownloadIfNeeded(true);
                await syncFiles.CheckExistsAllImageAndDownloadIfNeeded(false);
            }

            return result;
        }

        private async Task<bool> SendNotSyncedFiles()
        {
            var notSyncedMedias = _routePointMediaManager.GetNotSyncedMediaObjects();
            bool sendResult = await SendImage(notSyncedMedias, true);
            if (sendResult)
            {
                sendResult = await SendImage(notSyncedMedias, false);
                if (sendResult)
                {
                    foreach (var media in notSyncedMedias)
                    {
                        _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, sendResult);
                    }
                }
            }
            return sendResult;
        }

        private async Task<bool> SendImage(IEnumerable<RoutePointMediaObject> notSyncedMedias, bool isPreview)
        {
            foreach (var media in notSyncedMedias.Where(m=>!m.IsDeleted))
            {
                bool result = await _routePointMediaObjectsApi.SendImage(media.RoutePointId, media.RoutePointMediaObjectId, isPreview);
                AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                if (!result)
                {
                    Analytics.TrackEvent("Sync media: upload error", new Dictionary<string, string> { { "RoutePointMediaObjectId", media.RoutePointMediaObjectId } });
                    return false;
                }
            }
            return true;
        }

        private List<string> GetJsonStructures(List<string> mediasForUpload)
        {
            List<string> jsonStructures = new List<string>();

            foreach (var mediaId in mediasForUpload)
            {
                var uploadedObject = _routePointMediaManager.GetMediaObjectById(mediaId);
                if (uploadedObject != null)
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        uploadedObject.RoutePointMediaObjectId,
                        uploadedObject.RoutePointId,
                        uploadedObject.Version,
                        uploadedObject.IsDeleted
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }
    }
}
