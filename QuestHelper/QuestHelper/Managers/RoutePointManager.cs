using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
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

        internal bool Add(RoutePoint point, Route route)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    _realmInstance.Add(route);
                    point.IsNew = false;
                    point.MainRoute = route;
                    _realmInstance.Add(point);
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                //пишем лог
            }
            return result;
        }

        internal IEnumerable<RoutePoint> GetNotSynced()
        {
            return _realmInstance.All<RoutePoint>().Where(item => !item.ServerSynced);
        }

        internal void UpdateLocalData(Route route, List<RoutePoint> points)
        {
            foreach (var point in points)
            {
                Add(point, route);
            }
        }

        internal void SetSyncStatus(object routeId, bool added)
        {
            throw new NotImplementedException();
        }

        internal void AddMediaObject(RoutePoint point, string imagePreviewFilePath, string imageFilePath)
        {
            _realmInstance.Write(() =>
            {
                point.MediaObjects.Clear();
                point.MediaObjects.Add(new RoutePointMediaObject() { FileName = imageFilePath, Point = point, FileNamePreview = imagePreviewFilePath });
                point.UpdateDate = DateTime.Now;
            });
        }

        internal string GetDefaultImageFilename(RoutePoint point)
        {
            string filename = string.Empty;

            if (point.MediaObjects.Count > 0)
            {
                filename = point.MediaObjects[0].FileName;
            }
            else
            {
                filename = "emptyimg.png";
            }

            return filename;
        }

        internal string GetDefaultImagePreviewFilename(RoutePoint point)
        {
            string filename = string.Empty;

            if (point.MediaObjects.Count > 0)
            {
                filename = point.MediaObjects[0].FileNamePreview;
            }
            else
            {
                filename = "emptyimg.png";
            }

            return filename;
        }

        internal void SetName(RoutePoint point, string name)
        {
            _realmInstance.Write(() =>
            {
                point.Name = name;
                point.UpdateDate = DateTime.Now;
            });
        }
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
                //пишем лог
            }
            return result;
        }
    }
}
