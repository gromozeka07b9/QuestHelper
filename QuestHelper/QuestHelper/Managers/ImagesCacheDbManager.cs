using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model.Messages;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    /// <summary>
    /// Класс для кеширования свойств изображений в локальной БД
    /// </summary>
    public class ImagesCacheDbManager
    {
        private readonly IImageManager _imageManager;
        private DateTime _dateEnd;
        private DateTime _dateBegin;
        private LocalFileCacheManager _cacheManager;

        public ImagesCacheDbManager(IImageManager imageManager, DateTime dateBegin, DateTime dateEnd)
        {
            _imageManager = imageManager;
            _dateBegin = dateBegin;
            _dateEnd = dateEnd;
            _cacheManager = new LocalFileCacheManager();
        }

        internal string GetPathToCameraDirectory()
        {
            string pathToDCIMDirectory = DependencyService.Get<IPathService>().GetLastUsedDCIMPath();
            MediaFileManager mediaFileManager = new MediaFileManager();
            if(mediaFileManager.GetMediaFilesFromDirectory(new DirectoryInfo(pathToDCIMDirectory)).Any())
            {
                return pathToDCIMDirectory;
            } else
            {
                pathToDCIMDirectory = DependencyService.Get<IPathService>().PublicDirectoryDcim + "/Camera";
            }
            return pathToDCIMDirectory;

        }

        internal string GetPublicDirectoryDcim()
        {
            return DependencyService.Get<IPathService>().PublicDirectoryDcim + "/Camera";
        }

        internal List<string> GetFilenamesForIndexing(string pathToDCIMDirectory)
        {
            MediaFileManager mediaFileManager = new MediaFileManager();
            IEnumerable<string> listFiles = mediaFileManager.GetMediaFilesFromDirectory(new DirectoryInfo(pathToDCIMDirectory)).Select(f=>f.FullName);
            //IEnumerable<string> listFiles = GetListFiles(pathToDCIMDirectory);
            List<string> listFilesDb = _cacheManager.GetFullFilenames(pathToDCIMDirectory);
            return listFiles.Except(listFilesDb).ToList();
        }
        internal void UpdateFilenames(List<string> diffListFiles, string pathToDCIMDirectory)
        {
            var startDate = DateTime.Now;
            int countFiles = 0;
            foreach(string filename in diffListFiles)
            {
                var fileInfo = new FileInfo(filename);
                if(!_cacheManager.Exist(fileInfo.Name, pathToDCIMDirectory, fileInfo.CreationTime))
                {
                    _cacheManager.Save(new ViewLocalFile() {Id = Guid.NewGuid().ToString(), SourceFileName = fileInfo.Name, SourcePath = fileInfo.DirectoryName, FileNameDate = fileInfo.CreationTime });
                }

                if (countFiles % 50 == 0)
                {
                    Xamarin.Forms.MessagingCenter.Send<CurrentProgressIndexMessage>(new CurrentProgressIndexMessage() {Index = countFiles}, string.Empty);
                }
                countFiles++;
            }
            var delay = DateTime.Now - startDate;
            Analytics.TrackEvent("ImagesCacheDb:Update filenames", new Dictionary<string, string> {{"delay", delay.ToString()}, {"pathToDCIMDirectory", pathToDCIMDirectory}, {"countFiles", countFiles.ToString()} });
        }

        internal void UpdateMetadata(string pathToImageDirectory)
        {
            var startDate = DateTime.Now;
            int countFiles = 0;
            var files = _cacheManager.LocalFilesByDays(_dateBegin, _dateEnd, pathToImageDirectory);
            foreach(var currentFile in files)
            {
                if(!currentFile.Processed)
                {
                    _imageManager.SetPreviewImageQuality(ImageQualityType.MiddleSizeHiQuality);
                    var currentMetadata = _imageManager.GetPhoto(currentFile.Id, Path.Combine(currentFile.SourcePath, currentFile.SourceFileName));
                    if (currentMetadata.getMetadataPhotoResult)
                    {
                        currentFile.Latitude = currentMetadata.imageGpsCoordinates.Latitude;
                        currentFile.Longitude = currentMetadata.imageGpsCoordinates.Longitude;
                        currentFile.ImagePreviewFileName = ImagePathManager.GetImagePath(currentFile.Id, LocalDB.Model.MediaObjectTypeEnum.Image, true);
                    }
                    currentFile.Processed = true;
                    _cacheManager.Save(currentFile);
                    if (countFiles % 10 == 0)
                    {
                        Xamarin.Forms.MessagingCenter.Send<CurrentProgressIndexMessage>(new CurrentProgressIndexMessage() {Index = countFiles}, string.Empty);
                    }

                    countFiles++;
                }
            }

            var delay = DateTime.Now - startDate;
            Analytics.TrackEvent("ImagesCacheDb:Update metadata", new Dictionary<string, string> {
                {"delay", delay.ToString()}, 
                {"dateBegin", _dateBegin.ToString(DateTimeFormatInfo.InvariantInfo)}, 
                {"dateEnd", _dateEnd.ToString(DateTimeFormatInfo.InvariantInfo)},
                {"countFiles", files.Count().ToString()} });

        }

        internal int GetCountImagesForDaysAgo(int countDaysAgo, string pathToImageDirectory)
        {
            var currentDate = DateTime.Now;
            var dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            var dateStart = (new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0)).AddDays(-countDaysAgo);
            int count = _cacheManager.GetCountImagesByDay(dateStart, dateEnd, pathToImageDirectory).ToList().Sum(x=>x.Item2);
            return count;
        }
    }
}
