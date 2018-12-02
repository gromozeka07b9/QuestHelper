﻿using QuestHelper.LocalDB.Model;
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
    public class SyncMedia : SyncBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutePointsApiRequest _routePointsApi = new RoutePointsApiRequest(_apiUrl);
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl);
        private readonly RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();

        public async Task<bool> Sync()
        {
            bool result = false;

            var medias = _routePointMediaManager.GetMediaObjects().Select(x => new Tuple<string, int>(x.RoutePointMediaObjectId, x.Version));
            SyncObjectStatus mediasServerStatus = await _routePointMediaObjectsApi.GetSyncStatus(medias);
            if (mediasServerStatus != null)
            {
                result = true;

                List<string> forUpload = new List<string>();
                List<string> forDownload = new List<string>();

                FillListsObjectsForProcess(medias, mediasServerStatus, forUpload, forDownload);

                if (forUpload.Count > 0)
                {
                    result = await UploadAsync(GetJsonStructures(forUpload), _routePointMediaObjectsApi);
                    if (result)
                    {
                        result = await SendFilesForMediaList(forUpload);
                    }

                    if (!result) return result;
                }

                if (forDownload.Count > 0)
                {
                    result = await DownloadAsync(forDownload, _routePointMediaObjectsApi);
                }

                if (result)
                {
                    SyncFiles syncFiles = new SyncFiles();
                    result = await syncFiles.CheckExistsAllFilesForMediaAndDownloadIfNeeded();
                }
            }

            return result;
        }

        private async Task<bool> SendFilesForMediaList(List<string> forUpload)
        {
            bool result = false;

            foreach (var mediaId in forUpload)
            {
                var media = _routePointMediaManager.GetMediaObjectById(mediaId);
                result = await _routePointMediaObjectsApi.SendImage(media.RoutePointId, media.RoutePointMediaObjectId);
                if (result)
                {
                    result = await _routePointMediaObjectsApi.SendImage(media.RoutePointId, media.RoutePointMediaObjectId, true);
                }

                if (!result)
                {
                    //ToDo:пока костыль, увеличим версию чтобы при следующей синхронизации повторно отправить файлы. Нужна поддержка на стороне сервера для проверки файлов.
                    //Ошибка может возникнуть при отправке файлов на сервер
                    ViewRoutePointMediaObject viewMedia = new ViewRoutePointMediaObject();
                    viewMedia.Load(mediaId);
                    viewMedia.Version++;
                    viewMedia.Save();
                    break;
                }
            }

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
                        uploadedObject.Version
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }
    }
}
