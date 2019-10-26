using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.SharedModelsWS;
using QuestHelper.WS;
using Realms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Route = QuestHelper.LocalDB.Model.Route;

namespace QuestHelper.Model
{
    public class ViewFeedItem
    {
        private RoutePointManager _routePointManager = new RoutePointManager();
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTimeOffset _createDate;
        private string _creatorId = string.Empty;
        private string _creatorName = string.Empty;
        private string _imgUrl = string.Empty;
        //private string _imagePreviewPathForList = string.Empty;
        private string _imgFilename = string.Empty;
        private string _description = string.Empty;
        private FeedItemType _feedType = FeedItemType.Unknown;

        /// <summary>
        /// Создание элемента ленты, пока поддерживается только тип Route
        /// </summary>
        /// <param name="feedItemId"></param>
        /// <param name="feedItemType"></param>
        public ViewFeedItem(string feedItemId, int feedItemType = 1)
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
                else
                {
                    imgCover = "mount1.png";
                }
                return imgCover;
            }
        }
    }
}
