using Realms;
using System;
using System.Collections.Generic;
using System.Text;

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
                    _realmInstance = Realm.GetInstance();
                }
                catch (Exception)
                {
                    //ToDo:логгировать
                }
            }
            return _realmInstance;
        }
            

    }
}
