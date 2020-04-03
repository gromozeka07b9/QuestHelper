using System;
using System.Collections.Generic;

namespace QuestHelper.SharedModelsWS
{
    public class UserAccount : ModelBase
    {
        public UserAccount()
        {
        }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Email { get; set; }
    }
}
