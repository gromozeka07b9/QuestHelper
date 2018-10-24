using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
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
        private DateTimeOffset _createDate;

        public ViewRoutePoint(string routeId, string routePointId)
        {
            _routeId = routeId;
            if(string.IsNullOrEmpty(routePointId))
            {
                //RoutePointManager manager = new RoutePointManager();
                //_imagePreviewPath = manager.GetEmptyImageFilename();
            }
            else
            {
                load(routePointId);
            }
        }

        public ViewRoutePoint()
        {

        }
        private void load(string routePointId)
        {
            RoutePointManager manager = new RoutePointManager();
            RoutePoint point = manager.GetPointById(routePointId);
            if (point != null)
            {
                _id = routePointId;
                _name = point.Name;
                _description = point.Description;
                _address = point.Address;
                _latitude = point.Latitude;
                _longtitude = point.Longitude;
                _createDate = point.CreateDate;
                _imagePath = manager.GetDefaultImageFilename(_id);
                _imagePreviewPath = manager.GetDefaultImagePreviewFilename(_id);
            }
        }
        internal void Refresh(string routePointId)
        {
            load(routePointId);
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }
        public string RouteId
        {
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
                return _createDate.ToLocalTime().ToString();
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
            set
            {
                _imagePath = value;
            }
            get
            {
                return _imagePath;
            }
        }
        public string ImagePreviewPath
        {
            set
            {
                _imagePreviewPath = value;
            }
            get
            {
                return _imagePreviewPath;
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
            RoutePointManager routePointManager = new RoutePointManager();
            _id = routePointManager.Save(this);
            return !string.IsNullOrEmpty(_id);
        }
    }
}
