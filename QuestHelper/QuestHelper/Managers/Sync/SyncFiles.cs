using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Managers.Sync
{
    public class SyncFiles
    {
        private static SyncFiles _instance;
        private Action<string> _showWarning;

        private const string _apiUrl = "http://igosh.pro/api";
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl);

        private SyncFiles()
        {

        }

        internal void SetWarningDialogContext(Action<string> showWarning)
        {
            _showWarning = showWarning;
        }

        public static SyncFiles GetInstance()
        {
            if (_instance == null) _instance = new SyncFiles();
            return _instance;
        }

        internal async void CheckExistFileAndDownload()
        {
            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetMediaObjects();

            foreach (var mediaObject in mediaObjects)
            {
                string filename = ImagePathManager.GetImageFilename(mediaObject.RoutePointMediaObjectId, true);
                string pathToMediaFile = ImagePathManager.GetImagePath(mediaObject.RoutePointMediaObjectId, true);
                if (!File.Exists(pathToMediaFile))
                {
                    var result = await _routePointMediaObjectsApi.GetImage(mediaObject.RoutePointId, mediaObject.RoutePointMediaObjectId, filename);

                    filename = ImagePathManager.GetImageFilename(mediaObject.RoutePointMediaObjectId);
                    pathToMediaFile = ImagePathManager.GetImagePath(mediaObject.RoutePointMediaObjectId);
                    if (!File.Exists(pathToMediaFile))
                    {
                        result = await _routePointMediaObjectsApi.GetImage(mediaObject.RoutePointId, mediaObject.RoutePointMediaObjectId, filename);
                    }
                }
            }
        }
    }
}
