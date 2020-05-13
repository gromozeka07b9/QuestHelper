using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public class ImagesDataStoreManager
    {
        //private string _dir = "/storage/emulated/0/DCIM/Camera";
        //private string _dir = "/storage/emulated/0/Pictures";
        private List<GalleryImage> _sortedImages;
        private int _imgPageSize = 0;
        private bool _isShowAllImages = false;//false - show DCIM else all pictures
        private int _depthInDays = 0;//depth in days for choose photos

        public ImagesDataStoreManager(int imgPageSize, bool isShowAllImages, int depthInDays = 0)
        {
            _imgPageSize = imgPageSize;
            _isShowAllImages = isShowAllImages;
            _depthInDays = depthInDays;
        }

        public void LoadListImages()
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
            _sortedImages = _unorderedImages.OrderByDescending(f => f.CreateDate).ToList();
            if(_depthInDays > 0)
            {
                var fromDate = DateTime.Now.AddDays(-_depthInDays);
                _sortedImages = _sortedImages.Where(i => i.CreateDate >= fromDate).ToList();
            }
        }

        public IEnumerable<GalleryImage> GetItems(int lastIndex)
        {
            if (_sortedImages == null)
            {
                LoadListImages();
            }

            return _sortedImages.Skip(lastIndex).Take(_imgPageSize);
        }

        public int Count()
        {
            return _sortedImages != null ? _sortedImages.Count() : 0;
        }
    }

}
