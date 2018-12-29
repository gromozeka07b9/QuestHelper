using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class Parameter : RealmObject
    {
        [PrimaryKey]
        public string KeyName { get; set; }
        public string Value { get; set; }
        public DateTimeOffset CreateDate { get; set; } = DateTime.Now;
    }
}
