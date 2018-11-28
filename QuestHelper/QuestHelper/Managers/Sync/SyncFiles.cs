using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuestHelper.Managers.Sync
{
    public class SyncFiles
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl);

        public SyncFiles()
        {

        }

        internal async Task<bool> CheckExistsAllFilesForMediaAndDownloadIfNeeded()
        {
            bool result = true;

            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetMediaObjects();

            foreach (var mediaObject in mediaObjects)
            {
                string filename = ImagePathManager.GetImageFilename(mediaObject.RoutePointMediaObjectId, true);
                string pathToMediaFile = ImagePathManager.GetImagePath(mediaObject.RoutePointMediaObjectId, true);
                if (!File.Exists(pathToMediaFile))
                {
                    result = await _routePointMediaObjectsApi.GetImage(mediaObject.RoutePointId, mediaObject.RoutePointMediaObjectId, filename);
                    //если не загружено preview, то пробуем загрузить основное фото
                    filename = ImagePathManager.GetImageFilename(mediaObject.RoutePointMediaObjectId);
                    pathToMediaFile = ImagePathManager.GetImagePath(mediaObject.RoutePointMediaObjectId);
                    if (!File.Exists(pathToMediaFile))
                    {
                        result = await _routePointMediaObjectsApi.GetImage(mediaObject.RoutePointId, mediaObject.RoutePointMediaObjectId, filename);
                    }

                    //if (!result) break;
                }
            }

            return result;
        }
    }
}
