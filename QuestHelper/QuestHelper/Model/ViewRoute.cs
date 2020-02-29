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
using Route = QuestHelper.LocalDB.Model.Route;

namespace QuestHelper.Model
{
    public class ViewRoute : ISaveable
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTimeOffset _createDate;
        private int _version = 0;
        private bool _isShared = false;
        private bool _isPublished = false;
        private string _creatorId = string.Empty;
        private bool _isDeleted = false;
        private string _imagePreviewPathForList = string.Empty;
        private string _objVerHash = string.Empty;
        private string _imgFilename = string.Empty;
        private string _description = string.Empty;
        private string _routeLengthKmText = string.Empty;
        private string _routePointCountText = string.Empty;
        private string _routeLengthStepsText = string.Empty;
        private string _coverImage = string.Empty;

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
            LocalDB.Model.Route route = manager.GetRouteById(routeId);
            if (route != null)
            {
                _id = route.RouteId;
                _name = route.Name;
                _createDate = route.CreateDate;
                _version = route.Version;
                _isShared = route.IsShared;
                _isPublished = route.IsPublished;
                _isDeleted = route.IsDeleted;
                _creatorId = route.CreatorId;
                _description = route.Description;
                _imgFilename = route.ImgFilename;
                _objVerHash = route.ObjVerHash;
            }
        }
        internal void FillFromWSModel(RouteRoot routeRoot, string routeHash)
        {
            if (routeRoot != null)
            {
                _id = routeRoot.Route.Id;
                _name = routeRoot.Route.Name;
                _createDate = routeRoot.Route.CreateDate;
                _version = routeRoot.Route.Version;
                _isShared = routeRoot.Route.IsShared;
                _isPublished = routeRoot.Route.IsPublished;
                _isDeleted = routeRoot.Route.IsDeleted;
                _creatorId = routeRoot.Route.CreatorId;
                _description = routeRoot.Route.Description;
                _imgFilename = routeRoot.Route.ImgFilename;
                _objVerHash = routeHash;
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

        public string ObjVerHash
        {
            set
            {
                _objVerHash = value;
            }
            get
            {
                return _objVerHash;
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

        public string CoverImage
        {
            get
            {
                string imgCover = string.Empty;
                if (!string.IsNullOrEmpty(_coverImage))
                {
                    imgCover = _coverImage;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_imgFilename))
                    {
                        imgCover = Path.Combine(ImagePathManager.GetPicturesDirectory(), _imgFilename);
                    }

                    if (string.IsNullOrEmpty(imgCover))
                    {
                        RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
                        ViewRoutePointMediaObject vMedia = mediaManager.GetFirstMediaObjectByRouteId(RouteId);
                        if (!string.IsNullOrEmpty(vMedia.Id))
                        {
                            if (!vMedia.IsDeleted)
                            {
                                imgCover = ImagePathManager.GetImagePath(vMedia.RoutePointMediaObjectId, MediaObjectTypeEnum.Image, true);
                            }
                        }
                        else imgCover = "mount1.png";
                    }
                    _coverImage = imgCover;
                }
                return imgCover;
            }
        }

        public string RouteLengthKm
        {
            get
            {
                if(string.IsNullOrEmpty(_routeLengthKmText))
                {
                    fillRouteLengthData();
                }
                return _routeLengthKmText;
            }

        }

        private void fillRouteLengthData()
        {
            RouteManager _routeManager = new RouteManager();
            (int pointCount, double routeLength) = _routeManager.GetLengthRouteData(RouteId);
            _routeLengthKmText = $"{routeLength.ToString("F1")} Km";
            _routePointCountText = CommonResource.CommonMsg_PointCount.Replace("[pointCount]", pointCount.ToString("N0"));
            _routeLengthStepsText = CommonResource.CommonMsg_StepsCount.Replace("[stepCount]", (routeLength * 1000 * 1.3).ToString("N0"));

        }

        public string RoutePointsCount
        {
            get
            {
                if (string.IsNullOrEmpty(_routePointCountText))
                {
                    fillRouteLengthData();
                }
                return _routePointCountText;
            }

        }
        /*public string RoutePhotosCount
        {
            get
            {
                RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
                return $"Фото: {_routePointMediaObjectManager.GetCountByRouteId(RouteId).ToString("N0")}";
            }

        }*/
        /*public string RouteLengthSummary
        {
            get
            {
                return $"Длина маршрута {RouteLengthKm}, {RouteLengthSteps}";
            }

        }*/

        /*public string RoutePointAndPhotosCount
        {
            get
            {
                RoutePointManager _routePointManager = new RoutePointManager();
                RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
                //return $"Точек: {_routePointManager.GetPointsByRouteId(RouteId).Count().ToString("N0")}, фото: {_routePointMediaObjectManager.GetCountByRouteId(RouteId).ToString("N0")}";
                return $"Точек: много";
            }

        }*/
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
                if (string.IsNullOrEmpty(_routeLengthStepsText))
                {
                    fillRouteLengthData();
                }
                return _routeLengthStepsText;
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

        public bool IsHaveAnyPhotos
        {
            get
            {
                RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
                ViewRoutePointMediaObject vMedia = mediaManager.GetFirstMediaObjectByRouteId(RouteId);
                return (!string.IsNullOrEmpty(vMedia.FileName) && !vMedia.IsDeleted);
            }
        }

        public bool Save()
        {
            RouteManager routeManager = new RouteManager();
            return routeManager.Save(this);
        }
    }
}
