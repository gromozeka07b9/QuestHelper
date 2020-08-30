﻿using System;
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

        public List<ViewLocalFile> GetImagesInfo(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            List<ViewLocalFile> listCachedImages = new List<ViewLocalFile>();
            var listDbModel = RealmInstance.All<LocalFile>().Where(l => l.FileNameDate >= periodStart && l.FileNameDate <= periodEnd).OrderBy(l => l.FileNameDate).ToList();

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

        public DateTime GetMinDate()
        {
            var minItem = RealmInstance.All<LocalFile>().OrderBy(l => l.FileNameDate).FirstOrDefault();
            return minItem.FileNameDate.DateTime;
        }
        
        public List<Tuple<DateTime,int>> GetCountImagesByDay(DateTime dateBegin, DateTime dateEnd)
        {
            var countByDays = RealmInstance.All<LocalFile>()
                .Where(f => f.FileNameDate >= dateBegin && f.FileNameDate <= dateEnd)
                .ToList();
            var grouped = countByDays.GroupBy(f =>
                    new DateTime(f.FileNameDate.Year, f.FileNameDate.Month, f.FileNameDate.Day))
                .Select(g => new Tuple<DateTime,int>(g.Key, g.Count())).ToList();
            return grouped;
        }

        public bool Exist(string filename, DateTime fileCreationDate)
        {
            return RealmInstance.All<LocalFile>().Where(f => f.SourceFileName.Equals(filename) && f.FileNameDate == fileCreationDate).Any();
        }

        public List<ViewLocalFile> LocalFilesByDays(DateTime dateBegin, DateTime dateEnd)
        {
            var listCachedFiles = RealmInstance.All<LocalFile>().Where(f => f.FileNameDate >= dateBegin && f.FileNameDate <= dateEnd).ToList().Select(f => new ViewLocalFile(f.LocalFileId)
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
