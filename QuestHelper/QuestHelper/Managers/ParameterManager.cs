using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;

namespace QuestHelper.Managers
{
    /// <summary>
    /// Используется для хранения параметров приложения в формате key/value
    /// </summary>
    public class ParameterManager
    {
        readonly Realm _realmInstance;
        public ParameterManager()
        {
            _realmInstance = RealmAppInstance.GetAppInstance();
        }

        public bool Set(string key, string value)
        {
            bool result = false;
            try
            {
                _realmInstance.Write(() =>
                {
                    var paramObject = _realmInstance.Find<Parameter>(key);
                    if (paramObject != null)
                    {
                        _realmInstance.Remove(paramObject);
                    }
                    _realmInstance.Add(new Parameter(){KeyName = key, Value = value});
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("ParameterManager", "Set", e, false);
            }
            return result;
        }

        internal bool Get(string key, out string value)
        {
            value = string.Empty;
            var result =_realmInstance.Find<Parameter>(key);
            if (result != null)
            {
                value = result.Value;
            }
            return !string.IsNullOrEmpty(value);
        }
    }
}
