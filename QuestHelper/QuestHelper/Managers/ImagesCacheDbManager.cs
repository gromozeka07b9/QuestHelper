using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public ImagesCacheDbManager(IImageManager imageManager, DateTime dateBegin, DateTime dateEnd)
        {
            _imageManager = imageManager;
            _dateBegin = dateBegin;
            _dateEnd = dateEnd;
        }

        internal void UpdateFilenames()
        {
            var startDate = DateTime.Now;
            LocalFileCacheManager cacheManager = new LocalFileCacheManager();
            string pathToDCIMDirectory = DependencyService.Get<IPathService>().PublicDirectoryDcim;

            var listFiles = System.IO.Directory.EnumerateFiles(pathToDCIMDirectory, "*.jpg", SearchOption.AllDirectories);
            foreach(string filename in listFiles)
            {
                var fileInfo = new FileInfo(filename);
                if(!cacheManager.Exist(fileInfo.Name, fileInfo.CreationTime))
                {
                    cacheManager.Save(new ViewLocalFile() {Id = Guid.NewGuid().ToString(), SourceFileName = fileInfo.Name, SourcePath = fileInfo.DirectoryName, FileNameDate = fileInfo.CreationTime });
                }
            }
            var delay = DateTime.Now - startDate;
        }

        internal void UpdateMetadata()
        {
            var startDate = DateTime.Now;
            LocalFileCacheManager cacheManager = new LocalFileCacheManager();
            var files = cacheManager.LocalFilesByDays(_dateBegin, _dateEnd);
            foreach(var currentFile in files)
            {
                if(!currentFile.Processed)
                {
                    var currentMetadata = _imageManager.GetPhoto(currentFile.Id, Path.Combine(currentFile.SourcePath, currentFile.SourceFileName));
                    if (currentMetadata.getMetadataPhotoResult)
                    {
                        currentFile.Latitude = currentMetadata.imageGpsCoordinates.Latitude;
                        currentFile.Longitude = currentMetadata.imageGpsCoordinates.Longitude;
                        currentFile.ImagePreviewFileName = ImagePathManager.GetImagePath(currentFile.Id, LocalDB.Model.MediaObjectTypeEnum.Image, true);
                    }
                    currentFile.Processed = true;
                    cacheManager.Save(currentFile);
                }
            }

            var delay = DateTime.Now - startDate;
        }
    }
}
