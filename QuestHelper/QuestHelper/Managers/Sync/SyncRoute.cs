using QuestHelper.Model;
using QuestHelper.WS;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using QuestHelper.SharedModelsWS;
using System.Collections.Generic;
using System.Text;
using QuestHelper.LocalDB.Model;
using System.IO;
using Autofac;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model.Messages;
using Syncfusion.DataSource.Extensions;
using RoutePointMediaObject = QuestHelper.LocalDB.Model.RoutePointMediaObject;

namespace QuestHelper.Managers.Sync
{
    public class SyncRoute : SyncRouteBase
    {
        private const string _apiUrl = "https://igosh.pro/api";
        private string _authToken = string.Empty;
        private readonly string _routeId = string.Empty;
        private readonly RoutesApiRequest _routesApi;
        private readonly RoutePointsApiRequest _routePointsApi;
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private readonly RouteManager _routeManager = new RouteManager();
        private readonly RoutePointManager _routePointManager = new RoutePointManager();
        private readonly RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();
        //private bool _syncMediaFiles = false;
        private ITextfileLogger _log;


        public SyncRoute(string routeId, string authToken)
        {
            _routeId = routeId;
            _authToken = authToken;
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
            _log = App.Container.Resolve<ITextfileLogger>();
        }

        public async Task<bool> SyncAsync(string routeServerHash, bool loadOnlyPreviewImg = false)
        {
            var localRoute = _routeManager.GetViewRouteById(_routeId);
            //новый маршрут
            if (string.IsNullOrEmpty(routeServerHash))
            {
                bool updateResult = await updateRoute(routeServerHash, new RouteRoot() { Route = new SharedModelsWS.Route() { Version = -1 } }, localRoute);
                _log.AddStringEvent($"route {_routeId}, create result:{updateResult}");
            }

            //независимо от того, новый маршрут или нет, проверяем, что изменилось - придется пройтись по всему маршруту и узнать
            RouteRoot routeRoot = await _routesApi.GetRouteRoot(_routeId);
            bool AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if ((!AuthRequired) && (routeRoot != null))
            {
                bool updateResult = await updateRoute(routeServerHash, routeRoot, localRoute);
                if (!updateResult) return false;

                if (!localRoute.IsDeleted)
                {
                    updateResult = await updatePoints(routeRoot);
                    if (!updateResult) return false;

                    List<ViewRoutePointMediaObject> mediaForDownload = new List<ViewRoutePointMediaObject>();
                    List<ViewRoutePointMediaObject> mediaForUpload = new List<ViewRoutePointMediaObject>();
                    (updateResult, mediaForUpload, mediaForDownload) = await updateMedias(routeRoot);
                    if (!updateResult) return false;
                    
                    if (mediaForDownload.Count > 0)
                    {
                        await downloadMedias(mediaForDownload, loadOnlyPreviewImg);
                    }
                    if (mediaForUpload.Count > 0)
                    {
                        await uploadMedias(mediaForUpload);
                    }
                    /*if (_syncMediaFiles)
                    {
                        if (mediaForDownload.Count > 0)
                        {
                            downloadMedias(mediaForDownload, loadOnlyPreviewImg);
                        }
                        var medias = _routePointMediaManager.GetMediaObjectsByRouteId(routeRoot.Route.Id).Where(m => !m.OriginalServerSynced || !m.PreviewServerSynced).Select(m => new MediaForUpdate { RoutePointId = m.RoutePointId, RoutePointMediaObjectId = m.RoutePointMediaObjectId, OriginalServerSynced = m.OriginalServerSynced, PreviewServerSynced = m.PreviewServerSynced, IsDeleted = m.IsDeleted, MediaType = (MediaObjectTypeEnum)m.MediaType }).ToList();
                        _log.AddStringEvent($"media files sync,  route {_routeId}, media count:{medias.Count.ToString()}");

                        int count = medias.Count;
                        int index = 0;
                        foreach (var media in medias)
                        {
                            bool result = await updateImages(media, loadOnlyPreviewImg);
                            if (!result) syncImgHasErrors = !result;
                            index++;
                            double percent = (double)index * 100 / (double)count / 100;
                            Xamarin.Forms.MessagingCenter.Send<SyncProgressImageLoadingMessage>(new SyncProgressImageLoadingMessage() { RouteId = _routeId, ProgressValue = percent }, string.Empty);
                        }
                    }*/
                }
            }
            else return false;

            var serverHash = await _routesApi.UpdateHash(_routeId);
            if (_routesApi.LastHttpStatusCode == HttpStatusCode.OK)
            {
                var updatedLocalRoute = _routeManager.GetViewRouteById(_routeId);
                updatedLocalRoute.ServerSynced = true;
                updatedLocalRoute.ObjVerHash = HashManager.Generate(getVersionsForRoute(updatedLocalRoute).ToString());
                if (updatedLocalRoute.ObjVerHash.Equals(serverHash))
                {
                    _log.AddStringEvent($"set route {_routeId}, versions {serverHash}");
                    updatedLocalRoute.Save();
                }
                else
                {
                    _log.AddStringEvent($"failed set route {_routeId}, versions {serverHash}");
                    HandleError.Process("SyncRoute", "ErrorUpdateHash", new Exception("Client and Server hash different"), false, $"server:[{serverHash}], client:[{updatedLocalRoute.ObjVerHash}]");
                }

                if (updatedLocalRoute.IsDeleted)
                {
                    _log.AddStringEvent($"set delete route {_routeId}");
                    deleteRouteContain(updatedLocalRoute);
                }
            }
            else
            {
                HandleError.Process("SyncRoute", "ErrorUpdateHash", new Exception("Http error:" + _routesApi.LastHttpStatusCode.ToString()), false);
            }

            return true;
        }

