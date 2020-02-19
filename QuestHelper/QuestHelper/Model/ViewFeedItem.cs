using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.SharedModelsWS;
using QuestHelper.WS;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Route = QuestHelper.LocalDB.Model.Route;

namespace QuestHelper.Model
{
    public class ViewFeedItem : INotifyPropertyChanged
    {
        private RoutePointManager _routePointManager = new RoutePointManager();
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTimeOffset _createDate;
        private string _creatorId = string.Empty;
        private string _creatorName = string.Empty;
        private string _imgUrl = string.Empty;
        private string _imgFilename = string.Empty;
        private string _description = string.Empty;
        private string _favoriteImage = string.Empty;
        private int _viewsCount = 0;
        private bool _isUserLiked;
        private bool _isUserViewed;
        private int _favoritesCount = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Создание элемента ленты, пока поддерживается только тип Route
        /// </summary>
        /// <param name="feedItemId"></param>
        public ViewFeedItem(string feedItemId)
        {
            if (string.IsNullOrEmpty(feedItemId))
            {
                throw new Exception("Can't create new feed element");
            }
            _id = feedItemId;
        }
        public ViewFeedItem()
        {
        }

        public string Id
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public string Name
        {
            set
            {
                _name = value;
            }
            get
            {
                return _name;
            }
        }
        public string CreatorId
        {
            set
            {
                _creatorId = value;
            }
            get
            {
                return _creatorId;
            }
        }

        public string CreatorName
        {
            set
            {
                _creatorName = value;
            }
            get
            {
                return _creatorName;
            }
        }

        public string CreateDateText
        {
            get
            {
                return _createDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTimeOffset CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
            }
        }

        public string Description
        {
            set
            {
                _description = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    var coupleOfPoints = _routePointManager.GetFirstAndLastPoints(_id);
                    if (coupleOfPoints.Item1 != null)
                    {
                        _description = coupleOfPoints.Item1.Description;
                    }

                    if (string.IsNullOrEmpty(_description))
                    {
                        var points = _routePointManager.GetPointsByRouteId(_id);
                        StringBuilder sb = new StringBuilder();
                        foreach (var point in points)
                        {
                            sb.Append($"{point.Name}-");
                        }

                        string fullString = sb.ToString();
                        if (!string.IsNullOrEmpty(fullString))
                        {
                            _description = fullString.Substring(0, fullString.Length - 2);
                        }
                    }
                }
                return _description;
            }
        }

        public string ImgUrl
        {
            set
            {
                _imgUrl = value;
                _imgFilename = System.IO.Path.GetFileName(_imgUrl);
            }
            get
            {
                return _imgUrl;
            }
        }

        public string CoverImage
        {
            get
            {
                string imgCover = string.Empty;
                if (!string.IsNullOrEmpty(_imgFilename))
                {
                    string imgPath = Path.Combine(ImagePathManager.GetPicturesDirectory(), _imgFilename);
                    if (File.Exists(imgPath))
                    {
                        imgCover = imgPath;
                    }
                }
                return imgCover;
            }
        }

        public string FavoriteImage
        {
            get
            {
                return _isUserLiked ? "ic_like_on_1" : "ic_like_off_1";
            }
        }
        public int FavoritesCount
        {
            set
            {
                if (_favoritesCount != value)
                {
                    _favoritesCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FavoritesCount"));
                }
            }
            get
            {
                return _favoritesCount;
            }
        }
        public int ViewsCount
        {
            set
            {
                if(_viewsCount != value)
                {
                    _viewsCount = value;
                }
            }
            get
            {
                return _viewsCount;
            }
        }

        public bool IsUserLiked
        {
            set
            {
                _isUserLiked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FavoriteImage"));
            }
            get
            {
                return _isUserLiked;
            }
        }

        public bool IsUserViewed
        {
            set
            {
                _isUserViewed = value;
            }
            get
            {
                return _isUserViewed;
            }
        }
    }
}
