using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Model;

namespace QuestHelper.WS
{
    public interface IUploadable<T>
    {
        Task<bool> UploadToServerAsync(string id);
    }
}
