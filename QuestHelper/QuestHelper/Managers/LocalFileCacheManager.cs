using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;
using Xamarin.Essentials;

namespace QuestHelper.Managers
{
    public class LocalFileCacheManager : RealmInstanceMaker
    {
        public LocalFileCacheManager()
        {
        }

        internal void Delete(string id)
        {
            try
            {
                RealmInstance.Write(() =>
                {
                    var dbObject = RealmInstance.Find<LocalFile>(id);
                    if (dbObject != null)
                    {
                        RealmInstance.Remove(dbObject);
                    }
                });
            }
            catch (Exception e)
            {
                HandleError.Process("LocalFileCacheManager", "Delete", e, false);
            }
        }

        internal void DeleteAll()
        {
            try
            {
                RealmInstance.Write(() =>
                {
                    RealmInstance.RemoveAll<LocalFile>();
                });
            }
            catch (Exception e)
            {
                HandleError.Process("LocalFileCacheManager", "DeleteAll", e, false);
            }
        }

        public bool Save(ViewLocalFile viewItem)
        {
            bool result = false;
            try
            {
                RealmInstance.Write(() =>
                {
                    var dbObject = !string.IsNullOrEmpty(viewItem.Id) ? RealmInstance.Find<LocalFile>(viewItem.Id) : null;
                    if (null == dbObject)
                    {
                        dbObject = string.IsNullOrEmpty(viewItem.Id) ? new LocalFile() : new LocalFile() { LocalFileId = viewItem.Id };
                        RealmInstance.Add(dbObject);
                    }
                    dbObject.Address = viewItem.Address;
                    dbObject.Country = viewItem.Country;
                    dbObject.FileNameDate = viewItem.FileNameDate;
                    dbObject.CreateDate = viewItem.CreateDate;
                    dbObject.ImagePreviewFileName = viewItem.ImagePreviewFileName;
                    dbObject.Latitude = viewItem.Latitude;
                    dbObject.Longitude = viewItem.Longitude;
                    dbObject.SourceFileName = viewItem.SourceFileName;
                    dbObject.SourcePath = viewItem.SourcePath;
                    dbObject.Processed = viewItem.Processed;
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("LocalFileCacheManager", "Save", e, false);
            }
            return result;
        }

        public string GetFullPathAndFilename(string mediaId)
        {
            var objLocalFile = RealmInstance.Find<LocalFile>(mediaId);
            return Path.Combine(objLocalFile.SourcePath, objLocalFile.SourceFileName);
        }

        public List<ViewLocalFile> GetImagesInfo(DateTimeOffset periodStart, DateTimeOffset periodEnd,
            string pathToImageDirectory)
        {
            List<ViewLocalFile> listCachedImages = new List<ViewLocalFile>();
            var listDbModel = RealmInstance.All<LocalFile>().Where(l => l.FileNameDate >= periodStart && l.FileNameDate <= periodEnd && l.SourcePath.Equals(pathToImageDirectory)).OrderBy(l => l.FileNameDate).ToList();

            return listDbModel.Select(l=>new ViewLocalFile(l.LocalFileId) 
            { 
                Address = l.Address,
                Country = l.Country,
                CreateDate = l.CreateDate,
                FileNameDate = l.FileNameDate,
                ImagePreviewFileName = l.ImagePreviewFileName,                
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                SourceFileName = l.SourceFileName,
                SourcePath = l.SourcePath,
                Processed = l.Processed
            }).ToList();
        }
        
        public List<string> GetFullFilenames(string pathToImageDirectory)
        {
            List<string> listCachedImages = new List<string>();
            var listDbModel = RealmInstance.All<LocalFile>().Where(l => l.SourcePath.Equals(pathToImageDirectory)).ToList();

            return listDbModel.Select(l => Path.Combine(pathToImageDirectory, l.SourceFileName)).ToList();
        }

        public DateTime GetMinDate(string pathToImageDirectory)
        {
            var localFiles = RealmInstance.All<LocalFile>().Where(l=>l.SourcePath.Equals(pathToImageDirectory)).OrderBy(l => l.FileNameDate);
            var minItem = localFiles.FirstOrDefault();
            return minItem?.FileNameDate.DateTime ?? DateTime.Now;
        }

        public DateTime GetMaxDate(string pathToImageDirectory)
        {
            var localFiles = RealmInstance.All<LocalFile>().Where(l=>l.SourcePath.Equals(pathToImageDirectory)).OrderBy(l => l.FileNameDate);
            var maxItem = localFiles.LastOrDefault();
            return maxItem?.FileNameDate.DateTime ?? DateTime.Now;
        }
        
        public List<Tuple<DateTime, int>> GetCountImagesByDay(DateTime dateBegin, DateTime dateEnd,
            string pathToImageDirectory)
        {
            var countByDays = RealmInstance.All<LocalFile>()
                .Where(f => f.FileNameDate >= dateBegin && f.FileNameDate <= dateEnd && f.SourcePath.Equals(pathToImageDirectory))
                .ToList();
            var grouped = countByDays.GroupBy(f =>
                    new DateTime(f.FileNameDate.Year, f.FileNameDate.Month, f.FileNameDate.Day))
                .Select(g => new Tuple<DateTime,int>(g.Key, g.Count())).ToList();
            return grouped;
        }

        public bool Exist(string filename, string pathToDcimDirectory, DateTime fileCreationDate)
        {
            return RealmInstance.All<LocalFile>().Any(f => f.SourceFileName.Equals(filename) && f.FileNameDate == fileCreationDate && f.SourcePath.Equals(pathToDcimDirectory));
        }

        public List<ViewLocalFile> LocalFilesByDays(DateTime dateBegin, DateTime dateEnd, string pathToImageDirectory)
        {
            var listCachedFiles = RealmInstance.All<LocalFile>().Where(f => f.FileNameDate >= dateBegin && f.FileNameDate <= dateEnd && f.SourcePath.Equals(pathToImageDirectory)).ToList().Select(f => new ViewLocalFile(f.LocalFileId)
            {
                Address = f.Address,
                Country = f.Country,
                CreateDate = f.CreateDate,
                FileNameDate = f.FileNameDate,
                ImagePreviewFileName = f.ImagePreviewFileName,
                Latitude = (long)f.Latitude,
                Longitude = (long)f.Longitude,
                SourceFileName = f.SourceFileName,
                SourcePath = f.SourcePath,
                Processed = f.Processed
            }).ToList();
            return listCachedFiles;
        }
    }
}
