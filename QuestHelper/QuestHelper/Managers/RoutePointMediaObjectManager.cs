using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;

namespace QuestHelper.Managers
{
    public class RoutePointMediaObjectManager
    {
        Realm _realmInstance;
        public RoutePointMediaObjectManager()
        {
            _realmInstance = RealmAppInstance.GetAppInstance();
        }

        public bool SetSyncStatus(string Id, bool Status)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    var mediaObject = _realmInstance.Find<RoutePointMediaObject>(Id);
                    mediaObject.ServerSynced = Status;
                    mediaObject.ServerSyncedDate = DateTime.Now;
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "SetSyncStatus", e, false);
            }
            return result;
        }

        private bool Add(RoutePoint point, RoutePointMediaObject mediaObject)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    point.MediaObjects.Clear();
                    point.MediaObjects.Add(mediaObject);
                    _realmInstance.Add(point, true);
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "Add", e, false);
            }
            return result;
        }

        /*internal void Add(RoutePoint point, string imagePreviewFilePath, string imageFilePath)
        {
            _realmInstance.Write(() =>
            {
                point.MediaObjects.Clear();
                point.MediaObjects.Add(new RoutePointMediaObject()
                {
                    Point = point,
                    RoutePointId = point.RoutePointId,
                    ServerSynced = false
                });
                point.UpdateDate = DateTime.Now;
            });
        }*/

        internal string Save(ViewRoutePointMediaObject vmedia)
        {
            string returnId = string.Empty;

            try
            {
                _realmInstance.Write(() =>
                {
                    RoutePointMediaObject mediaObject = !string.IsNullOrEmpty(vmedia.Id) ? _realmInstance.Find<RoutePointMediaObject>(vmedia.Id) : null;
                    if (mediaObject == null)
                    {
                        mediaObject = new RoutePointMediaObject();
                        mediaObject.RoutePointMediaObjectId = vmedia.Id;
                        mediaObject.RoutePointId = vmedia.RoutePointId;
                        _realmInstance.Add(mediaObject);
                    }

                    returnId = mediaObject.RoutePointMediaObjectId;
                    mediaObject.ServerSynced = vmedia.ServerSynced;
                    mediaObject.ServerSyncedDate = vmedia.ServerSyncedDate;
                    mediaObject.Version = vmedia.Version;
                });
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "SaveRoutePointMediaObject", e, false);
            }

            return returnId;
        }


        internal IEnumerable<RoutePointMediaObject> GetAllMediaObjects()
        {
            return _realmInstance.All<RoutePointMediaObject>();
        }
        internal IEnumerable<RoutePointMediaObject> GetNotSyncedMediaObjects()
        {
            return _realmInstance.All<RoutePointMediaObject>().Where(x=>!x.ServerSynced);
        }

        internal RoutePointMediaObject GetMediaObjectById(string mediaId)
        {
            return _realmInstance.All<RoutePointMediaObject>().SingleOrDefault(x => x.RoutePointMediaObjectId == mediaId);
        }

        internal IEnumerable<RoutePointMediaObject> GetMediaObjectsByRoutePointId(string routePointId)
        {
            return _realmInstance.All<RoutePointMediaObject>().Where(x=>x.RoutePointId == routePointId);
        }
        internal int GetCountByRouteId(string routeId)
        {
            int count = 0;
            var route = _realmInstance.All<Route>().Where(x => x.RouteId == routeId).FirstOrDefault();
            foreach (var point in route.Points)
            {
                count += _realmInstance.All<RoutePointMediaObject>().Where(x => x.RoutePointId == point.RoutePointId).Count();
            }

            return count;
        }
    }
}
