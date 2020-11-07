using System;
using Newtonsoft.Json.Linq;

namespace QuestHelper.Server.Controllers.v2
{
    internal class FilterParameters
    {
        private JObject _jsonFilter;

        public FilterParameters(string jsonFilter)
        {
            this._jsonFilter = !string.IsNullOrEmpty(jsonFilter) ? JObject.Parse(jsonFilter) : new JObject();
        }

        internal bool isFilterPresent(string name)
        {
            return _jsonFilter.ContainsKey(name);
        }

        internal string GetStringByName(string keyName)
        {
            return _jsonFilter.Property(keyName).Value.ToString();
        }
        internal bool GetBooleanByName(string keyName)
        {
            return _jsonFilter.Property(keyName).Value.ToObject<bool>();
        }

        internal DateTime GetDateTimeByName(string keyName)
        {
            return _jsonFilter.Property(keyName).Value.ToObject<DateTime>();
        }
    }
}