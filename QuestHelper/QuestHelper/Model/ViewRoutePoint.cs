using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.SharedModelsWS;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using RoutePoint = QuestHelper.SharedModelsWS.RoutePoint;

namespace QuestHelper.Model
{
    public class ViewRoutePoint : ISaveable
    {
        private string _id = string.Empty;
        private string _routeId = string.Empty;
        private string _name = string.Empty;
        private string _imagePath = string.Empty;
        private string _imagePreviewPath = string.Empty;
        private string _description = string.Empty;
        private string _address = string.Empty;
        private double _latitude = 0;
        private double _longtitude = 0;
        private bool _isDeleted = false;
        private int _version = 0;
        private List<LocalDB.Model.RoutePointMediaObject> _mediaObjects = new List<LocalDB.Model.RoutePointMediaObject>();
        private DateTimeOffset _createDate;
        private RoutePointManager routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();

        public ViewRoutePoint(string routeId, string routePointId)
        {
            _routeId = routeId;
            if (!string.IsNullOrEmpty(routePointId))
            {
                load(routePointId);
            }
            else
            {
                _createDate = DateTimeOffset.Now;
                _imagePreviewPath = "emptyimg.png";
                _imagePath = "emptyimg.png";
            }
        }

        public ViewRoutePoint()
        {

        }
        private void load(string routePointId)
        {
            LocalDB.Model.RoutePoint point = routePointManager.GetPointById(routePointId);
            if (point != null)
            {
                _id = routePointId;
                _name = point.Name;
                _description = point.Description;
                _address = point.Address;
                _latitude = point.Latitude;
                _longtitude = point.Longitude;
                _createDate = point.CreateDate;
                _version = point.Version;
                _isDeleted = point.IsDeleted;
                refreshMediaObjects();
            }
        }
        internal void FillFromWSModel(RoutePoint routePoint)
        {
            if (routePoint != null)
            {
                _id = routePoint.Id;
                _name = routePoint.Name;
                _description = routePoint.Description;
                _address = routePoint.Address;
                _latitude = routePoint.Latitude.HasValue ? (double)routePoint.Latitude : 0;
                _longtitude = routePoint.Longitude.HasValue ? (double)routePoint.Longitude : 0;
                _createDate = routePoint.CreateDate;
                _version = routePoint.Version;
                _isDeleted = routePoint.IsDeleted;
                refreshMediaObjects();
            }
        }

        private void refreshMediaObjects()
        {
            /*_mediaObjects = mediaManager.GetMediaObjectsByRoutePointId(_id)?.ToList();
            if ((_mediaObjects != null) && (_mediaObjects.Count > 0))
            {
                _imagePath = ImagePathManager.GetImagePath(_mediaObjects[0].RoutePointMediaObjectId);
                _imagePreviewPath = ImagePathManager.GetImagePath(_mediaObjects[0].RoutePointMediaObjectId, true);
            }
            else
            {
                _imagePath = routePointManager.GetDefaultImageFilename(_id);
                _imagePreviewPath = routePointManager.GetDefaultImagePreviewFilename(_id);
            }*/
        }

        internal void Refresh(string routePointId)
        {
            if(!string.IsNullOrEmpty(routePointId))
                load(routePointId);
        }

        public List<LocalDB.Model.RoutePointMediaObject> MediaObjects
        {
            get
            {
                return _mediaObjects;
            }
        }
        public string Id
        {
            get
            {
                return _id;
            }
        }
        public string RoutePointId
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
        public string RouteId
        {
            set
            {
                _routeId = value;
            }
            get
            {
                return _routeId;
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
        public string CreateDateText
        {
            get
            {
                return _createDate.ToString("D");
            }
        }

        public string NameText
        {
            get
            {
                return _name.ToUpper();
            }
        }
        public DateTimeOffset CreateDate
        {
            set
            {
                _createDate = value;
            }
            get
            {
                return _createDate;
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

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
        }
        public string ImagePreviewPath
        {
            get
            {
                return _imagePreviewPath;

                /*if (!string.IsNullOrEmpty(_imagePreviewPath)&& File.Exists(_imagePreviewPath))
                    return _imagePreviewPath;
                else
                    return _imagePath;*/
            }
        }
        public string ImagePreviewPathForList
        {
            get
            {
                if (!string.IsNullOrEmpty(_imagePreviewPath) && File.Exists(_imagePreviewPath))
                    return _imagePreviewPath;
                else
                    return "camera1.png";
            }
        }        

        public double Latitude
        {
            set
            {
                _latitude = value;
            }
            get
            {
                return _latitude;
            }
        }
        public double Longitude
        {
            set
            {
                _longtitude = value;
            }
            get
            {
                return _longtitude;
            }
        }
        public int Version
        {
            set
            {
                _version = value;
            }
            get
            {
                return _version;
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

        internal bool Delete()
        {
            bool result = routePointManager.Delete(this);
            if(result)
            {
                _id = string.Empty;
                _routeId = string.Empty;
                _name = string.Empty;
                _imagePath = string.Empty;
                _imagePreviewPath = string.Empty;
            }
            return result;
        }

        internal void AddImage(string mediaId)
        {
            ViewRoutePointMediaObject media = new ViewRoutePointMediaObject();
            media.RoutePointMediaObjectId = mediaId;
            media.RoutePointId = Id;
            media.Version = 1;
            mediaManager.Save(media);
            load(Id);
        }
        internal void DeleteImage(string mediaId)
        {
            if (mediaManager.Delete(mediaId))
            {
                string imagePath = ImagePathManager.GetImagePath(mediaId);
                string imagePreviewPath = ImagePathManager.GetImagePath(mediaId, true);
                try
                {
                    File.Delete(imagePath);
                }
                catch (Exception)
                {
                }
                try
                {
                    File.Delete(imagePreviewPath);
                }
                catch (Exception)
                {
                }
                refreshMediaObjects();
            }
        }

        public bool Save()
        {
            _id = routePointManager.Save(this);
            return !string.IsNullOrEmpty(_id);
        }
    }
}
