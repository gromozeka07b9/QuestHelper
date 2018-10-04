using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.Model.DB;
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

        internal bool Add(RoutePoint point, RoutePointMediaObject mediaObject)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    //mediaObject.Point = point;
                    //mediaObject.RoutePointId = point.RoutePointId;
                    //_realmInstance.Add(mediaObject, true);
                    point.MediaObjects.Clear();
                    point.MediaObjects.Add(new RoutePointMediaObject() { FileName = mediaObject.FileName, Point = point, PreviewImage = mediaObject.PreviewImage });
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
        internal bool SetSyncStatus(string Id, bool Status)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    var mediaObject = _realmInstance.Find<RoutePointMediaObject>(Id);
                    mediaObject.ServerSynced = Status;
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

        internal void UpdateLocalData(RoutePoint point, List<RoutePointMediaObject> mediaObjects)
        {
            foreach (var media in mediaObjects)
            {
                Add(point, media);
            }
        }

        internal IEnumerable<RoutePointMediaObject> GetNotSyncedFiles()
        {
            var collection = _realmInstance.All<Model.DB.RoutePointMediaObject>();
            return collection;
        }
    }
}
