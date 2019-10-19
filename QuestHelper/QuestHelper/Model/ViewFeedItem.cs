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

        /*public void Load(string routeId)
        {
            RouteManager manager = new RouteManager();
            LocalDB.Model.Route route = manager.GetRouteById(routeId);
            if (route != null)
            {
                _id = route.RouteId;
                _name = route.Name;
                _createDate = route.CreateDate;
                _version = route.Version;
                _creatorId = route.CreatorId;
                _description = route.Description;
                _objVerHash = route.ObjVerHash;
            }
        }*/
        /*internal void FillFromWSModel(RouteRoot routeRoot, string routeHash)
        {
            if (routeRoot != null)
            {
                _id = routeRoot.Route.Id;
                _name = routeRoot.Route.Name;
                _createDate = routeRoot.Route.CreateDate;
                _version = routeRoot.Route.Version;
                _isDeleted = routeRoot.Route.IsDeleted;
                _creatorId = routeRoot.Route.CreatorId;
                _description = routeRoot.Route.Description;
                _objVerHash = routeHash;
            }
        }*/

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
                return _createDate.ToLocalTime().ToString();
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
