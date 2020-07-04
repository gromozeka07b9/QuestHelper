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

        public bool Exist(string filename, DateTime fileCreationDate)
        {
            return RealmInstance.All<LocalFile>().Where(f => f.SourceFileName.Equals(filename) && f.FileNameDate == fileCreationDate).Any();
        }

        public List<ViewLocalFile> LocalFilesByDays(int depth)
        {
            var maxDate = DateTime.Now.AddDays(-depth);
            var listCachedFiles = RealmInstance.All<LocalFile>().Where(f => f.FileNameDate >= maxDate).ToList().Select(f => new ViewLocalFile(f.LocalFileId)
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
