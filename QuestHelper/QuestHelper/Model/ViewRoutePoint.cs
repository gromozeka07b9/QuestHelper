using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model
{
    public class ViewRoutePoint
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
        private int _version = 0;
        private List<RoutePointMediaObject> _mediaObjects = new List<RoutePointMediaObject>();
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
        }

        public ViewRoutePoint()
        {

        }
        private void load(string routePointId)
        {
            RoutePoint point = routePointManager.GetPointById(routePointId);
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
                _mediaObjects = mediaManager.GetMediaObjectsByRoutePointId(_id)?.ToList();
                if ((_mediaObjects != null) && (_mediaObjects.Count > 0))
                {
                    _imagePath = ImagePathManager.GetImagePath(_mediaObjects[0].RoutePointMediaObjectId);
                    _imagePreviewPath = ImagePathManager.GetImagePath(_mediaObjects[0].RoutePointMediaObjectId, true);
                    //_imagePath = Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "pictures", $"img_{mediaCollection[0].RoutePointMediaObjectId}.jpg");
                    //_imagePreviewPath = Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "pictures", $"img_{mediaCollection[0].RoutePointMediaObjectId}_preview.jpg");
                }
                else
                {
                    _imagePath = routePointManager.GetDefaultImageFilename(_id);
                    _imagePreviewPath = routePointManager.GetDefaultImagePreviewFilename(_id);
                }
            }
        }
        internal void Refresh(string routePointId)
        {
            if(!string.IsNullOrEmpty(routePointId))
                load(routePointId);
        }

        public List<RoutePointMediaObject> MediaObjects
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
                
                if (!string.IsNullOrEmpty(_imagePreviewPath)&& File.Exists(_imagePreviewPath))
                    return _imagePreviewPath;
                else
                    return _imagePath;
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

        internal bool Save()
        {
            _id = routePointManager.Save(this);
            return !string.IsNullOrEmpty(_id);
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
    }
}
