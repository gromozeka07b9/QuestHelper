using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class User : RealmObject
    {
        [PrimaryKey]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Email { get; set; }
    }
}
