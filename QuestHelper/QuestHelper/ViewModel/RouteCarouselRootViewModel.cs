using Microsoft.AppCenter.Analytics;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RouteCarouselRootViewModel : INotifyPropertyChanged
    {
        private const string _apiUrl = "https://igosh.pro/api";
        private readonly ViewRoute _vRoute;
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager routePointMediaObjectManager = new RoutePointMediaObjectManager();
        private RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private CarouselItem _currentItem;
        private bool _isMaximumQualityPhoto = false;
        private bool _isVisibleQualityImageSelector = false;
        private List<CarouselItem> _carouselPages;
        private List<ImagePreviewItem> _currentPointImagesPreview;
        private int _prevCarouselPointsSelectedIndex;
        private int _carouselPointsSelectedIndex;
        private bool _isMapShow;

        //private string _currentPreviewFile;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand CardsItemAppearedCommand { get; private set; }
        public ICommand ShowOtherPhotoCommand { get; private set; }
        public ICommand SwipeDescriptionRightCommand { get; private set; }
        public ICommand SwipeDescriptionLeftCommand { get; private set; }
        //public ICommand MapIconTappedCommand { get; private set; }

        public RouteCarouselRootViewModel(string routeId)
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            CardsItemAppearedCommand = new Command(cardsItemAppearedCommand);
            ShowOtherPhotoCommand = new Command(showOtherPhotoCommand);
            SwipeDescriptionRightCommand = new Command(swipeDescriptionRightCommand);
            SwipeDescriptionLeftCommand = new Command(swipeDescriptionLeftCommand);
            //MapIconTappedCommand = new Command(mapIconTappedCommand);
            _vRoute = new ViewRoute(routeId);
        }

        /*private void mapIconTappedCommand(object obj)
        {
            IsMapShow = !IsMapShow;
        }*/

        private void swipeDescriptionLeftCommand(object obj)
        {
            if((_carouselPages != null) && (CarouselPointsSelectedIndex < _carouselPages.Count - 1))
            {
                CarouselPointsSelectedIndex++;
            }
        }

        private void swipeDescriptionRightCommand(object obj)
        {
            if ((_carouselPages != null) && (CarouselPointsSelectedIndex > 0))
            {
                CarouselPointsSelectedIndex--;
            }
        }

        private void showOtherPhotoCommand(object obj)
        {
            //var customImg = (CustomCachedImage)obj;
            var customImg = (ImagePreviewItem)obj;
            CurrentItem.ImageSource = customImg.ImageSource;
            CurrentItem.MediaId = customImg.RoutePointMediaId;
            CurrentItem.MediaType = (MediaObjectTypeEnum)customImg.MediaType;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CarouselPages"));
            CurrentItem.IsFullImage = false;
            startWaitLoadOriginal(CurrentItem);
        }

        private void cardsItemAppearedCommand(object obj)
        {
            var cardArgs = (PanCardView.EventArgs.ItemAppearedEventArgs)obj;
            if (cardArgs.Item != null)
            {
                CurrentItem = (RouteCarouselRootViewModel.CarouselItem)cardArgs.Item;

                var pointMediaObjects = routePointMediaObjectManager.GetMediaObjectsByRoutePointId(CurrentItem.RoutePointId);
                CurrentPointImagesPreview = pointMediaObjects.Select(media => new ImagePreviewItem() { RoutePointId = CurrentItem.RoutePointId, RoutePointMediaId = media.RoutePointMediaObjectId, MediaType = media.MediaType, ImageSource = ImagePathManager.GetImagePath(media.RoutePointMediaObjectId, (MediaObjectTypeEnum)media.MediaType, true) }).ToList();

                startWaitLoadOriginal(CurrentItem);
            }
            GC.Collect();
        }

        private void startWaitLoadOriginal(CarouselItem newItem)
        {
            if ((!newItem.IsFullImage) && (IsMaximumQualityPhoto))
            {
                Device.StartTimer(TimeSpan.FromMilliseconds(400), OnTimerForUpdate);
            }
        }

        private bool OnTimerForUpdate()
        {
            string fullImgPath = ImagePathManager.GetImagePath(CurrentItem.MediaId, MediaObjectTypeEnum.Image, false);
            if (File.Exists(fullImgPath))
            {
                CurrentItem.ImageSource = fullImgPath;
                CurrentItem.IsFullImage = true;
            }
            else
            {
                if (IsMaximumQualityPhoto)
                {
                    LoadFullImage(fullImgPath);
                }
            }
            GC.Collect();
            return false;
        }

        internal async void LoadFullImage(string fullImgPath)
        {
            if (_routePointMediaObjectsApi == null) await initApi();

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
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        public async void StartDialogAsync()
        {
            IsMaximumQualityPhoto = (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi) ||
                                     Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Ethernet));
            await initApi();
            string _userId = await getUserId();

            //Автора альбома пока не считаем за просмотр
            if (!_vRoute.CreatorId.Equals(_userId))
            {
                Analytics.TrackEvent("Album opened", new Dictionary<string, string> { { "Album", RouteName } });
            }
        }

        private async Task<bool> initApi()
        {
            TokenStoreService token = new TokenStoreService();
            string _authToken = await token.GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(_authToken))
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

        public int CarouselRowHeight => Convert.ToInt32(Consts.DeviceSize.FullScreenHeight * 0.7);

        public string RouteName
        {
            get { return _vRoute?.Name.ToUpper(); }
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
                else text = _currentItem.RoutePointDescription;
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
        public bool IsVisiblePreviewImgList
        {
            get
            {
                return CurrentPointImagesPreview != null && CurrentPointImagesPreview.Count > 1;
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

        public bool IsMapShow
        {
            /*set
            {
                if(_isMapShow != value)
                {
                    _isMapShow = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsMapShow"));
                }
            }*/
            get
            {
                return true;
            }
        }
        public List<PointForMap> PointsOnMap
        {
            get
            {
                var points = CarouselPages.Select(p => new PointForMap() 
                { 
                    Name = p.RoutePointName,
                    Description = p.RoutePointDescription,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    PathToPicture = ImagePathManager.GetImagePath(p.MediaId, MediaObjectTypeEnum.Image, true)
                }).ToList();

                return points;
            }
        }

        public CarouselItem CurrentItem
        {
            get 
            { 
                return _currentItem; 
            }
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

        public int CarouselPointsSelectedIndex
        {
            get
            {
                return _carouselPointsSelectedIndex;
            }
            set
            {
                if(value != _carouselPointsSelectedIndex)
                {
                    _prevCarouselPointsSelectedIndex = _carouselPointsSelectedIndex;
                    _carouselPointsSelectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CarouselPointsSelectedIndex"));
                }
            }
        }
        public List<ImagePreviewItem>  CurrentPointImagesPreview
        {
            get { 
                    return _currentPointImagesPreview; 
                }
            set
            {
                if (_currentPointImagesPreview != value)
                {
                    _currentPointImagesPreview = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPointImagesPreview"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisiblePreviewImgList"));
                }
            }
        }

        public List<CarouselItem> CarouselPages
        {
            get
            {
                if(_carouselPages == null)
                {
                    var points = _routePointManager.GetPointsByRouteId(_vRoute.RouteId).Select(point => new CarouselItem() 
                    { 
                        RouteId = _vRoute.RouteId, 
                        RoutePointId = point.RoutePointId, 
                        MediaId = point.ImageMediaId,
                        MediaType = point.ImageMediaType,
                        ImageSource = point.ImagePreviewPath, 
                        RoutePointName = point.NameText, 
                        RoutePointDescription = point.Description, 
                        Latitude = point.Latitude, 
                        Longitude = point.Longitude 
                    });
                    _carouselPages = points.ToList();
                }
                return _carouselPages;
            }
        }

        public class ImagePreviewItem
        {
            public string RoutePointId { get; set; }
            public string RoutePointMediaId { get; set; }
            public FileImageSource ImageSource { get; set; }
            public int MediaType { get; internal set; }
        }

        public class CarouselItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private Aspect _imageAspect = Aspect.AspectFill;
            private ImageSource _imageSource;
            public ICommand ChangeImageAspectCommand { get; private set; }
            public ICommand ViewPhotoCommand { get; private set; }

            public CarouselItem()
            {
                ChangeImageAspectCommand = new Command(changeImageAspectCommand);
                ViewPhotoCommand = new Command(viewPhotoAsync);
            }
            private void changeImageAspectCommand(object obj)
            {
                if(MediaType == MediaObjectTypeEnum.Audio)
                {
                    PhotoImageAspect = Aspect.AspectFit;
                }
                else
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

            public ImageSource ImageSource
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
            public MediaObjectTypeEnum MediaType { get; set; }
        }
    }
}
