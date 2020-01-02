using Microsoft.AppCenter.Analytics;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.View;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RouteCarouselRootViewModel : INotifyPropertyChanged
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly ViewRoute _routeObject;
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private CarouselItem _currentItem;
        private string _routePointId = string.Empty;
        private bool _isMaximumQualityPhoto = true;
        private bool _isVisibleQualityImageSelector = false;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }

        public RouteCarouselRootViewModel(string routeId, string routePointId = "")
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            _routeObject = new ViewRoute(routeId);
            _routePointId = routePointId;
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        public string RouteName
        {
            get { return _routeObject?.Name.ToUpper(); }
        }

        public string RoutePointName
        {
            get { return _currentItem?.RoutePointName.ToUpper(); }
        }

        public string RoutePointDescription
        {
            get
            {
                string text = string.Empty;
                if (string.IsNullOrEmpty(_currentItem?.RoutePointDescription))
                {
                    text = "";
                }
                else text = _currentItem?.RoutePointDescription;
                return text;
            }
        }
        public bool DescriptionIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(_currentItem?.RoutePointDescription);
            }
        }

        public bool IsVisibleQualityImageSelector
        {
            set 
            {
                if (_isVisibleQualityImageSelector != value)
                {
                    _isVisibleQualityImageSelector = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleQualityImageSelector"));
                }
            }
            get
            {
                return _isVisibleQualityImageSelector;
            }
        }

        public bool IsMaximumQualityPhoto
        {
            set
            {
                if(_isMaximumQualityPhoto != value)
                {
                    _isMaximumQualityPhoto = value;
                    if (_isMaximumQualityPhoto)
                    {
                        string fullImgPath = ImagePathManager.GetImagePath(CurrentItem.MediaId, MediaObjectTypeEnum.Image, false);
                        if (!File.Exists(fullImgPath))
                        {
                            LoadFullImage(fullImgPath);
                        }
                        else
                        {
                            if (File.Exists(fullImgPath))
                            {
                                CurrentItem.ImageSource = fullImgPath;
                                CurrentItem.IsFullImage = true;
                            }

                        }
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsMaximumQualityPhoto"));
                }
            }
            get
            {
                return _isMaximumQualityPhoto;
            }
        }

        public List<PointForMap> PointsOnMap { get; } = new List<PointForMap>();

        public CarouselItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (_currentItem != value)
                {
                    _currentItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoutePointName"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoutePointDescription"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DescriptionIsVisible"));
                }
            }
        }

        internal async void LoadFullImage(string fullImgPath)
        {
            if(_routePointMediaObjectsApi == null) await initApi();

            DateTime dtStartLoad = DateTime.Now;
            var task = Task.Run(async () =>
                {
                    string imgName = ImagePathManager.GetMediaFilename(CurrentItem.MediaId, MediaObjectTypeEnum.Image, false);
                    return await _routePointMediaObjectsApi.GetImage(CurrentItem.RoutePointId, CurrentItem.MediaId, ImagePathManager.GetPicturesDirectory(), imgName);
                });
            bool result = task.Result;
            if (File.Exists(fullImgPath))
            {
                CurrentItem.ImageSource = fullImgPath;
                CurrentItem.IsFullImage = true;
            }

            //IsMaximumQualityPhoto = false;
            /*int maxTimeoutSecondsForLoad = 3;
            int loadTimeInSeconds = (DateTime.Now - dtStartLoad).Seconds;
            if (loadTimeInSeconds > maxTimeoutSecondsForLoad)
            {
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 1, Message = CommonResource.CommonMsg_HiImageQualityDisabled }, string.Empty);
                IsMaximumQualityPhoto = false;
                IsVisibleQualityImageSelector = true;
            }
            else
            {
                IsVisibleQualityImageSelector = false;
            }*/
        }

        public List<CarouselItem> CarouselPages
        {
            get
            {
                List<CarouselItem> items = new List<CarouselItem>();
                IEnumerable<ViewRoutePoint> points = new List<ViewRoutePoint>();
                if (!string.IsNullOrEmpty(_routePointId))
                {
                    points = _routePointManager.GetPointsByRouteId(_routeObject.RouteId).Where(p=>p.Id.Equals(_routePointId));
                }
                else
                {
                    points = _routePointManager.GetPointsByRouteId(_routeObject.RouteId);
                }
                if (points.Any())
                {
                    foreach (var point in points)
                    {
                        PointsOnMap.Add(new PointForMap() { Name = point.Name, Description = point.Description, PathToPicture = point.ImagePreviewPathForList, Latitude = point.Latitude, Longitude = point.Longitude});
                        if (point.MediaObjects.Where(m=>m.MediaType == 0).Any())
                        {
                            foreach (var media in point.MediaObjects.Where(m => m.MediaType == 0))
                            {
                                items.Add(new CarouselItem(){RouteId = _routeObject.RouteId, RoutePointId = point.RoutePointId, MediaId = media.RoutePointMediaObjectId, ImageSource = ImagePathManager.GetImagePath(media.RoutePointMediaObjectId, MediaObjectTypeEnum.Image, true), RoutePointName = point.NameText, RoutePointDescription = point.Description, Latitude = point.Latitude, Longitude = point.Longitude });
                            }
                        }
                        else
                        {
                            items.Add(new CarouselItem() { RouteId = _routeObject.RouteId, RoutePointId = point.RoutePointId, MediaId = string.Empty, RoutePointName = point.NameText, RoutePointDescription = point.Description, Latitude = point.Latitude, Longitude = point.Longitude });
                        }
                    }
                }
                return items;
            }
        }

        public async void StartDialogAsync()
        {
            await initApi();
            string _userId = await getUserId();

            //Автора альбома пока не считаем за просмотр
            if (!_routeObject.CreatorId.Equals(_userId))
            {
                Analytics.TrackEvent("Album opened", new Dictionary<string, string> { { "Album", RouteName } });
            }
        }

        private async Task<bool> initApi()
        {
            TokenStoreService token = new TokenStoreService();
            string _authToken = await token.GetAuthTokenAsync();
            if(!string.IsNullOrEmpty(_authToken))
            {
                _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
            }

            return !(_routePointMediaObjectsApi == null);
        }

        private async Task<string> getUserId()
        {
            TokenStoreService token = new TokenStoreService();
            string _userId = await token.GetUserIdAsync();
            return _userId;
        }

        public class CarouselItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private Aspect _imageAspect = Aspect.AspectFill;
            private string _imageSource = string.Empty;
            public ICommand ChangeImageAspectCommand { get; private set; }
            public ICommand ViewPhotoCommand { get; private set; }

            public CarouselItem()
            {
                ChangeImageAspectCommand = new Command(changeImageAspectCommand);
                ViewPhotoCommand = new Command(viewPhotoAsync);
            }
            private void changeImageAspectCommand(object obj)
            {
                if (PhotoImageAspect == Aspect.AspectFit)
                {
                    PhotoImageAspect = Aspect.AspectFill;
                }
                else
                {
                    PhotoImageAspect = Aspect.AspectFit;
                }
            }
            private void viewPhotoAsync()
            {
                var defaultViewerService = DependencyService.Get<IDefaultViewer>();
                string path = (FileImageSource)_imageSource;
                if (!string.IsNullOrEmpty(path))
                {
                    string filePath = File.Exists(path) ? path : path.Replace("_preview", "");
                    defaultViewerService.Show(filePath);
                }
            }

            public bool IsFullImage { get; set; }

            public string ImageSource
            {
                get
                {
                    return _imageSource;
                }
                set
                {
                    if (_imageSource != value)
                    {
                        _imageSource = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                    }
                }
            }
            public string RouteId { get; set; }
            public string RoutePointId { get; set; }
            public string MediaId { get; set; }
            public Aspect PhotoImageAspect
            {
                get
                {
                    return _imageAspect;
                }
                set
                {
                    if (_imageAspect != value)
                    {
                        _imageAspect = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotoImageAspect"));
                    }
                }
            }
            public string RoutePointName { get; set; }
            public string RoutePointDescription { get; set; }
            public double Latitude { get; internal set; }
            public double Longitude { get; internal set; }
        }
    }
}
