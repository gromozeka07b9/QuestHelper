using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using QuestHelper.Model;
using QuestHelper.WS;

namespace QuestHelper.Managers.Sync
{
    public class SyncBase
    {
        protected string _errorReport = string.Empty;
        public bool AuthRequired { get; internal set; }

        public void FillListsObjectsForProcess(IEnumerable<Tuple<string, int>> clientObjects, SyncObjectStatus serverStatus, List<string> forUpload, List<string> forDownload)
        {
            foreach (var serverObject in serverStatus.Statuses)
            {
                //если сервер вернул версию 0, значит на сервере объекта еще нет, его надо будет отправить
                if (serverObject.Version == 0)
                {
                    forUpload.Add(serverObject.ObjectId);
                }
                else
                {
                    //если версия на сервере уже есть, значит есть расхождение версий между сервером и клиентом
                    //тут варианты: если на сервере более старшая версия, клиент должен ее забрать себе
                    //если на сервере младшая версия, то клиент должен отправить свою версию на сервер
                    //Одинаковыми версии быть не могут, если нет изменений, сервер не должен вернуть информацию по маршруту
                    //Если на клиенте маршрута вообще нет, значит грузим его с сервера
                    var objectClient = clientObjects.SingleOrDefault(r => r.Item1 == serverObject.ObjectId);
                    if (objectClient != null)
                    {
                        if (serverObject.Version > objectClient.Item2)
                        {
                            forDownload.Add(serverObject.ObjectId);
                        }
                        else
                        {
                            forUpload.Add(serverObject.ObjectId);
                        }
                    }
                    else
                    {
                        forDownload.Add(serverObject.ObjectId);
                    }
                }
            }
        }
        public async System.Threading.Tasks.Task<bool> DownloadAsync<T>(List<string> idsForDownload, IDownloadable<T> api)
        {
            bool result = true;
            StringBuilder sbErrors = new StringBuilder();
            string syncType = typeof(T).ToString();
            foreach (var id in idsForDownload)
            {
                ISaveable downloadedObject = await api.DownloadFromServerAsync(id);
                AuthRequired = (api.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || api.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                bool resultId = downloadedObject.Save();
                if (!resultId)
                {
                    sbErrors.AppendLine($"downloading {syncType}:{id}");
                    result = resultId;
                }
            }
            _errorReport += sbErrors.ToString();
            return result;
        }
        public async System.Threading.Tasks.Task<bool> UploadAsync<T>(List<string> jsonStructureForUpload, IUploadable<T> api)
        {
            bool result = true;
            StringBuilder sbErrors = new StringBuilder();
            string syncType = typeof(T).ToString();
            foreach (string objectJson in jsonStructureForUpload)
            {
                bool resultId = await api.UploadToServerAsync(objectJson);
                AuthRequired = (api.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || api.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                if (!resultId)
                {
                    sbErrors.AppendLine($"uploading {syncType}:{objectJson}");
                    result = resultId;
                }
            }
            _errorReport += sbErrors.ToString();
            return result;
        }
    }
}
