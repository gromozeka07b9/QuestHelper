using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.WS
{
    public interface ICloudStoreFiles
    {
        bool SendFile(string filePath, string fileName);
        void ReceiveFile(string fileId, string filePath);
    }
}
