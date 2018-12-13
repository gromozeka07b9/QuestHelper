﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.Model;
using QuestHelper.WS;

namespace QuestHelper.Managers.Sync
{
    public class SyncBase
    {
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
            bool result = false;
            foreach (var id in idsForDownload)
            {
                ISaveable downloadedObject = await api.DownloadFromServerAsync(id);
                result = downloadedObject.Save();
                if (!result)
                {
                    break;
                }
            }
            return result;
        }
        public async System.Threading.Tasks.Task<bool> UploadAsync<T>(List<string> jsonStructureForUpload, IUploadable<T> api)
        {
            bool result = false;
            foreach (string objectJson in jsonStructureForUpload)
            {
                result = await api.UploadToServerAsync(objectJson);
                if (!result)
                {
                    break;
                }
            }
            return result;
        }
    }
}