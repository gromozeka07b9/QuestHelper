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

        public static SyncFiles GetInstance()
        {
            if (_instance == null) _instance = new SyncFiles();
            return _instance;
        }

        internal void Start()
        {
            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            var mediaObjects = mediaManager.GetNotSyncedFiles();
            AzureBlobRequest storage = new AzureBlobRequest();
            foreach (var media in mediaObjects)
            {
                if((!media.ServerSynced)&&(!string.IsNullOrEmpty(media.FileName)&& (!string.IsNullOrEmpty(media.FileNamePreview))))
                {
                    FileInfo infoFileOriginal = new FileInfo(media.FileName);
                    bool result = storage.SendFile(infoFileOriginal.DirectoryName, infoFileOriginal.Name);
                    if(result)
                    {
                        FileInfo infoFilePreview = new FileInfo(media.FileNamePreview);
                        result = storage.SendFile(infoFilePreview.DirectoryName, infoFilePreview.Name);
                        mediaManager.SetSyncStatus(media.RoutePointMediaObjectId, result);
                    }
                }
                /*if (!result)
                {
                    _showWarning("Произошла ошибка передачи файлов на сервер. Проверьте подключение к сети и повторите отправку.");
                    break;
                }*/
            }
        }

        internal void SetWarningDialogContext(Action<string> showWarning)
        {
            _showWarning = showWarning;
        }
    }
}
