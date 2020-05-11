using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Consts;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class MakeNewRouteViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ViewPhotoCommand { get; private set; }
        public ICommand ImagesTresholdReachedCommand { get; private set; }

        public ObservableCollection<GalleryImage> Images { get; set; }
        public bool IsBusy { get; set; }

        private ImagesDataStore _imagesDataStore;

        int _imagesPageSize = 10;
        private bool _isShowAllImages;

        public MakeNewRouteViewModel()
        {
            ViewPhotoCommand = new Command(viewPhotoAsync);
            ImagesTresholdReachedCommand = new Command(imagesTresholdReachedCommand);
            _imagesDataStore = new ImagesDataStore(_imagesPageSize, IsShowAllImages);
            Images = new ObservableCollection<GalleryImage>();
        }

        private void imagesTresholdReachedCommand(object obj)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {                    
                    var images = _imagesDataStore.GetItems(Images.Count > 0 ? Images.Count : _imagesPageSize);
                    foreach(var image in images)
                    {
                        Images.Add(image);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("imagesTresholdReachedCommand" + e.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public void CloseDialog()
        {
         
        }

        public void StartDialog()
        {
            _imagesDataStore.LoadListImages();
            imagesTresholdReachedCommand(new object());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
        }

        public IEnumerable<NewRoutePoint> RoutePoints
        {
            get
            {
                //var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new MediaPreview() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType }).ToList();
                var list = new List<NewRoutePoint>();
                list.Add(new NewRoutePoint() { Name = "Точка 1"});
                /*list.Add(new NewRoutePoint() { Name = "Точка 2" });
                list.Add(new NewRoutePoint() { Name = "Точка 3" });
                list.Add(new NewRoutePoint() { Name = "Точка 4" });
                list.Add(new NewRoutePoint() { Name = "Точка 5" });
                list.Add(new NewRoutePoint() { Name = "Точка 6" });
                list.Add(new NewRoutePoint() { Name = "Точка 7" });
                list.Add(new NewRoutePoint() { Name = "Точка 8" });
                list.Add(new NewRoutePoint() { Name = "Точка 9" });*/
                return list;
            }
        }

        private void viewPhotoAsync(object imageSource)
        {
            string path = string.Empty;
            var defaultViewerService = DependencyService.Get<IDefaultViewer>();
            if (imageSource is GalleryImage)
            {
                path = ((GalleryImage)imageSource).ImagePath;
            }
            if (!string.IsNullOrEmpty(path))
            {
                defaultViewerService.Show(path.Replace("_preview", ""));
            }
        }

        public bool IsShowAllImages
        {
            set
            {
                if (value != _isShowAllImages)
                {
                    _isShowAllImages = value;
                    _imagesDataStore = new ImagesDataStore(_imagesPageSize, _isShowAllImages);
                    _imagesDataStore.LoadListImages();
                    Images = new ObservableCollection<GalleryImage>();
                    imagesTresholdReachedCommand(new object());
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                }
            }
            get
            {
                return _isShowAllImages;
            }
        }
    }

    public class GalleryImage
    {
        public string ImagePath { get; set; }
        public DateTime CreateDate { get; internal set; }
        public string ImageCreateDateString
        {
            get
            {
                return CreateDate.ToString("dd MMMM yyyy");
            }
        }
    }

    public class NewRoutePoint
    {
        public string Name;

    }

    public class ImagesDataStore
    {
        //private string _dir = "/storage/emulated/0/DCIM/Camera";
        //private string _dir = "/storage/emulated/0/Pictures";
        private List<GalleryImage> _sortedImages;
        private int _imgPageSize = 0;
        private bool _isShowAllImages = false;//false - show DCIM else all pictures

        public ImagesDataStore(int imgPageSize, bool isShowAllImages)
        {
            _imgPageSize = imgPageSize;
            _isShowAllImages = isShowAllImages;
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
        }

        public IEnumerable<GalleryImage> GetItems(int lastIndex)
        {
            if(_sortedImages == null)
            {
                LoadListImages();
            }

            return _sortedImages.Skip(lastIndex).Take(_imgPageSize);
        }
    }

}