        private async Task uploadMedias(List<ViewRoutePointMediaObject> mediaForUpload)
        {
            Analytics.TrackEvent("SyncRoute", new Dictionary<string, string> {{"RouteId", _routeId}, {"uploadMediasCount", mediaForUpload.Count.ToString()}});
            int count = mediaForUpload.Count;
            int index = 0;
            foreach (var media in mediaForUpload)
            {
                await uploadMedia(media.RoutePointId, media.Id, media.MediaType, true);
                if (media.MediaType == MediaObjectTypeEnum.Image)
                {
                    await uploadMedia(media.RoutePointId, media.Id, media.MediaType, false);
                }
                index++;
                double percent = (double)index * 100 / (double)count / 100;
                Xamarin.Forms.MessagingCenter.Send<SyncProgressImageLoadingMessage>(new SyncProgressImageLoadingMessage() { RouteId = _routeId, ProgressValue = percent }, string.Empty);
            }
        }

        private async Task<bool> uploadMedia(string routePointId, string mediaId, MediaObjectTypeEnum mediaMediaType, bool loadOnlyPreviewImg)
        {
            string filename = ImagePathManager.GetMediaFilename(mediaId, mediaMediaType, loadOnlyPreviewImg);
            bool uploadResult = await _routePointMediaObjectsApi.SendImage(routePointId, mediaId, mediaMediaType, loadOnlyPreviewImg);
            
            if (!uploadResult)
            {
                HandleError.Process("SyncRoute", "ErrorUploadMedia", new Exception("ErrorUploadMedia"), false, $"filename:[{filename}], status:[{_routePointMediaObjectsApi.LastHttpStatusCode.ToString()}]");
            }
            
            return uploadResult;

        }

        private async Task downloadMedias(List<ViewRoutePointMediaObject> mediaForDownload, bool loadOnlyPreviewImg)
        {
            Analytics.TrackEvent("SyncRoute", new Dictionary<string, string> {{"RouteId", _routeId}, {"downloadMediasCount", mediaForDownload.Count.ToString()}});

            int count = mediaForDownload.Count;
            int index = 0;
            foreach (var media in mediaForDownload)
            {
                if (!media.IsDeleted)
                {
                    await downloadMedia(media.RoutePointId, media.Id, media.MediaType, true);
                    if ((!loadOnlyPreviewImg) && (media.MediaType == MediaObjectTypeEnum.Image))
                    {
                        await downloadMedia(media.RoutePointId, media.Id, media.MediaType, loadOnlyPreviewImg);
                    }
                }
                else
                {
                    deleteMedia(media);
                }
                index++;
                double percent = (double)index * 100 / (double)count / 100;
                Xamarin.Forms.MessagingCenter.Send<SyncProgressImageLoadingMessage>(new SyncProgressImageLoadingMessage() { RouteId = _routeId, ProgressValue = percent }, string.Empty);
            }
            
        }

