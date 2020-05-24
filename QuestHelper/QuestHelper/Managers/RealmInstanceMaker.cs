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
                        SchemaVersion = 10,
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