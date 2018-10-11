using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
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
        public IEnumerable<RoutePointMediaObject> GetNotSyncedFiles()
        {
            return _realmInstance.All<RoutePointMediaObject>();
        }

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

        internal IEnumerable<RoutePointMediaObject> GetNotSynced()
        {
            return _realmInstance.All<RoutePointMediaObject>().Where(item => !item.ServerSynced);
        }
    }
}
