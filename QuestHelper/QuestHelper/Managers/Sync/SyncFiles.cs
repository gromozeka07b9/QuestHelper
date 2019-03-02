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
        protected string _errorReport = string.Empty;
        public bool AuthRequired { get; internal set; }

        public SyncFiles(string authToken)
        {
            _authToken = authToken;
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
        }

        internal async Task<bool> CheckExistsAllImageAndDownloadIfNeeded(bool IsPreview)
        {
            bool result = true;
            StringBuilder sbErrors = new StringBuilder();

            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetAllMediaObjects();

            foreach (var mediaObject in mediaObjects)
            {
                if (!mediaObject.IsDeleted)
                {
                    string filename = ImagePathManager.GetImageFilename(mediaObject.RoutePointMediaObjectId, IsPreview);
                    string pathToPicturesDirectory = ImagePathManager.GetPicturesDirectory();
                    string pathToMediaFile = ImagePathManager.GetImagePath(mediaObject.RoutePointMediaObjectId, IsPreview);
                    if (!File.Exists(pathToMediaFile))
                    {
                        result = await _routePointMediaObjectsApi.GetImage(mediaObject.RoutePointId, mediaObject.RoutePointMediaObjectId, pathToPicturesDirectory, filename);
                        AuthRequired = (_routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routePointMediaObjectsApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                        if (!result)
                        {
                            sbErrors.AppendLine($"downl: p:{mediaObject.RoutePointId}, f:{filename}, result:{result}");
                        }
                    }
                }
            }

            _errorReport += sbErrors.ToString();

            return result;
        }

        public string ErrorReport
        {
            get { return _errorReport; }
        }
    }
}
