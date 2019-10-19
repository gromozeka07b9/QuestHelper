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
        private string _pathToMediaCatalogShared;
        public MediaManager()
        {
#if DEBUG
            _pathToMediaCatalog = "C:\\gosh\\pics\\pictures";
            _pathToMediaCatalogShared = "C:\\gosh\\pics\\pictures\\shared";
#else
            _pathToMediaCatalog = "/media/goshmedia";
            _pathToMediaCatalogShared = "/var/www/www-root/data/wwwroot/wwwroot/shared";            
#endif
        }

        public string PathToMediaCatalog
        {
            get
            {
                return _pathToMediaCatalog; 
            }
        }
        public string PathToSharedMediaCatalog
        {
            get
            {
                return _pathToMediaCatalogShared;
            }
        }
        internal bool MediaFileExist(string filename)
        {
            return File.Exists(Path.Combine(_pathToMediaCatalog, filename));
        }
        internal bool SharedMediaFileExist(string filename)
        {
            return File.Exists(Path.Combine(_pathToMediaCatalogShared, filename));
        }

        internal void DownloadToStream(MemoryStream memoryStream, string filename)
        {
            using (FileStream fileStream = File.OpenRead(Path.Combine(_pathToMediaCatalog, filename)))
            {
                memoryStream.SetLength(fileStream.Length);
                fileStream.Read(memoryStream.GetBuffer(), 0, (int) fileStream.Length);
            }
        }

        internal void CopyMediaFileToSharedCatalog(string filename)
        {
            try
            {
                System.IO.File.Copy(Path.Combine(_pathToMediaCatalog, filename), Path.Combine(_pathToMediaCatalogShared, filename), true);
                Console.WriteLine($"Copied preview: imgFileName:{filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
