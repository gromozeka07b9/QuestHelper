using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Consts;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
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

        private ImagesDataStoreManager _imagesDataStore;

        int _imagesPageSize = 10;
        private bool _isShowAllImages;

        public MakeNewRouteViewModel()
        {
            ViewPhotoCommand = new Command(viewPhotoAsync);
            ImagesTresholdReachedCommand = new Command(imagesTresholdReachedCommand);
            _imagesDataStore = new ImagesDataStoreManager(_imagesPageSize, IsShowAllImages);
            Images = new ObservableCollection<GalleryImage>();
        }

        private void imagesTresholdReachedCommand(object obj)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {                    
                    var images = _imagesDataStore.GetItems(Images.Count);
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
                    _imagesDataStore = new ImagesDataStoreManager(_imagesPageSize, _isShowAllImages);
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



    public class NewRoutePoint
    {
        public string Name;

    }


}
