using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuestHelper.Managers.Sync
{
    public class SyncFiles
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly string _authToken = string.Empty;
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        public bool AuthRequired { get; internal set; }

        public SyncFiles(string authToken)
        {
            _authToken = authToken;
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
        }

        internal async Task<bool> CheckExistsAllImageAndDownloadIfNeeded(bool IsPreview)
        {
            bool result = true;

            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetAllMediaObjects();

            foreach (var mediaObject in mediaObjects)
            {
                string filename = ImagePathManager.GetImageFilename(mediaObject.RoutePointMediaObjectId, IsPreview);
                string pathToPicturesDirectory = ImagePathManager.GetPicturesDirectory();
                string pathToMediaFile = ImagePathManager.GetImagePath(mediaObject.RoutePointMediaObjectId, IsPreview);
                if (!File.Exists(pathToMediaFile))
                {
                    result = await _routePointMediaObjectsApi.GetImage(mediaObject.RoutePointId, mediaObject.RoutePointMediaObjectId, pathToPicturesDirectory, filename);
                    AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                }
            }

            return result;
        }
    }
}
