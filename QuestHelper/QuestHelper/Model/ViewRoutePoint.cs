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
        //private RoutePoint _dBRoutePoint;
        //private ImageSource _imagePreview;
        private string _id = string.Empty;
        private string _routeId = string.Empty;
        private string _name = string.Empty;
        private string _imagePath = string.Empty;
        private string _imagePreviewPath = string.Empty;
        private string _description = string.Empty;
        private string _address = string.Empty;
        private double _latitude = 0;
        private double _longtitude = 0;
        private DateTime _createDate;

        public ViewRoutePoint(string routeId, string routePointId)
        {
            RoutePointManager manager = new RoutePointManager();
            RoutePoint point = manager.GetPointById(routePointId);
            _id = routePointId;
            _routeId = routeId;
            _name = point.Name;
            _description = point.Description;
            _address = point.Address;
            _latitude = point.Latitude;
            _longtitude = point.Longitude;

            //_imagePath = point.im
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }
        public string Name
        {
            set
            {
                /*var realm = RoutePointManager.GetRealmInstance();
                realm.Write(() =>
                {
                    _dBRoutePoint.Name = value;
                    _dBRoutePoint.UpdateDate = DateTime.Now;
                });*/
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
            get
            {
                return _description;
            }
        }
        /*public ImageSource ImagePreview
        {
            get
            {
                if (_dBRoutePoint.MediaObjects.Count > 0)
                {
                    return ImageSource.FromFile(_dBRoutePoint.MediaObjects[0].FileNamePreview);
                } else
                {
                    return ImageSource.FromFile("earth21.png");
                }
            }
        }*/

        public string ImagePath
        {
            set
            {
                _imagePath = value;
            }
            get
            {
                RoutePointManager manager = new RoutePointManager();
                return manager.GetDefaultImageFilename(_id);
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
                RoutePointManager manager = new RoutePointManager();
                return manager.GetDefaultImagePreviewFilename(_id);
            }
        }
        public double Latitude
        {
            set
            {
                /*var realm = RoutePointManager.GetRealmInstance();
                realm.Write(() =>
                {
                    _dBRoutePoint.Latitude = value;
                    _dBRoutePoint.UpdateDate = DateTime.Now;
                });*/
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
                /*var realm = RoutePointManager.GetRealmInstance();
                realm.Write(() =>
                {
                    _dBRoutePoint.Longitude = value;
                    _dBRoutePoint.UpdateDate = DateTime.Now;
                });*/
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
                /*var realm = RoutePointManager.GetRealmInstance();
                realm.Write(() =>
                {
                    _dBRoutePoint.Address = value;
                    _dBRoutePoint.UpdateDate = DateTime.Now;
                });*/
                _address = value;
            }
            get
            {
                return _address;
            }
        }
        /*internal void AddMedia(string imageFilePath, string imagePreviewFilePath)
        {
            RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
            manager.Add(_dBRoutePoint, imagePreviewFilePath, imageFilePath);
        }*/

        internal bool Save()
        {
            /*_point.MainRoute = _route;
            RoutePointManager manager = new RoutePointManager();
            manager.Add()*/
            RouteManager routeManager = new RouteManager();
            RoutePointManager routePointManager = new RoutePointManager();
            RoutePoint point = routePointManager.GetPointById(_id);
            if(point == null)
            {
                point = new RoutePoint();

            } else
            {
                point.UpdateDate = DateTimeOffset.Now;
            }
            point.RoutePointId = _id;
            point.RouteId = _routeId;
            point.MainRoute = routeManager.GetRouteById(_routeId);
            point.Address = _address;
            point.Description = _description;
            point.Latitude = _latitude;
            point.Longitude = _longtitude;
            point.Name = _name;
            return routePointManager.Add(point);
        }
    }
}
