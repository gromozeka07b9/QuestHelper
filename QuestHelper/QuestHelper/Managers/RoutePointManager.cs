﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;

namespace QuestHelper.Managers
{
    public class RoutePointManager : RealmInstanceMaker
    {
        public RoutePointManager()
        {
        }

        internal RoutePoint GetPointById(string id)
        {
            return !string.IsNullOrEmpty(id)? RealmInstance.Find<RoutePoint>(id):null;
        }

        public IEnumerable<ViewRoutePoint> GetPointsByRouteId(string routeId, bool withDeleted = false)
        {
            List<ViewRoutePoint> collection = new List<ViewRoutePoint>();
            try
            {
                var collectionRealm = withDeleted
                    ? RealmInstance.All<RoutePoint>().Where(point => point.RouteId == routeId)
                        .OrderBy(point => point.CreateDate)
                    : RealmInstance.All<RoutePoint>().Where(point => point.RouteId == routeId && !point.IsDeleted)
                        .OrderBy(point => point.CreateDate);
                foreach (var item in collectionRealm)
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
            var collection = RealmInstance.All<RoutePoint>().Where(point => point.Latitude == latitude && point.Longitude == longitude);
            return collection.FirstOrDefault();
        }

        internal string Save(ViewRoutePoint vpoint)
        {
            string returnid = string.Empty;
            RouteManager routeManager = new RouteManager();
            vpoint.Version = vpoint.Version == 0 ? 1 : vpoint.Version;
            if (vpoint.CreateDate.Year == 1)
            {
                HandleError.Process("RoutePointManager", "Save", new Exception("CreateDate is empty"), false,$"routePointId:[{vpoint.RoutePointId}]" );
                //throw new Exception("create date year = 1");
                //vpoint.CreateDate = new DateTime(2020,1,1);
            }
            try
            {
                RealmInstance.Write(() =>
                {
                    RoutePoint point = !string.IsNullOrEmpty(vpoint.Id) ? RealmInstance.Find<RoutePoint>(vpoint.Id) : null;
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
                        RealmInstance.Add(point);
                    }
                    else
                    {
                        //point = _realmInstance.Find<RoutePoint>(vpoint.Id);
                    }

                    if (point.Version != vpoint.Version)
                    {
                        //нужно для того, чтобы синхронизация обнаружила отличия от сервера и проверила версии с последующей отправкой изменений на сервер
                        point.MainRoute.ObjVerHash = string.Empty;
                    }
                    returnid = point.RoutePointId;
                    point.Address = vpoint.Address;
                    point.Description = vpoint.Description;
                    point.Latitude = vpoint.Latitude;
                    point.Longitude = vpoint.Longitude;
                    point.Name = vpoint.Name;
                    point.UpdateDate = DateTime.Now;
                    point.Version = vpoint.Version;
                    point.IsDeleted = vpoint.IsDeleted;
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

        internal void DeleteObjectFromLocalStorage(ViewRoutePoint vpoint)
        {
            if (vpoint != null)
            {
                try
                {
                    RoutePoint point = !string.IsNullOrEmpty(vpoint.Id) ? RealmInstance.Find<RoutePoint>(vpoint.Id) : null;
                    if (point != null)
                    {
                        RealmInstance.Write(() =>
                        {
                            RealmInstance.Remove(point);
                        });
                    }
                }
                catch (Exception e)
                {
                    HandleError.Process("RoutePointManager", "DeleteObjectFromLocalStorage", e, false);
                }
            }
        }

        internal bool Delete(ViewRoutePoint viewRoutePoint)
        {
            bool result = false;
            string rId = viewRoutePoint.RouteId;
            try
            {
                RealmInstance.Write(() =>
                {
                    var point = RealmInstance.Find<RoutePoint>(viewRoutePoint.Id);
                    foreach(var item in point.MediaObjects)
                    {
                        RealmInstance.Remove(item);
                    }
                    RealmInstance.Remove(point);
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
            var deletedRoutes = RealmInstance.All<Route>().Where(r => r.IsDeleted).ToList().Select(d=>d.RouteId);
            return RealmInstance.All<RoutePoint>().ToList().Where(r => (!deletedRoutes.Any(d => d == r.RouteId)));
            //return _realmInstance.All<RoutePoint>().Where(r=>(!deletedRoutes.Any(d=>d == r.RouteId)));
        }
        internal Tuple<RoutePoint, RoutePoint> GetFirstAndLastPoints(string routeId)
        {
            var routePoints = RealmInstance.All<RoutePoint>().Where(p => p.RouteId == routeId).OrderBy(p=>p.CreateDate);
            if (routePoints.Count() > 0)
            {
                var first = routePoints.FirstOrDefault();
                var last = routePoints.LastOrDefault();
                return new Tuple<RoutePoint, RoutePoint>(first, last);
            }
            return new Tuple<RoutePoint,RoutePoint>(new RoutePoint(), new RoutePoint());
        }

        internal (ViewRoutePoint, ViewRoutePoint) GetFirstAndLastViewRoutePoints(string routeId)
        {
            var routePoints = RealmInstance.All<RoutePoint>().Where(p => p.RouteId == routeId).OrderBy(p => p.CreateDate);
            if (routePoints.Count() > 0)
            {
                var first = new ViewRoutePoint(routeId, routePoints.FirstOrDefault()?.RoutePointId);
                var last = new ViewRoutePoint(routeId, routePoints.LastOrDefault()?.RoutePointId);
                return (first, last);
            }
            return (new ViewRoutePoint(), new ViewRoutePoint());
        }

        internal string GetDefaultImageFilename(string routePointId)
        {
            string filename = string.Empty;

            RoutePoint point = RealmInstance.Find<RoutePoint>(routePointId);
            if (point?.MediaObjects.Count > 0)
            {
                filename = $"img_{point.MediaObjects[0].RoutePointMediaObjectId}.jpg";
            }
            else
            {
                filename = "emptyphoto.png";
            }

            return filename;
        }

        internal string GetDefaultImagePreviewFilename(string routePointId)
        {
            string filename = string.Empty;
            RoutePoint point = RealmInstance.Find<RoutePoint>(routePointId);
            if (point?.MediaObjects.Count > 0)
            {
                filename = $"img_{point.MediaObjects[0].RoutePointMediaObjectId}_preview.jpg";
            }
            else
            {
                filename = "emptylist.png";
            }

            return filename;
        }
    }
}
