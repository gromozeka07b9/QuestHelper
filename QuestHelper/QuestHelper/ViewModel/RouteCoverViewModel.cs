using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Managers.Sync;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RouteCoverViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private RoutePointManager _routePointManager = new RoutePointManager();
        private ViewRoute _vroute;
        private ViewRoutePoint _pointItem;
        private readonly string _routeId;
        private string _creatorName;
        private double _progressValue;
        private bool _isVisibleProgress;
        private bool _isVisibleStartRoute;
        private bool _isVisibleList;
        private ObservableCollection<ViewRoutePoint> _viewPointsOfRoute = new ObservableCollection<ViewRoutePoint>();
        private string _creatorImgUrl;

        public ICommand StartRouteCommand { get; private set; }
        public ICommand BackNavigationCommand { get; private set; }

        /// <summary>
        /// Вызывается при открытии обложки из страницы альбомов, когда маршрут уже существует в локальной БД
        /// </summary>
        /// <param name="viewRoute"></param>
        public RouteCoverViewModel(ViewRoute viewRoute)
        {
            init();

            if (!string.IsNullOrEmpty(viewRoute.Id))
            {
                _routeId = viewRoute.Id;
                _vroute = viewRoute;
            }
            else throw new Exception("viewRoute.Id is empty!");
        }

        /// <summary>
        /// Вызывается при открытии обложки из ленты, в этом случае не все элементы маршрута еще могут быть загружены
        /// </summary>
        /// <param name="viewFeedItem"></param>
        public RouteCoverViewModel(ViewFeedItem viewFeedItem)
        {
            init();

            if (!string.IsNullOrEmpty(viewFeedItem.Id))
            {
                _routeId = viewFeedItem.Id;
                _vroute = new ViewRoute(viewFeedItem.Id);
                _vroute.CreateDate = viewFeedItem.CreateDate;
                _vroute.Description = viewFeedItem.Description;
                _vroute.Name = viewFeedItem.Name;
                _vroute.ImgFilename = viewFeedItem.CoverImage;
                _vroute.CreatorId = viewFeedItem.CreatorId;
                _creatorName = viewFeedItem.CreatorName;
            }
            else throw new Exception("viewFeedItem.Id is empty!");
        }

        private void init()
        {
            StartRouteCommand = new Command(startRouteCommand);
            BackNavigationCommand = new Command(backNavigationCommand);
            //PointsOfRoute = new ObservableCollection<ViewRoutePoint>();
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        private void startRouteCommand(object obj)
        {
            if (!IsVisibleProgress)
            {
                var page = new RouteCarouselRootPage(_vroute.RouteId);
                Navigation.PushModalAsync(page);
            }
        }

        public void CloseDialog()
        {
            MessagingCenter.Unsubscribe<SyncRouteCompleteMessage>(this, string.Empty);
            MessagingCenter.Unsubscribe<SyncProgressImageLoadingMessage>(this, string.Empty);
        }

        public async void StartDialog()
        {
            MessagingCenter.Subscribe<SyncRouteCompleteMessage>(this, string.Empty, (sender) =>
            {
                if (sender.RouteId.Equals(_vroute.Id) && sender.SuccessSync)
                {
                    _vroute = new ViewRoute(_vroute.Id);
                    //updatePoints();
                    IsVisibleList = true;
                    IsVisibleProgress = false;
                    IsVisibleStartRoute = !IsVisibleProgress;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RouteCoverImage"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CreateDateText"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Author"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RowHeightForDescription"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RowHeightForImage"));
                }
            });
            MessagingCenter.Subscribe<SyncProgressImageLoadingMessage>(this, string.Empty, (sender) =>
            {
                if (sender.RouteId.Equals(_vroute.Id))
                {
                    ProgressValue = sender.ProgressValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RowHeightForDescription"));
                }
            });
            MessagingCenter.Subscribe<SyncProgressRouteLoadingMessage>(this, string.Empty, (sender) =>
            {
                if (!string.IsNullOrEmpty(sender.RouteId) && (sender.RouteId.Equals(_vroute.Id)))
                {
                    ProgressValue = sender.ProgressValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RowHeightForDescription"));
                }
            });

            if (string.IsNullOrEmpty(_vroute.ObjVerHash))
            {
                //Это не существующий или не до конца синхронизированный маршрут, синкать в любом случае
                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { RouteId = _vroute.Id, NeedCheckVersionRoute = false}, string.Empty);
                IsVisibleProgress = true;
                IsVisibleList = false;
            }
            else
            {
                //Тут возможны варианты - либо это актуальный маршрут, либо нет, но это надо еще проверить
                //Но показываем точки, и в фоне проверяем версию, только если она отличается, запускаем синхронизацию
                //updatePoints();
                IsVisibleProgress = false;
                IsVisibleList = true;
                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { RouteId = _vroute.Id, NeedCheckVersionRoute = true}, string.Empty);
            }

            IsVisibleStartRoute = IsVisibleList;

            var creator = new ViewUserInfo();
            creator.Load(_vroute.CreatorId);
            Author = creator.Name;
            CreatorImgUrl = creator.ImgUrl;
            if (string.IsNullOrEmpty(Author))
            {
                if(await creator.UpdateFromServerAsync())
                {
                    Author = creator.Name;
                    CreatorImgUrl = creator.ImgUrl;
                }
            }
        }

        public string CreatorImgUrl
        {
            set
            {
                if (_creatorImgUrl != value)
                {
                    _creatorImgUrl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CreatorImgUrl"));
                }
            }
            get
            {
                return !string.IsNullOrEmpty(_creatorImgUrl) ? _creatorImgUrl : "avatar1.png";
            }
        }

        public double ProgressValue
        {
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressValue"));
                }
            }
            get
            {
                return _progressValue;
            }
        }
        public bool IsVisibleList
        {
            set
            {
                if (_isVisibleList != value)
                {
                    _isVisibleList = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleList"));
                }
            }
            get
            {
                return _isVisibleList;
            }
        }

        public bool IsVisibleProgress
        {
            set
            {
                if (_isVisibleProgress != value)
                {
                    _isVisibleProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleProgress"));
                }
            }
            get
            {
                return _isVisibleProgress;
            }
        }
        public bool IsVisibleStartRoute
        {
            set
            {
                if (_isVisibleStartRoute != value)
                {
                    _isVisibleStartRoute = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleStartRoute"));
                }
            }
            get
            {
                return _isVisibleStartRoute;
            }
        }

        public string AnimationName
        {
            get
            {
                var nowDate = DateTime.Now;            
                bool isNewYear = nowDate.Month >= 11 || nowDate.Month == 1;
                
                //return isNewYear ? "newyearloading.json" : "usualloading.json";
                return isNewYear ? AnimationResourceController.GetPath("newyearloading") : AnimationResourceController.GetPath("usualloading");
            }
        }

        public int AnimationSize
        {
            get
            {
                return AnimationName.Contains("newyearloading") ? 150 : 80;
            }
        }

        public int RowHeightForDescription
        {
            get
            {
                return string.IsNullOrEmpty(Description) ? 20 : 128;
            }
        }
        public int RowHeightForImage
        {
            get
            {
                return _isVisibleProgress ? 600 : 500;
            }
        }

        public string RouteCoverImage
        {
            get
            {
                if(string.IsNullOrEmpty(_vroute.CoverImage) || (_vroute.CoverImage.Equals("mount1.png")))
                {
                    return string.Empty;
                } else return _vroute.CoverImage;
            }
        }
        public string Description
        {
            get
            {
                string description = _vroute.Description;
                if (string.IsNullOrEmpty(description))
                {
                    var coupleOfPoints = _routePointManager.GetFirstAndLastPoints(_vroute.RouteId);
                    if (coupleOfPoints.Item1 != null)
                    {
                        description = coupleOfPoints.Item1.Description;
                    }
                }
                return description; 
            }
        }
        public string Name
        {
            get { return _vroute.Name; }
        }
        public string CreateDateText
        {
            get { return _vroute.CreateDate.ToString("yyyy MMMM"); }
        }
        public string Author
        {
            set
            {
                if (_creatorName != value)
                {
                    _creatorName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Author"));
                }
            }

            get { return _creatorName; }
        }
    }
}
