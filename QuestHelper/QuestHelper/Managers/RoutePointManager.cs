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
            return _realmInstance.Find<RoutePoint>(id);
        }

        internal IEnumerable<RoutePoint> GetPointsByRoute(Route routeItem)
        {
            var collection = _realmInstance.All<RoutePoint>().Where(point => point.MainRoute == routeItem).OrderByDescending(point => point.CreateDate);
            return collection;
        }
        internal RoutePoint GetPointByCoordinates(double latitude, double longitude)
        {
            var collection = _realmInstance.All<RoutePoint>().Where(point => point.Latitude == latitude && point.Longitude == longitude);
            return collection.FirstOrDefault();
        }

        internal bool Save(ViewRoutePoint vpoint)
        {
            bool result = false;
            RouteManager routeManager = new RouteManager();
            try
            {
                _realmInstance.Write(() =>
                {
                    RoutePoint point;
                    if (string.IsNullOrEmpty(vpoint.Id))
                    {
                        point = new RoutePoint();
                        point.RouteId = vpoint.RouteId;
                        point.MainRoute = routeManager.GetRouteById(vpoint.RouteId);
                        point.MainRoute.Points.Add(point);//?
                        _realmInstance.Add(point);
                    }
                    else
                    {
                        point = _realmInstance.Find<RoutePoint>(vpoint.Id);
                    }
                    point.Address = vpoint.Address;
                    point.Description = vpoint.Description;
                    point.Latitude = vpoint.Latitude;
                    point.Longitude = vpoint.Longitude;
                    point.Name = vpoint.Name;
                    point.UpdateDate = DateTime.Now;
                    if((point.MediaObjects.Count > 0) && (point.MediaObjects[0].FileName != vpoint.ImagePath))
                    {
                        point.MediaObjects.Clear();
                    }
                    if(!string.IsNullOrEmpty(vpoint.ImagePath) && (vpoint.ImagePath != "emptyimg.png"))
                    {
                        RoutePointMediaObject defaultMedia = new RoutePointMediaObject();
                        defaultMedia.FileName = vpoint.ImagePath;
                        defaultMedia.FileNamePreview = vpoint.ImagePreviewPath;
                        defaultMedia.RoutePointId = point.RoutePointId;
                        defaultMedia.Point = point;
                        point.MediaObjects.Add(defaultMedia);
                    }
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointManager", "AddRoutePoint", e, false);
            }
            return result;
        }

        internal IEnumerable<RoutePoint> GetNotSynced()
        {
            return _realmInstance.All<RoutePoint>().Where(item => !item.ServerSynced);
        }

        internal void UpdateLocalData(Route route, List<RoutePoint> points)
        {
            /*foreach (var point in points)
            {
                Add(point, route);
            }*/
        }

        /*internal void AddMediaObject(RoutePoint point, string imagePreviewFilePath, string imageFilePath)
        {
            _realmInstance.Write(() =>
            {
                point.MediaObjects.Clear();
                point.MediaObjects.Add(new RoutePointMediaObject() { FileName = imageFilePath, Point = point, FileNamePreview = imagePreviewFilePath });
                point.UpdateDate = DateTime.Now;
            });
        }*/

        internal string GetDefaultImageFilename(string routePointId)
        {
            string filename = string.Empty;

            RoutePoint point = _realmInstance.Find<RoutePoint>(routePointId);
            if (point?.MediaObjects.Count > 0)
            {
                filename = point.MediaObjects[0].FileName;
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
                filename = point.MediaObjects[0].FileNamePreview;
            }
            else
            {
                filename = GetEmptyImageFilename();
            }

            return filename;
        }

        internal string GetEmptyImageFilename()
        {
            return "emptyimg.png";
        }
        /*internal void SetName(RoutePoint point, string name)
        {
            _realmInstance.Write(() =>
            {
                point.Name = name;
                point.UpdateDate = DateTime.Now;
            });
        }*/
        public bool SetSyncStatus(string Id, bool Status)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    var point = _realmInstance.Find<RoutePoint>(Id);
                    point.ServerSynced = Status;
                    point.ServerSyncedDate = DateTime.Now;
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointManager", "SetSyncStatus", e, false);
            }
            return result;
        }
    }
}
