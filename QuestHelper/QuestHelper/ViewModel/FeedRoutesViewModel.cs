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
using QuestHelper.Model.Messages;
using Microsoft.AppCenter.Analytics;

namespace QuestHelper.ViewModel
{
    class FeedRoutesViewModel : INotifyPropertyChanged
    {
        private const string _feedCacheId = "FeedApiCache";
        private IMemoryCache _memoryCache;
        private string _feedItemId;
        private string _textFilter;
        private IEnumerable<ViewFeedItem> _feedItems = new List<ViewFeedItem>();
        private ViewFeedItem _feedItem = new ViewFeedItem();
        private int _countOfUpdateListByTimer = 0;
        private bool _noItemsWarningIsVisible = false;
        private bool _isRefreshing = false;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RefreshFeedCommand { get; private set; }
        public ICommand SetLikeCommand { get; private set; }
        public ICommand SearchRoutesCommand { get; private set; }
        

        public FeedRoutesViewModel()
        {
            RefreshFeedCommand = new Command(refreshFeedCommandAsync);
            SetLikeCommand = new Command(setLikeCommand);
            SearchRoutesCommand = new Command(searchRoutesCommand);
            _memoryCache = App.Container.Resolve<IMemoryCache>();
        }

        private async void searchRoutesCommand(object text)
        {
            await refreshFeed(false, TextFilter);
        }

        private void setLikeCommand(object routeObject)
        {
            var item = routeObject as ViewFeedItem;
            item.IsUserLiked = !item.IsUserLiked;
            item.FavoritesCount = item.IsUserLiked ? ++item.FavoritesCount : --item.FavoritesCount;
            MessagingCenter.Send(new SetEmotionRouteMessage() { RouteId = item.Id, Emotion = item.IsUserLiked }, string.Empty);
            List<FeedItem> feed = new List<FeedItem>();
            if (_memoryCache.TryGetValue(_feedCacheId, out feed))
            {
                var itemInCache = feed.Where(f => f.Id.Equals(item.Id)).FirstOrDefault();
                if (itemInCache != null)
                {
                    itemInCache.IsUserLiked = item.IsUserLiked ? 1 : 0;
                    itemInCache.LikeCount = item.FavoritesCount;
                }
                addFeedToCache(feed);
            }
            Analytics.TrackEvent($"Set route emotion: {item.IsUserLiked}");
        }

        private void addFeedToCache(List<FeedItem> feed)
        {
            _memoryCache.Set(_feedCacheId, feed, new MemoryCacheEntryOptions()
            {
#if DEBUG
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
#else
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(600)
#endif
            });
        }

        public async void startDialogAsync()
        {
            Analytics.TrackEvent("Feed started", new Dictionary<string, string> { });
            await refreshFeed(false, TextFilter);
        }

        internal void closeDialog()
        {
        }

        async void refreshFeedCommandAsync()
        {
            await refreshFeed(true, TextFilter);
        }

        private async Task refreshFeed(bool force, string textFilter)
        {
            IsRefreshing = true;
            List<FeedItem> feed = new List<FeedItem>();
            if (!force)
            {
                if (!_memoryCache.TryGetValue(_feedCacheId, out feed))
                {
                    feed = await getFeedFromApi();
                }
            }
            else
            {
                feed = await getFeedFromApi();
            }

            if(feed != null)
            {
                FeedItems = getSortedViewFeed(feed, textFilter);
            }
            else
            {
                HandleError.Process("FeedRoutesViewModel", "GetFeed", new Exception("feed is null"), false);
            }

            PropertyChanged(this, new PropertyChangedEventArgs("FeedItems"));
            if (FeedItems?.Count() == 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(3), OnTimerForUpdate);
            }

            NoItemsWarningIsVisible = FeedItems?.Count() == 0;
            IsRefreshing = false;
        }

        private IEnumerable<ViewFeedItem> getSortedViewFeed(List<FeedItem> feed, string textFilter)
        {
            var items = new List<ViewFeedItem>();
            string filterString = textFilter is null ? string.Empty: textFilter.Trim();
            foreach (var item in feed)
            {
                bool itemFoundInFilter = true;
                if (!string.IsNullOrEmpty(filterString))
                {
                    itemFoundInFilter = (item.Name.IndexOf(filterString, StringComparison.CurrentCultureIgnoreCase) > -1) ||
                                        (item.Description.IndexOf(filterString, StringComparison.CurrentCultureIgnoreCase) > -1) ||
                                        (item.CreatorName.IndexOf(filterString, StringComparison.CurrentCultureIgnoreCase) > -1);
                }
                if (itemFoundInFilter)
                {
                    items.Add(new ViewFeedItem(item.Id)
                    {
                        Name = item.Name,
                        CreatorId = item.CreatorId,
                        CreateDate = item.CreateDate,
                        Description = item.Description,
                        CreatorName = item.CreatorName,
                        ImgUrl = item.ImgUrl,
                        FavoritesCount = item.LikeCount,
                        ViewsCount = item.ViewCount,
                        IsUserViewed = item.IsUserViewed > 0,
                        IsUserLiked = item.IsUserLiked > 0
                    });
                }
            }

            //FeedItems = items.OrderByDescending(i => i.CreateDate);
            return items.OrderByDescending(i => i.CreateDate);
        }

        private async Task<List<FeedItem>> getFeedFromApi()
        {
            List<FeedItem> feed;
            TokenStoreService tokenService = new TokenStoreService();
            string token = await tokenService.GetAuthTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                GuestAuthHelper guestHelper = new GuestAuthHelper();
                token = await guestHelper.TryGetGuestTokenAsync();
                if(!string.IsNullOrEmpty(token))
                {
                    ParameterManager par = new ParameterManager();
                    par.Set("GuestMode", "1");
                }
                else
                {
                    return new List<FeedItem>();
                }
            }

            FeedApiRequest feedApi = new FeedApiRequest(token);
            feed = await feedApi.GetFeed();
            if (feedApi.LastHttpStatusCode == HttpStatusCode.OK)
            {
                addFeedToCache(feed);
                foreach (var item in feed)
                {
                    await feedApi.GetCoverImage(item.ImgUrl);
                }
            }
            bool AuthRequired = (feedApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || feedApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if (AuthRequired)
            {
                /*var pageCollections = new PagesCollection();
                MainPageMenuItem destinationPage = pageCollections.GetLoginPage();
                Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);*/
                GuestAuthHelper guestHelper = new GuestAuthHelper();
                guestHelper.ResetCurrentGuestToken();
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
                //return _isRefreshing;
                return false;
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
        public string TextFilter
        {
            set
            {
                if ((_textFilter != value) && (string.IsNullOrEmpty(value)))
                {
                    Task.Run(() => refreshFeed(false, value));
                }
                if (_textFilter != value)
                {
                    _textFilter = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("TextFilter"));
                    }
                }
            }
            get
            {
                return _textFilter;
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
                    Navigation.PushModalAsync(coverPage);
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRouteItem"));
                    if (!_feedItem.IsUserViewed)
                    {
                        Xamarin.Forms.MessagingCenter.Send<AddRouteViewedMessage>(new AddRouteViewedMessage() { RouteId = _feedItem.Id }, string.Empty);
                        Analytics.TrackEvent($"Set route viewed");
                    }
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
