using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using Realms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model
{
    public class ViewRoute : ISaveable
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTimeOffset _createDate;
        private int _version = 0;
        private bool _isShared = false;

        public ViewRoute(string routeId)
        {
            _id = routeId;
            if (!string.IsNullOrEmpty(routeId))
            {
                Load(routeId);
            }
            else
            {
                CreateDate = DateTimeOffset.Now;
                Version = 1;                
            }
        }
        public ViewRoute()
        {
        }

        public void Load(string routeId)
        {
            RouteManager manager = new RouteManager();
            Route route = manager.GetRouteById(routeId);
            if (route != null)
            {
                _id = route.RouteId;
                _name = route.Name;
                _createDate = route.CreateDate;
                _version = route.Version;
                _isShared = route.IsShared;
            }
        }

        public void Refresh(string routeId)
        {
            if(!string.IsNullOrEmpty(routeId))
            {
                Load(routeId);
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
        public string RouteId
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

        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }
        public string ImagePreviewPathForList
        {
            get
            {
                RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
                ViewRoutePointMediaObject vMedia = mediaManager.GetFirstMediaObjectByRouteId(RouteId);
                if (!string.IsNullOrEmpty(vMedia.Id))
                {
                    if (!vMedia.IsDeleted)
                    {
                        string path = ImagePathManager.GetImagePath(vMedia.RoutePointMediaObjectId, true);
                        if (File.Exists(path))
                        {
                            return ImagePathManager.GetImagePath(vMedia.RoutePointMediaObjectId, true);
                        }
                    }
                }
                return "mount1.png";
            }
        }
        public string RouteLengthKm
        {
            get
            {
                RouteManager _routeManager = new RouteManager();
                double routeLength = _routeManager.GetLength(RouteId);
                return $"{routeLength.ToString("F1")} км";
            }

        }
        public string RoutePointsCount
        {
            get
            {
                RoutePointManager _routePointManager = new RoutePointManager();
                return $"Точек: {_routePointManager.GetPointsByRouteId(RouteId).Count().ToString("N0")}";
            }

        }
        public string RoutePhotosCount
        {
            get
            {
                RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
                return $"Фото: {_routePointMediaObjectManager.GetCountByRouteId(RouteId).ToString("N0")}";
            }

        }
        public string RoutePointAndPhotosCount
        {
            get
            {
                RoutePointManager _routePointManager = new RoutePointManager();
                RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
                return $"Точек: {_routePointManager.GetPointsByRouteId(RouteId).Count().ToString("N0")}, фото: {_routePointMediaObjectManager.GetCountByRouteId(RouteId).ToString("N0")}";
            }

        }
        public string RouteDays
        {
            get
            {
                RoutePointManager _routePointManager = new RoutePointManager();
                var tuplePoints = _routePointManager.GetFirstAndLastPoints(RouteId);
                if (tuplePoints.Item1.CreateDate.Day == tuplePoints.Item2.CreateDate.Day)
                {
                    return $"{tuplePoints.Item1.CreateDate.Year.ToString()}, {tuplePoints.Item1.CreateDate.ToString("MMMM", CultureInfo.InvariantCulture)} {tuplePoints.Item1.CreateDate.Day}";
                }
                else
                {
                    return $"{tuplePoints.Item1.CreateDate.Year.ToString()}, {tuplePoints.Item1.CreateDate.ToString("MMMM", CultureInfo.InvariantCulture)} {tuplePoints.Item1.CreateDate.Day} - {tuplePoints.Item2.CreateDate.ToString("MMMM", CultureInfo.InvariantCulture)} {tuplePoints.Item2.CreateDate.Day}";
                }
            }

        }
        public string RouteLengthSteps
        {
            get
            {
                RouteManager _routeManager = new RouteManager();
                double routeLength = _routeManager.GetLength(RouteId);
                return $"{(routeLength * 1000 * 1.3).ToString("N0")} шагов";
            }

        }

        public string UserId { get; set; }

        public bool IsShared
        {
            get
            {
                return _isShared;
            }
        }

        public bool Save()
        {
            RouteManager routeManager = new RouteManager();
            return routeManager.Save(this);
        }
    }
}
