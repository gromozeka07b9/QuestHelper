using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestHelper.Managers.Sync
{
    public class SyncFiles
    {
        private static SyncFiles _instance;
        private Action<string> _showWarning;

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

        internal void Start()
        {
            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetNotSyncedFiles();
            
            foreach (var media in mediaObjects)
            {
                if((!media.ServerSynced)&&(string.IsNullOrEmpty(media.FileName)))
                {
                    mediaManager.SetSyncStatus(media.RoutePointMediaObjectId, true);
                } else
                {
                    /*if ((!media.ServerSynced) && (!string.IsNullOrEmpty(media.FileName) && (!string.IsNullOrEmpty(media.FileNamePreview))))
                    {
                        UploadMedia(media);
                    }*/
                }

                /*if (!result)
                {
                    _showWarning("Произошла ошибка передачи файлов на сервер. Проверьте подключение к сети и повторите отправку.");
                    break;
                }*/
            }
        }

        public static void UploadMedia(LocalDB.Model.RoutePointMediaObject media)
        {
            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            FileInfo infoFileOriginal = new FileInfo(media.FileName);
            AzureBlobRequest storage = new AzureBlobRequest();
            bool result = storage.SendFile(infoFileOriginal.DirectoryName, infoFileOriginal.Name);
            if (result)
            {
                FileInfo infoFilePreview = new FileInfo(media.FileNamePreview);
                result = storage.SendFile(infoFilePreview.DirectoryName, infoFilePreview.Name);
                mediaManager.SetSyncStatus(media.RoutePointMediaObjectId, result);
            }
        }
    }
}
