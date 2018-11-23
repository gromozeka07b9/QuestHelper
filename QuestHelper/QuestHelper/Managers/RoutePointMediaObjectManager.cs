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
        internal static Realm GetRealmInstance()
        {
            return RealmAppInstance.GetAppInstance();
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
                //пишем лог
            }
            return result;
        }
        /*public IEnumerable<RoutePointMediaObject> GetNotSyncedFiles()
        {
            return _realmInstance.All<RoutePointMediaObject>().Where(media=>!media.ServerSynced);
        }*/

        public void UpdateLocalData(RoutePoint point, List<RoutePointMediaObject> mediaObjects)
        {
            foreach (var media in mediaObjects)
            {
                Add(point, media);
            }
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
                //пишем лог
            }
            return result;
        }

        internal void Add(RoutePoint point, string imagePreviewFilePath, string imageFilePath)
        {
            _realmInstance.Write(() =>
            {
                point.MediaObjects.Clear();
                point.MediaObjects.Add(new RoutePointMediaObject()
                {
                    FileName = imageFilePath,
                    Point = point,
                    FileNamePreview = imagePreviewFilePath,
                    RoutePointId = point.RoutePointId,
                    ServerSynced = false
                });
                point.UpdateDate = DateTime.Now;
            });
        }

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
                    mediaObject.Version = vmedia.Version;
                    //mediaObject.FileName = vmedia.FileName;
                    //mediaObject.FileNamePreview = vmedia.FileNamePreview;
                });
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "SaveRoutePointMediaObject", e, false);
            }

            return returnId;
        }


        internal IEnumerable<RoutePointMediaObject> GetMediaObjects()
        {
            return _realmInstance.All<RoutePointMediaObject>();
        }

        internal RoutePointMediaObject GetMediaObjectById(string mediaId)
        {
            return _realmInstance.All<RoutePointMediaObject>().SingleOrDefault(x => x.RoutePointMediaObjectId == mediaId);
        }

        internal IEnumerable<RoutePointMediaObject> GetMediaObjectsByRoutePointId(string routePointId)
        {
            return _realmInstance.All<RoutePointMediaObject>().Where(x=>x.RoutePointId == routePointId);
        }
        /*internal IEnumerable<RoutePointMediaObject> GetNotSynced()
        {
            return _realmInstance.All<RoutePointMediaObject>().Where(item => !item.ServerSynced);
        }*/
    }
}