        private async Task<bool> downloadMedia(string routePointId, string mediaId, MediaObjectTypeEnum mediaMediaType, bool loadOnlyPreviewImg)
        {
            bool downloadResult = false;
            string filename = ImagePathManager.GetMediaFilename(mediaId, mediaMediaType, loadOnlyPreviewImg);
            string pathToMediaFile = ImagePathManager.GetImagePath(mediaId, mediaMediaType, loadOnlyPreviewImg);

            downloadResult = File.Exists(pathToMediaFile);
            if (!downloadResult)
            {
                downloadResult = await _routePointMediaObjectsApi.GetImage(routePointId, mediaId, ImagePathManager.GetPicturesDirectory(), filename);
                if (!downloadResult)
                {
                    HandleError.Process("SyncRoute", "ErrorDownloadMedia", new Exception("ErrorDownloadMedia"), false, $"filename:[{filename}], status:[{_routePointMediaObjectsApi.LastHttpStatusCode.ToString()}]");
                }
            }

            return downloadResult;
        }

        private void deleteMedia(ViewRoutePointMediaObject media)
        {
            _log.AddStringEvent($"image deleted, passed mediaid:{media.RoutePointMediaObjectId}");
            _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, false, true);
            _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, true, true);
            MediaFileManager fileManager = new MediaFileManager();
            fileManager.Delete(media.RoutePointMediaObjectId, media.MediaType);
        }

        private async Task<bool> updateImages(MediaForUpdate media, bool loadOnlyPreviewImg)
        {
            bool result = false;
            if (!media.IsDeleted)
            {
                if ((media.MediaType == MediaObjectTypeEnum.Audio || (media.MediaType == MediaObjectTypeEnum.Image && !loadOnlyPreviewImg)) && (!media.OriginalServerSynced))
                {
                    var httpStatus = await updateMediaFileAsync(media.RoutePointId, media.RoutePointMediaObjectId,
                        media.MediaType, false);
                    result = httpStatus == HttpStatusCode.OK;
                    _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, false,
                        result);
                    if (!result)
                        _log.AddStringEvent(
                            $"media file update {media.RoutePointMediaObjectId}, original, http status:{HttpStatusCode.OK}");
                }

                if ((!media.PreviewServerSynced) && (media.MediaType == MediaObjectTypeEnum.Image))
                {
                    var httpStatus = await updateMediaFileAsync(media.RoutePointId, media.RoutePointMediaObjectId,
                        media.MediaType, true);
                    bool resultPreview = httpStatus == HttpStatusCode.OK;
                    _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, true, resultPreview);
                    if (!resultPreview)
                        _log.AddStringEvent(
                            $"media file update {media.RoutePointMediaObjectId}, preview, http status:{HttpStatusCode.OK}");
                    if (result) result = resultPreview;
                }
            }
            else
            {
                _log.AddStringEvent($"image deleted, passed mediaid:{media.RoutePointMediaObjectId}");
                _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, false, true);
                _routePointMediaManager.SetSyncStatus(media.RoutePointMediaObjectId, true, true);
                MediaFileManager fileManager = new MediaFileManager();
                fileManager.Delete(media.RoutePointMediaObjectId, media.MediaType);
                result = true;
            }

            return result;
        }

        private void deleteRouteContain(ViewRoute route)
        {
            _log.AddStringEvent($"delete data for route {_routeId}");

            string pathToPicturesDirectory = ImagePathManager.GetPicturesDirectory();

            var mediasByRoute = _routePointMediaManager.GetMediaObjectsByRouteId(route.RouteId);
            foreach (var media in mediasByRoute)
            {
                ViewRoutePointMediaObject viewMedia = new ViewRoutePointMediaObject();
                viewMedia.Load(media.RoutePointMediaObjectId);
                try
                {
                    string pathToMediaFile = ImagePathManager.GetImagePath(viewMedia.RoutePointMediaObjectId, (MediaObjectTypeEnum)media.MediaType, false);
                    if (File.Exists(pathToMediaFile))
                    {
                        File.Delete(pathToMediaFile);
                    }
                    string pathToMediaFilePreview = ImagePathManager.GetImagePath(viewMedia.RoutePointMediaObjectId, (MediaObjectTypeEnum)media.MediaType, true);
                    if (File.Exists(pathToMediaFilePreview))
                    {
                        File.Delete(pathToMediaFilePreview);
                    }
                }
                catch (Exception e)
                {
                    HandleError.Process("DeleteRouteContain", "NewFile", e, false);
                }
                viewMedia.Delete();
            }

            var pointsByRoute = _routePointManager.GetPointsByRouteId(route.RouteId);
            foreach (var point in pointsByRoute)
            {
                point.Delete();
            }
        }

        private async Task<HttpStatusCode> updateMediaFileAsync(string routePointId, string routePointMediaObjectId, MediaObjectTypeEnum mediaType, bool isPreview)
        {
            string pathToMediaFilesDirectory = ImagePathManager.GetPicturesDirectory();

            return await syncMediaFileAsync(routePointId, routePointMediaObjectId, pathToMediaFilesDirectory, mediaType, isPreview);
        }

        private async Task<HttpStatusCode> syncMediaFileAsync(string routePointId, string mediaId, string pathToPicturesDirectory, MediaObjectTypeEnum mediaType, bool isPreview)
        {
            bool result = false;

            string filename = ImagePathManager.GetMediaFilename(mediaId, mediaType, isPreview);
            string pathToMediaFile = ImagePathManager.GetImagePath(mediaId, mediaType, isPreview);
            if (!File.Exists(pathToMediaFile))
            {
                result = await _routePointMediaObjectsApi.GetImage(routePointId, mediaId, pathToPicturesDirectory, filename);
                _log.AddStringEvent($"get image point:{routePointId},  media:{mediaId}, filename:{filename}, result http status:{_routePointMediaObjectsApi.LastHttpStatusCode}");
            }
            else
            {
                result = await _routePointMediaObjectsApi.SendImage(routePointId, mediaId, mediaType, isPreview);
                _log.AddStringEvent($"send image point:{routePointId},  media:{mediaId}, filename:{filename}, result http status:{_routePointMediaObjectsApi.LastHttpStatusCode}");
            }

            return _routePointMediaObjectsApi.LastHttpStatusCode;
        }

        private StringBuilder getVersionsForRoute(ViewRoute updatedLocalRoute)
        {
            StringBuilder versions = new StringBuilder();

            versions.Append(updatedLocalRoute.Version);

            var points = _routePointManager.GetPointsByRouteId(updatedLocalRoute.RouteId, true).OrderBy(p => p.RoutePointId);
            foreach (var point in points)
            {
                versions.Append(point.Version);
            }

            var medias = _routePointMediaManager.GetMediaObjectsByRouteId(updatedLocalRoute.RouteId).OrderBy(m => m.RoutePointMediaObjectId);
            foreach (var media in medias)
            {
                versions.Append(media.Version);
            }

            return versions;
        }

        private async Task<(bool, List<ViewRoutePointMediaObject>, List<ViewRoutePointMediaObject>)> updateMedias(RouteRoot routeRoot)
        {
            bool updateResult = true;

            //List<string> mediasToUpload = new List<string>();
            List<ViewRoutePointMediaObject> mediasToUpload = new List<ViewRoutePointMediaObject>();
            List<ViewRoutePointMediaObject> mediasToDowload = new List<ViewRoutePointMediaObject>();

            var medias = _routePointMediaManager.GetMediaObjectsByRouteId(routeRoot.Route.Id);

            List<SharedModelsWS.RoutePointMediaObject> serverMedias = new List<SharedModelsWS.RoutePointMediaObject>();
            foreach (var serverPoint in routeRoot.Route.Points)
            {                
                serverMedias.AddRange(serverPoint.MediaObjects.Select(m => m));
            }

            var newMedias = medias.Where(m => !serverMedias.Any(sm => sm.Id == m.RoutePointMediaObjectId));
            //новые медиа
            //var refreshedNewMedias = newMedias.Select(m => new ViewRoutePointMediaObject() {Id = m.RoutePointMediaObjectId});
            var refreshedNewMedias = new List<ViewRoutePointMediaObject>();
            foreach (var mediaItem in newMedias)
            {
                ViewRoutePointMediaObject media = new ViewRoutePointMediaObject();
                media.Load(mediaItem.RoutePointMediaObjectId);
                refreshedNewMedias.Add(media);
            }
            mediasToUpload.AddRange(refreshedNewMedias);

            foreach (var serverMedia in serverMedias)
            {
                var localMedia = _routePointMediaManager.GetMediaObjectById(serverMedia.Id);
                if ((localMedia == null) || (serverMedia.Version > localMedia.Version))
                {
                    ViewRoutePointMediaObject updateMedia = new ViewRoutePointMediaObject();
                    updateMedia.FillFromWSModel(serverMedia);
                    updateResult = updateMedia.Save();
                    mediasToDowload.Add(updateMedia);
                }
                else if (serverMedia.Version < localMedia.Version)
                {
                    //в очередь на отправку
                    //mediasToUpload.Add(serverMedia.Id);
                    var media = new ViewRoutePointMediaObject() {Id = serverMedia.Id};
                    media.Refresh();
                    mediasToUpload.Add(media);
                }
                if (!updateResult) break;

            }

            if (mediasToUpload.Count > 0)
            {
                /*List<ViewRoutePointMediaObject> viewMediasToUpload = new List<ViewRoutePointMediaObject>();
                foreach (string mediaId in mediasToUpload)
                {
                    var media = new ViewRoutePointMediaObject();
                    media.Load(mediaId);
                    viewMediasToUpload.Add(media);
                }*/

                //updateResult = await UploadAsync(GetJsonStructuresMedias(viewMediasToUpload), _routePointMediaObjectsApi);
                updateResult = await UploadAsync(GetJsonStructuresMedias(mediasToUpload), _routePointMediaObjectsApi);
            }

            return (updateResult, mediasToUpload, mediasToDowload);
        }

        private async Task<bool> updatePoints(RouteRoot routeRoot)
        {
            bool updateResult = true;

            List<string> pointsToUpload = new List<string>();
            var pointsByRoute = _routePointManager.GetPointsByRouteId(_routeId, true);
            //если есть новые точки, на отправку
            pointsToUpload.AddRange(pointsByRoute
                .Where(p => !routeRoot.Route.Points.Any(sp => sp.Id == p.RoutePointId)).Select(p => p.Id)
                .ToList());

            foreach (var serverPoint in routeRoot.Route.Points)
            {
                var localPoint = _routePointManager.GetPointById(serverPoint.Id);
                if ((localPoint == null) || (serverPoint.Version > localPoint.Version))
                {
                    ViewRoutePoint updatePoint = new ViewRoutePoint(serverPoint.RouteId, serverPoint.Id);
                    updatePoint.FillFromWSModel(serverPoint);
                    updateResult = updatePoint.Save();
                }
                else if (serverPoint.Version < localPoint.Version)
                {
                    //на сервере более старая версия, в очередь на отправку
                    pointsToUpload.Add(serverPoint.Id);
                }

                if (!updateResult) return false;
            }


            if (pointsToUpload.Count > 0)
            {

                List<ViewRoutePoint> viewPointsToUpload = new List<ViewRoutePoint>();

                foreach (string routePointId in pointsToUpload)
                {
                    viewPointsToUpload.Add(new ViewRoutePoint(routeRoot.Route.Id, routePointId));
                }

                updateResult = await UploadAsync(GetJsonStructuresPoints(viewPointsToUpload), _routePointsApi);
            }

            return updateResult;
        }

        private async Task<bool> updateRoute(string routeServerHash, RouteRoot routeRoot, ViewRoute localRoute)
        {
            bool updateResult = false;
            
            if (routeRoot == null) return updateResult;

            if ((localRoute == null) || (routeRoot.Route?.Version > localRoute.Version))
            {
                ViewRoute updateViewRoute = new ViewRoute(_routeId);
                updateViewRoute.FillFromWSModel(routeRoot, routeServerHash);
                updateResult = updateViewRoute.Save();
                if ((updateResult)&&(!string.IsNullOrEmpty(updateViewRoute.ImgFilename)))
                {
                    updateResult = await _routesApi.DownloadCoverImage(_routeId, updateViewRoute.ImgFilename);
                }
            }
            else if (string.IsNullOrEmpty(routeServerHash))
            {
                updateResult = await UploadAsync(GetRouteJsonStructure(localRoute, ""), _routesApi);
            }
            else if (routeRoot.Route.Version < localRoute.Version)
            {

                string coverImgBase64 = string.Empty;
                if (!string.IsNullOrEmpty(localRoute.ImgFilename))
                {
                    RoutePointMediaObjectRequest mediaRequest = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
                    var httpResult = await mediaRequest.ImageExist(localRoute.ImgFilename);
                    if(httpResult == HttpStatusCode.NotFound)
                    {
                        string pathToCoverImg = Path.Combine(ImagePathManager.GetPicturesDirectory(), localRoute.ImgFilename);
                        var bytes = File.ReadAllBytes(pathToCoverImg);
                        coverImgBase64 = Convert.ToBase64String(bytes);
                    }
                }

                updateResult = await UploadAsync(GetRouteJsonStructure(localRoute, coverImgBase64), _routesApi);
            }
            else if (routeRoot.Route.Version == localRoute.Version)
            {
                updateResult = true;
            }
            return updateResult;
        }

        /*public bool SyncImages
        {
            set
            {
                _syncMediaFiles = value;
            }
            get
            {
                return _syncMediaFiles;
            }
        }*/

        private class MediaForUpdate
        {
            public string RoutePointId { get; set; }
            public string RoutePointMediaObjectId { get; set; }
            public bool OriginalServerSynced { get; set; }
            public bool PreviewServerSynced { get; set; }
            public bool IsDeleted { get; set; }
            public MediaObjectTypeEnum MediaType { get; set; }
        }
    }
}
