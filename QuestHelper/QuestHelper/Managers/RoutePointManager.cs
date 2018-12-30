using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;

namespace QuestHelper.Managers
{
    public class RoutePointManager
    {
        Realm _realmInstance;
        public RoutePointManager()
        {
            _realmInstance = RealmAppInstance.GetAppInstance();
            _realmInstance.Error += _realmInstance_Error;
        }

        private void _realmInstance_Error(object sender, ErrorEventArgs e)
        {
        }

        internal static Realm GetRealmInstance()
        {
            return RealmAppInstance.GetAppInstance();
        }
        internal RoutePoint GetPointById(string id)
        {
            return !string.IsNullOrEmpty(id)?_realmInstance.Find<RoutePoint>(id):null;
        }

        internal IEnumerable<ViewRoutePoint> GetPointsByRouteId(string routeId)
        {
            List<ViewRoutePoint> collection = new List<ViewRoutePoint>();
            try
            {
                var collectionRealm = _realmInstance.All<RoutePoint>().Where(point => point.RouteId == routeId).OrderByDescending(point=>point.CreateDate);
                foreach(var item in collectionRealm)
                {
                    collection.Add(new ViewRoutePoint(item.RouteId, item.RoutePointId));
                }
            }
            catch (Exception e)
            {
                collection = new List<ViewRoutePoint>();
                HandleError.Process("RoutePointManager", "GetPointsByRouteId", e, false);
            }
            return collection;
        }
        internal RoutePoint GetPointByCoordinates(double latitude, double longitude)
        {
            var collection = _realmInstance.All<RoutePoint>().Where(point => point.Latitude == latitude && point.Longitude == longitude);
            return collection.FirstOrDefault();
        }

        internal string Save(ViewRoutePoint vpoint)
        {
            string returnid = string.Empty;
            RouteManager routeManager = new RouteManager();
            try
            {
                _realmInstance.Write(() =>
                {
                    RoutePoint point = !string.IsNullOrEmpty(vpoint.Id) ? _realmInstance.Find<RoutePoint>(vpoint.Id) : null;
                    if(point == null)
                    {
                        point = new RoutePoint();
                        point.RoutePointId = !string.IsNullOrEmpty(vpoint.Id) ? vpoint.Id: point.RoutePointId;
                        point.RouteId = vpoint.RouteId;
                        point.MainRoute = routeManager.GetRouteById(vpoint.RouteId);
                        if (point.MainRoute != null)
                        {
                            point.MainRoute.Points.Add(point);//?
                        }
                        else
                        {
                            HandleError.Process("RoutePointManager", "SavePoint", new Exception($"routeId:{vpoint.RouteId}, pointId:{vpoint.Id}"), false);
                        }
                        _realmInstance.Add(point);
                    }
                    else
                    {
                        //point = _realmInstance.Find<RoutePoint>(vpoint.Id);
                    }
                    returnid = point.RoutePointId;
                    point.Address = vpoint.Address;
                    point.Description = vpoint.Description;
                    point.Latitude = vpoint.Latitude;
                    point.Longitude = vpoint.Longitude;
                    point.Name = vpoint.Name;
                    point.UpdateDate = DateTime.Now;
                    point.Version = vpoint.Version;
                    point.CreateDate = vpoint.CreateDate;
                    point.MediaObjects.Clear();
                    foreach (var media in vpoint.MediaObjects)
                    {
                        point.MediaObjects.Add(media);
                    }
                });
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointManager", "AddRoutePoint", e, false);
            }
            return returnid;
        }
        internal bool Delete(ViewRoutePoint viewRoutePoint)
        {
            bool result = false;
            string rId = viewRoutePoint.RouteId;
            try
            {
                _realmInstance.Write(() =>
                {
                    var point = _realmInstance.Find<RoutePoint>(viewRoutePoint.Id);
                    foreach(var item in point.MediaObjects)
                    {
                        _realmInstance.Remove(item);
                    }
                    _realmInstance.Remove(point);
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointManager", "DeleteRoutePoint", e, false);
            }

            return result;
        }

        /*internal IEnumerable<RoutePoint> GetNotSynced()
        {
            return _realmInstance.All<RoutePoint>().Where(item => !item.ServerSynced);
        }*/
        internal IEnumerable<RoutePoint> GetPoints()
        {
            return _realmInstance.All<RoutePoint>();
        }

        /*internal void UpdateLocalData(Route route, List<RoutePoint> points)
        {
            foreach (var point in points)
            {
                Add(point, route);
            }
        }*/

        internal string GetDefaultImageFilename(string routePointId)
        {
            string filename = string.Empty;

            RoutePoint point = _realmInstance.Find<RoutePoint>(routePointId);
            if (point?.MediaObjects.Count > 0)
            {
                filename = $"img_{point.MediaObjects[0].RoutePointMediaObjectId}.jpg";
            }
            else
            {
                filename = "emptyimg.png";
            }

            return filename;
        }

        internal string GetDefaultImagePreviewFilename(string routePointId)
        {
            string filename = string.Empty;
            RoutePoint point = _realmInstance.Find<RoutePoint>(routePointId);
            if (point?.MediaObjects.Count > 0)
            {
                filename = $"img_{point.MediaObjects[0].RoutePointMediaObjectId}_preview.jpg";
            }
            else
            {
                filename = "emptyimg.png";
            }

            return filename;
        }
    }
}
