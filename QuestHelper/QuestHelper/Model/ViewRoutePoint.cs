﻿using QuestHelper.LocalDB.Model;
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
        private int _version = 0;
        private DateTimeOffset _createDate;
        private RoutePointManager routePointManager = new RoutePointManager();

        public ViewRoutePoint(string routeId, string routePointId)
        {
            _routeId = routeId;
            RoutePointManager routePointManager = new RoutePointManager();
            if (string.IsNullOrEmpty(routePointId))
            {
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
                _imagePath = routePointManager.GetDefaultImageFilename(_id);
                _imagePreviewPath = routePointManager.GetDefaultImagePreviewFilename(_id);
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
    }
}
