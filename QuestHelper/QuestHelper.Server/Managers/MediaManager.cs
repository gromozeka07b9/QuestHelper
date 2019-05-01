using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Managers
{
    public class MediaManager
    {
        private string _pathToMediaCatalog;
        public MediaManager(string pathToMediaCatalog)
        {
            _pathToMediaCatalog = pathToMediaCatalog;
        }

        internal bool FileExist(string filename)
        {
            return File.Exists(Path.Combine(_pathToMediaCatalog, filename));
        }

        internal void DownloadToStream(MemoryStream memoryStream, string filename)
        {
            using (FileStream fileStream = File.OpenRead(Path.Combine(_pathToMediaCatalog, filename)))
            {
                memoryStream.SetLength(fileStream.Length);
                fileStream.Read(memoryStream.GetBuffer(), 0, (int) fileStream.Length);
            }
        }
    }
}
