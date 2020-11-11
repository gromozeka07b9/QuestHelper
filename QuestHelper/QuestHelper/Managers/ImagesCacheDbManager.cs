using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
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
            if (GetListFiles(pathToDCIMDirectory).Any())
            {
                return pathToDCIMDirectory;
            } else
            {
                pathToDCIMDirectory = DependencyService.Get<IPathService>().PublicDirectoryDcim + "/Camera";
            }
            return pathToDCIMDirectory;

        }

        internal void UpdateFilenames()
        {
            string pathToDCIMDirectory = DependencyService.Get<IPathService>().PublicDirectoryDcim + "/Camera";
            var startDate = DateTime.Now;
            ParameterManager parameterManager = new ParameterManager();
            if(!parameterManager.Get("CameraDirectoryFullPath", out pathToDCIMDirectory))
            {
#if DEBUG                
                UserDialogs.Instance.Toast("использован путь к фото по-умолчанию");
#endif 
            }

            /*string pathToDCIMDirectory = DependencyService.Get<IPathService>().GetLastUsedDCIMPath();
            if (string.IsNullOrEmpty(pathToDCIMDirectory))
            {
                pathToDCIMDirectory = DependencyService.Get<IPathService>().PublicDirectoryDcim + "/Camera";
#if DEBUG                
                UserDialogs.Instance.Toast("использован путь к фото по-умолчанию");
#endif                
            }*/

            IEnumerable<string> listFiles = GetListFiles(pathToDCIMDirectory);
            int countFiles = 0;
            foreach(string filename in listFiles)
            {
                var fileInfo = new FileInfo(filename);
                if(!_cacheManager.Exist(fileInfo.Name, fileInfo.CreationTime))
                {
                    _cacheManager.Save(new ViewLocalFile() {Id = Guid.NewGuid().ToString(), SourceFileName = fileInfo.Name, SourcePath = fileInfo.DirectoryName, FileNameDate = fileInfo.CreationTime });
                }

                countFiles++;
            }
            var delay = DateTime.Now - startDate;
            Analytics.TrackEvent("ImagesCacheDb:Update filenames", new Dictionary<string, string> {{"delay", delay.ToString()}, {"pathToDCIMDirectory", pathToDCIMDirectory}, {"countFiles", countFiles.ToString()} });
        }

        internal IEnumerable<string> GetListFiles(string pathToDCIMDirectory)
        {
            IEnumerable<string> listFiles = new List<string>();
            try
            {
                listFiles = System.IO.Directory.EnumerateFiles(pathToDCIMDirectory, "*.jpg", SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Analytics.TrackEvent("GetListFiles", new Dictionary<string, string> { { "path", pathToDCIMDirectory } , { "Error", e.Message } });
            }
            return listFiles;
        }

        internal void UpdateMetadata()
        {
            var startDate = DateTime.Now;
            var files = _cacheManager.LocalFilesByDays(_dateBegin, _dateEnd);
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
                }
            }

            var delay = DateTime.Now - startDate;
            Analytics.TrackEvent("ImagesCacheDb:Update metadata", new Dictionary<string, string> {
                {"delay", delay.ToString()}, 
                {"dateBegin", _dateBegin.ToString(DateTimeFormatInfo.InvariantInfo)}, 
                {"dateEnd", _dateEnd.ToString(DateTimeFormatInfo.InvariantInfo)},
                {"countFiles", files.Count().ToString()} });

        }

        internal int GetCountImagesForDaysAgo(int countDaysAgo)
        {
            var currentDate = DateTime.Now;
            var dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            var dateStart = (new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0)).AddDays(-countDaysAgo);
            int count = _cacheManager.GetCountImagesByDay(dateStart, dateEnd).ToList().Sum(x=>x.Item2);
            return count;
        }
    }
}
