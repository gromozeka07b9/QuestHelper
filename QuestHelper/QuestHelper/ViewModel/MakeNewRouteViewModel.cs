using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Consts;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
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

        private List<GalleryImage> _images = new List<GalleryImage>();
        public ICommand ViewPhotoCommand { get; private set; }

        public MakeNewRouteViewModel()
        {
            ViewPhotoCommand = new Command(viewPhotoAsync);
        }

        public void CloseDialog()
        {
         
        }

        public void StartDialog()
        {
            string dir = "/storage/emulated/0/DCIM/Camera";
            var listImages = System.IO.Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Where(f=>f.EndsWith(".jpg")||f.EndsWith(".jpeg")||f.EndsWith(".png"));
            var _unorderedImages = new List<GalleryImage>();
            foreach(var imgFilename in listImages)
            {
                var fi = new FileInfo(imgFilename);
                _unorderedImages.Add(new GalleryImage() { ImagePath = imgFilename, CreateDate = fi.CreationTime });
            }
            _images = _unorderedImages.OrderByDescending(f=>f.CreateDate).ToList();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
        }

        public IEnumerable<NewRoutePoint> RoutePoints
        {
            get
            {
                //var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new MediaPreview() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType }).ToList();
                var list = new List<NewRoutePoint>();
                list.Add(new NewRoutePoint() { Name = "Точка 1"});
                list.Add(new NewRoutePoint() { Name = "Точка 2" });
                list.Add(new NewRoutePoint() { Name = "Точка 3" });
                list.Add(new NewRoutePoint() { Name = "Точка 4" });
                list.Add(new NewRoutePoint() { Name = "Точка 5" });
                list.Add(new NewRoutePoint() { Name = "Точка 6" });
                list.Add(new NewRoutePoint() { Name = "Точка 7" });
                list.Add(new NewRoutePoint() { Name = "Точка 8" });
                list.Add(new NewRoutePoint() { Name = "Точка 9" });
                return list;
            }
        }
        public IEnumerable<GalleryImage> Images
        {
            get
            {
                //var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new MediaPreview() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType }).ToList();
                //var list = new List<GalleryImage>();
                //list.Add(new GalleryImage() { Path = "" });
                //list.Add(new GalleryImage() { Path = "" });
                return _images.Take(10);
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

        /*public int WidthImage
        {
            get
            {
                return Convert.ToInt32(DeviceSize.FullScreenWidth / 3);
            }
        }*/
    }

    public class GalleryImage
    {
        public string ImagePath { get; set; }
        public DateTime CreateDate { get; internal set; }
    }

    public class NewRoutePoint
    {
        public string Name;

    }

}
