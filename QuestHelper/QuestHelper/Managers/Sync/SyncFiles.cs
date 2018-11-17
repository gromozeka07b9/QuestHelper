using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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

        internal async void Start()
        {
            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetMediaObjects();

            var media = await _routePointMediaObjectsApi.GetFile("36750d40-fd54-468a-8cd5-4af972b54be8", "2ba9c945-3c41-4fef-9d07-aaebe15b11cd", "img_e5074bd4-0e50-4d20-b43d-2b0fe28902f8.jpg");
                /// data / user / 0 / com.sd.QuestHelper / files /.config / img_e5074bd4 - 0e50 - 4d20 - b43d - 2b0fe28902f8.jpg
            /*foreach (var mediaObject in mediaObjects)
            {
                string filename = $"img_{mediaObject.RoutePointMediaObjectId}.jpg";
                if (!File.Exists($"./Photos/{filename}"))
                {
                    var media = await _routePointMediaObjectsApi.GetFile(mediaObject.RoutePointId, mediaObject.RoutePointId, filename);

                }
            }*/
        }
    }
}
