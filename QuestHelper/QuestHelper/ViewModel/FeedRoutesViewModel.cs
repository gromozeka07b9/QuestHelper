using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuestHelper.Model.Messages;
using QuestHelper.Model;
using Acr.UserDialogs;
using QuestHelper.WS;
using Xamarin.Essentials;

namespace QuestHelper.ViewModel
{
    class FeedRoutesViewModel : INotifyPropertyChanged
    {
        private string _feedItemId;
        private IEnumerable<ViewFeedItem> _feedItems = new List<ViewFeedItem>();
        private ViewFeedItem _feedItem = new ViewFeedItem();
        //private RouteManager _routeManager = new RouteManager();

        //private RoutesApiRequest _api = new RoutesApiRequest("http://questhelperserver.azurewebsites.net");
        private int _countOfUpdateListByTimer = 0;
        private bool _noItemsWarningIsVisible = false;
        private bool _isRefreshing = false;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RefreshFeedCommand { get; private set; }

        public FeedRoutesViewModel()
        {
            RefreshFeedCommand = new Command(refreshFeedCommandAsync);
        }

        public void startDialog()
        {
            refreshFeedCommandAsync();
        }

        internal void closeDialog()
        {
        }

        async void refreshFeedCommandAsync()
        {
            IsRefreshing = true;
            TokenStoreService tokenService = new TokenStoreService();
            FeedApiRequest feedApi = new FeedApiRequest(await tokenService.GetAuthTokenAsync());
            var listFeedItems = await feedApi.GetFeed();
            if (feedApi.LastHttpStatusCode == HttpStatusCode.OK)
            {
                var items = new List<ViewFeedItem>();
                foreach (var item in listFeedItems)
                {
                    items.Add(new ViewFeedItem(item.Id)
                    {
                        Name = item.Name,
                        CreatorId = item.CreatorId,
                        CreateDate = item.CreateDate,
                        Description = item.Description,
                        CreatorName = item.CreatorName,
                        ImgUrl = item.ImgUrl
                    });
                    await feedApi.GetCoverImage(item.ImgUrl);
                }
                FeedItems = items.OrderByDescending(i=>i.CreateDate);
                PropertyChanged(this, new PropertyChangedEventArgs("FeedItems"));
            }
            if (FeedItems?.Count() == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), OnTimerForUpdate);
            }
            NoItemsWarningIsVisible = FeedItems?.Count() == 0;
            IsRefreshing = false;
        }

        private bool OnTimerForUpdate()
        {
            _countOfUpdateListByTimer++;
            if ((_countOfUpdateListByTimer > 2) || (FeedItems.Count() > 0))
            {
                _countOfUpdateListByTimer = 0;
            }
            else
            {
                refreshFeedCommandAsync();
            }
            return false;
        }

        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshing"));
                    }
                }
            }
            get
            {
                return _isRefreshing;
            }
        }
        public string FeedItemId
        {
            set
            {
                if (_feedItemId != value)
                {
                    _feedItemId = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("FeedItemId"));
                    }
                }
            }
            get
            {
                return _feedItemId;
            }
        }
        public ViewFeedItem SelectedFeedItem
        {
            set
            {
                if (_feedItem != value)
                {
                    _feedItem = value;

                    /*var routePage = new RoutePage(value.RouteId, false);
                    Navigation.PushAsync(routePage);
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRouteItem"));
                    addNewPointFromShareAsync(_routeItem.Name);*/
                    _feedItem = null;
                }
            }
        }

        public bool NoItemsWarningIsVisible
        {
            set
            {
                if (_noItemsWarningIsVisible != value)
                {
                    _noItemsWarningIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoItemsWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noItemsWarningIsVisible;
            }
        }

        public IEnumerable<ViewFeedItem> FeedItems
        {
            set
            {
                if (_feedItems != value)
                {
                    _feedItems = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("FeedItems"));
                        NoItemsWarningIsVisible = _feedItems.Count() > 0;
                    }
                }
            }
            get
            {
                return _feedItems;
            }
        }
    }
}
