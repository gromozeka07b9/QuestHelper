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
using System.IO;
using Xamarin.Forms;
using QuestHelper.Model.Messages;
using QuestHelper.Model.WS;

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

                MessagingCenter.Send<SyncProgressMessage>(new SyncProgressMessage() { SyncInProgress = true, SyncDetailText = "Синхронизация превью..." }, string.Empty);
                bool resultSyncPreview = await SyncFiles(true);
                MessagingCenter.Send<SyncProgressMessage>(new SyncProgressMessage() { SyncInProgress = true, SyncDetailText = "Синхронизация оригиналов..." }, string.Empty);
                bool resultSyncOriginal = await SyncFiles(false);
                result = (resultSyncOriginal)&&(resultSyncPreview);
            }

            return result;
        }

        private async Task<bool> SyncFiles(bool IsSyncPreview)
        {
            bool syncResult = true;

            var notSyncedMedias = _routePointMediaManager.GetNotSyncedMediaObjects(IsSyncPreview).Where(m => !m.IsDeleted);
            StringBuilder sbErrors = new StringBuilder();
            foreach (var media in notSyncedMedias)
            {
                sbErrors.AppendLine($"to sync: preview:{IsSyncPreview} p:{media.RoutePointId} m:{media.RoutePointMediaObjectId}");
            }
            _errorReport += sbErrors.ToString();

            ImagesServerStatus imagesRequestObject = new ImagesServerStatus();
            foreach (var media in notSyncedMedias)
            {
                string nameMediafile = ImagePathManager.GetImageFilename(media.RoutePointMediaObjectId, IsSyncPreview);
                imagesRequestObject.Images.Add(new ImagesServerStatus.Imagefile(){Name = nameMediafile });
            }
            ImagesServerStatus imagesResponseObject = await _routePointMediaObjectsApi.GetImagesStatus(imagesRequestObject);
            AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (AuthRequired) return false;
            if (imagesResponseObject.Images.Count > 0)
            {
                foreach (var imageItem in imagesResponseObject.Images)
                {
                    var media = getMediaByImagename(notSyncedMedias, imageItem.Name);
                    if (imageItem.OnServer)
                    {
                        string pathToPictures = ImagePathManager.GetPicturesDirectory();
                        string pathToMediaFile = Path.Combine(pathToPictures, imageItem.Name);
                        if (!File.Exists(pathToMediaFile))
                        {
                            bool resultDownload = await _routePointMediaObjectsApi.GetImage(media.RoutePointId, media.RoutePointMediaObjectId, ImagePathManager.GetPicturesDirectory(), imageItem.Name);
                            AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                            if (AuthRequired) return false;
                            if (resultDownload)
                            {
                                _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, IsSyncPreview, true);
                            }
                            else
                            {
                                syncResult = false;
                                sbErrors.AppendLine($"error download: p:{media.RoutePointId} m:{media.RoutePointMediaObjectId}");
                            }
                        }
                        else
                        {
                            _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, IsSyncPreview, true);
                        }
                    }
                    else
                    {
                        bool resultUpload = await _routePointMediaObjectsApi.SendImage(media.RoutePointId, media.RoutePointMediaObjectId, IsSyncPreview);
                        AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                        if (AuthRequired) return false;
                        if (resultUpload)
                        {
                            _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, IsSyncPreview, true);
                        }
                        else
                        {
                            syncResult = false;
                            sbErrors.AppendLine($"error upload: p:{media.RoutePointId} m:{media.RoutePointMediaObjectId}");
                        }
                    }
                }
            }
            /*foreach (var media in notSyncedMedias)
        {
            string nameMediafile = ImagePathManager.GetImageFilename(media.RoutePointMediaObjectId, IsSyncPreview);
            HttpStatusCode checkResult = await _routePointMediaObjectsApi.ImageExist(nameMediafile);
            AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (AuthRequired) return false;
            switch (checkResult)
            {
                case HttpStatusCode.NotFound:
                {
                    bool resultUpload = await _routePointMediaObjectsApi.SendImage(media.RoutePointId, media.RoutePointMediaObjectId, IsSyncPreview);
                    AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                    if (AuthRequired) return false;
                    if (resultUpload)
                    {
                        _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, IsSyncPreview, true);
                    }
                    else
                    {
                        syncResult = false;
                        sbErrors.AppendLine($"error upload: p:{media.RoutePointId} m:{media.RoutePointMediaObjectId}");
                    }
                }; break;
                case HttpStatusCode.OK:
                {
                    string pathToPictures = ImagePathManager.GetPicturesDirectory();
                    string pathToMediaFile = Path.Combine(pathToPictures, nameMediafile);
                    if (!File.Exists(pathToMediaFile))
                    {
                        bool resultDownload = await _routePointMediaObjectsApi.GetImage(media.RoutePointId, media.RoutePointMediaObjectId, ImagePathManager.GetPicturesDirectory(), nameMediafile);
                        AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                        if (AuthRequired) return false;
                        if (resultDownload)
                        {
                            _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, IsSyncPreview, true);
                        }
                        else
                        {
                            syncResult = false;
                            sbErrors.AppendLine($"error download: p:{media.RoutePointId} m:{media.RoutePointMediaObjectId}");
                        }
                    }
                    else
                    {
                        _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, IsSyncPreview, true);
                    }
                    }; break;
            }

        }*/

            _errorReport += sbErrors.ToString();

            return syncResult;
        }

        private RoutePointMediaObject getMediaByImagename(IEnumerable<RoutePointMediaObject> notSyncedMedias, string name)
        {
            string imageNameOriginal = name.ToLower().Replace("_preview", "").Replace("img_", "").Trim();
            var imageNameParts = imageNameOriginal.Split('.');
            if (imageNameParts.Length > 0)
            {
                var item = notSyncedMedias.Where(m => m.RoutePointMediaObjectId.Equals(imageNameParts[0])).SingleOrDefault();
                return item;
            }
            return new RoutePointMediaObject();
        }

        private async Task<bool> SendImages(IEnumerable<RoutePointMediaObject> notSyncedMedias, bool isPreview)
        {
            bool result = true;
            StringBuilder sbErrors = new StringBuilder();
            foreach (var media in notSyncedMedias.Where(m=>!m.IsDeleted))
            {
                bool resultUpload = await _routePointMediaObjectsApi.SendImage(media.RoutePointId, media.RoutePointMediaObjectId, isPreview);
                AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                if (!resultUpload)
                {
                    result = false;
                    sbErrors.AppendLine($"up: p:{media.RoutePointId} m:{media.RoutePointMediaObjectId}");
                    Analytics.TrackEvent("Sync media: upload error", new Dictionary<string, string> { { "RoutePointId", media.RoutePointId }, { "RoutePointMediaObjectId", media.RoutePointMediaObjectId } });
                    if(AuthRequired) return result;
                }
            }

            _errorReport += sbErrors.ToString();

            return result;
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

        public string ErrorReport
        {
            get { return _errorReport; }
        }
    }
}
