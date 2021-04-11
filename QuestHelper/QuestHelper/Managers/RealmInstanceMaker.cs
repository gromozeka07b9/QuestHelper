using Realms;
using System;

namespace QuestHelper.Managers
{
    public class RealmInstanceMaker
    {
        internal Realm RealmInstance
        {
            get
            {
                Realm _instance = null;
                try
                {
                    var config = new RealmConfiguration()
                    {
                        //v3 - added Route.ObjVerHash
                        //v4 - added RoutePointMediaObject.MediaType
                        //v5 - added Route.ImgFilename & Route.Description
                        //v6 = added RoutePointMediaObject.Processed and RoutePointMediaObject.ProcessResultText
                        //v7 - added Poi
                        //v8 - Poi - deleted RouteId, +UpdateDate,Address,ByRoutePointId,IsPublished
                        //v9 - Poi = added ByRouteId
                        //v10 - added LocalFile
                        //v11 - added indexes into LocalFile
                        //v12 - added RouteTrackPlace, upgrade Realm from 3.4 to 10.1.2
                        SchemaVersion = 12,
                        MigrationCallback = (migration, oldSchemaVersion) =>
                        {

                        }
                    };
                    _instance = Realm.GetInstance(config);
                }
                catch (Exception e)
                {
                    HandleError.Process("RealmInstanceMaker", "RealmInstance", e, true);
                }
                return _instance;
            }
        }
    }
}