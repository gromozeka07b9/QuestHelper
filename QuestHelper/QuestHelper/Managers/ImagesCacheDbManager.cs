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
        private int _depthInDays = 0;//depth in days for choose photos
        private IImageManager _imageManager;

        public ImagesCacheDbManager(IImageManager imageManager, int depthInDays = 0)
        {
            _imageManager = imageManager;
            _depthInDays = depthInDays;
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
                    cacheManager.Save(new ViewLocalFile() { SourceFileName = fileInfo.Name, SourcePath = fileInfo.DirectoryName, FileNameDate = fileInfo.CreationTime });
                }
            }
            var delay = DateTime.Now - startDate;
        }

        internal void UpdateMetadata()
        {
            var startDate = DateTime.Now;
            LocalFileCacheManager cacheManager = new LocalFileCacheManager();
            var files = cacheManager.LocalFilesByDays(7);
            foreach(var currentFile in files)
            {
                if(!currentFile.Processed && currentFile.Latitude == 0 && currentFile.Longitude == 0)
                {
                    var currentMetadata = _imageManager.GetPhoto(Path.Combine(currentFile.SourcePath, currentFile.SourceFileName));
                    if (currentMetadata.getMetadataPhotoResult)
                    {
                        currentFile.Latitude = (long)currentMetadata.imageGpsCoordinates.Latitude;
                        currentFile.Longitude = (long)currentMetadata.imageGpsCoordinates.Longitude;
                        currentFile.ImagePreviewFileName = ImagePathManager.GetImagePath(currentMetadata.newMediaId, LocalDB.Model.MediaObjectTypeEnum.Image, true);
                    }
                    currentFile.Processed = true;
                    cacheManager.Save(currentFile);
                }
            }
            var delay = DateTime.Now - startDate;
        }

        /*public void LoadListImages()
        {
            string pathToPicturesDirectory = DependencyService.Get<IPathService>().PublicDirectoryPictures;
            string pathToDCIMDirectory = DependencyService.Get<IPathService>().PublicDirectoryDcim;

            var _listImages = System.IO.Directory.GetFiles(pathToDCIMDirectory, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png"));
            if (_isShowAllImages)
            {
                var otherImgs = System.IO.Directory.GetFiles(pathToPicturesDirectory, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png"));
                _listImages = _listImages.Concat(otherImgs);
            }
            var _unorderedImages = new List<GalleryImage>();
            foreach (var imgFilename in _listImages)
            {
                var fi = new FileInfo(imgFilename);
                _unorderedImages.Add(new GalleryImage() { ImagePath = imgFilename, CreateDate = fi.CreationTime });
            }
        }*/

        /*public IEnumerable<GalleryImage> GetItems(int lastIndex)
        {
            if (_sortedImages == null)
            {
                LoadListImages();
            }

            return _sortedImages.Skip(lastIndex).Take(_imgPageSize);
        }
        public IEnumerable<GalleryImage> GetRandomItems(int count)
        {
            List<GalleryImage> images = new List<GalleryImage>();
            try
            {
                if (_sortedImages == null)
                {
                    LoadListImages();
                }

                Random rnd = new Random(DateTime.Now.Second);
                for (int i = 0; i < count; i++)
                {
                    int idx = rnd.Next(0, _sortedImages.Count);
                    images.Add(_sortedImages[idx]);
                }
            }
            catch (Exception)
            {

            }
            return images;
        }

        public int Count()
        {
            return _sortedImages != null ? _sortedImages.Count() : 0;
        }*/
    }

}
