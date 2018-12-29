using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Model;

namespace QuestHelper.WS
{
    public interface IDownloadable<T>
    {
        Task<ISaveable> DownloadFromServerAsync(string id);
        HttpStatusCode GetLastHttpStatusCode();
    }
}
