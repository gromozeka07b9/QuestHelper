using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Resources;
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
using Xamarin.Forms.Maps;
using Route = QuestHelper.LocalDB.Model.Route;

namespace QuestHelper.Model
{
    public class ViewPoi : ISaveable
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTimeOffset _createDate;
        private DateTimeOffset _updateDate;
        private string _creatorId = string.Empty;
        private bool _isDeleted = false;
        private bool _isPublished = false;
        private string _imgFilename = string.Empty;
        private string _description = string.Empty;
        private string _byRoutePointId = string.Empty;
        private string _address = string.Empty;
        //private string _routeId;
        private int _likesCount;
        private int _viewsCount;
        private Position _position;

        public ViewPoi(string id)
        {
            _id = id;
            if (!string.IsNullOrEmpty(id))
            {
                Load(id);
            }
            else
            {
                CreateDate = DateTimeOffset.Now;
            }
        }

        public ViewPoi(SharedModelsWS.Poi poi)
        {
            FillFromWSModel(poi);
        }

        public void Load(string id)
        {
            PoiManager manager = new PoiManager();
            LocalDB.Model.Poi poi = manager.GetPoiById(id);
            if (poi != null)
            {
                _id = poi.PoiId;
                _name = poi.Name;
                _createDate = poi.CreateDate;
                _updateDate = poi.UpdateDate;
                _address = poi.Address;
                _isPublished = poi.IsPublished;
                _isDeleted = poi.IsDeleted;
                _creatorId = poi.CreatorId;
                _byRoutePointId = poi.ByRoutePointId;
                _byRoutePointId = poi.ByRoutePointId;
                _description = poi.Description;
                _imgFilename = poi.ImgFilename;
                _likesCount = poi.LikesCount;
                _viewsCount = poi.ViewsCount;
                _position = new Position(poi.Latitude, poi.Longitude);
            }
        }
        internal void FillFromWSModel(SharedModelsWS.Poi poi)
        {
            if (poi != null)
            {
                _id = poi.Id;
                _name = poi.Name;
                _createDate = poi.CreateDate;
                _updateDate = poi.UpdateDate;
                _address = poi.Address;
                _isPublished = poi.IsPublished;
                _isDeleted = poi.IsDeleted;
                _creatorId = poi.CreatorId;
                _byRoutePointId = poi.ByRoutePointId;
                _description = poi.Description;
                _imgFilename = poi.ImgFilename;
                _likesCount = 0;
                _viewsCount = 0;
                _position = new Position(poi.Latitude.GetValueOrDefault(0), poi.Longitude.GetValueOrDefault(0));
            }
        }

        public void Refresh(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                Load(id);
            }
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

        public string ByRoutePointId
        {
            set
            {
                _byRoutePointId = value;
            }
            get
            {
                return _byRoutePointId;
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
        public DateTimeOffset UpdateDate
        {
            get
            {
                return _updateDate;
            }
            set
            {
                _updateDate = value;
            }
        }

        public string ImgFilename
        {
            set
            {
                _imgFilename = value;
            }
            get
            {
                return _imgFilename;
            }
        }
        public string Address
        {
            set
            {
                _address = value;
            }
            get
            {
                return _address;
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
        public Position Location
        {
            set
            {
                _position = value;
            }
            get
            {
                return _position;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
            }
        }

        public bool IsPublished
        {
            get
            {
                return _isPublished;
            }
            set
            {
                _isPublished = value;
            }
        }

        public int LikesCount
        {
            get
            {
                return _likesCount;
            }
            set
            {
                _likesCount = value;
            }
        }
        public int ViewsCount
        {
            get
            {
                return _viewsCount;
            }
            set
            {
                _viewsCount = value;
            }
        }

        public bool Save()
        {
            PoiManager manager = new PoiManager();
            return manager.Save(this);
        }
    }
}
