using Autofac;
using Microsoft.Extensions.Caching.Memory;
using QuestHelper.Model;
using QuestHelper.SharedModelsWS;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using QuestHelper.Managers;
using Xamarin.Forms;
using QuestHelper.View;

namespace QuestHelper.ViewModel
{
    class FeedRoutesViewModel : INotifyPropertyChanged
    {
        private const string _feedCacheId = "FeedApiCache";
        private IMemoryCache _memoryCache;
        private string _feedItemId;
        private IEnumerable<ViewFeedItem> _feedItems = new List<ViewFeedItem>();
        private ViewFeedItem _feedItem = new ViewFeedItem();
        private int _countOfUpdateListByTimer = 0;
        private bool _noItemsWarningIsVisible = false;
        private bool _isRefreshing = false;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RefreshFeedCommand { get; private set; }

        public FeedRoutesViewModel()
        {
            RefreshFeedCommand = new Command(refreshFeedCommandAsync);
            _memoryCache = App.Container.Resolve<IMemoryCache>();
        }

        public void startDialog()
        {
            var toolbarService = DependencyService.Get<IToolbarService>();
            toolbarService.SetVisibilityToolbar(true);
            refreshFeedCommandAsync();
        }

        internal void closeDialog()
        {
        }

        async void refreshFeedCommandAsync()
        {
            IsRefreshing = true;
            List<FeedItem> feed = new List<FeedItem>();
            if (!_memoryCache.TryGetValue(_feedCacheId, out feed))
            {
                feed = await getFeedFromApi();
            }
            FeedItems = getSortedViewFeed(feed);

            PropertyChanged(this, new PropertyChangedEventArgs("FeedItems"));
            if (FeedItems?.Count() == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), OnTimerForUpdate);
            }
            NoItemsWarningIsVisible = FeedItems?.Count() == 0;
            IsRefreshing = false;
        }

        private IEnumerable<ViewFeedItem> getSortedViewFeed(List<FeedItem> feed)
        {
            var items = new List<ViewFeedItem>();
            foreach (var item in feed)
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
            }

            //FeedItems = items.OrderByDescending(i => i.CreateDate);
            return items.OrderByDescending(i => i.CreateDate);
        }

        private async Task<List<FeedItem>> getFeedFromApi()
        {
            List<FeedItem> feed;
            TokenStoreService tokenService = new TokenStoreService();
            FeedApiRequest feedApi = new FeedApiRequest(await tokenService.GetAuthTokenAsync());
            feed = await feedApi.GetFeed();
            if (feedApi.LastHttpStatusCode == HttpStatusCode.OK)
            {
                _memoryCache.Set(_feedCacheId, feed, new MemoryCacheEntryOptions()
                {
#if DEBUG
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
#else
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
#endif
                });
                foreach (var item in feed)
                {
                    await feedApi.GetCoverImage(item.ImgUrl);
                }
            }

            return feed;
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
                    var coverPage = new RouteCoverPage(value);
                    Navigation.PushAsync(coverPage);
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRouteItem"));
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
