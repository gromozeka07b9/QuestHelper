﻿using Realms;
using System;
using System.Collections.Generic;
using System.Text;
using Realms.Exceptions;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public static class RealmAppInstance
    {
        private static Realm _realmInstance = null;
        public static Realm GetAppInstance()
        {
            if(_realmInstance == null)
            {
                try
                {
                    var config = new RealmConfiguration()
                    {
                        SchemaVersion = 1, MigrationCallback = (migration, oldSchemaVersion) =>
                        {

                        }
                    };
                    _realmInstance = Realm.GetInstance(config);
                }
                catch (Exception e)
                {
                    HandleError.Process("RealmAppInstance", "GetAppInstance", e, true);
                }
            }
            return _realmInstance;
        }
            

    }
}
