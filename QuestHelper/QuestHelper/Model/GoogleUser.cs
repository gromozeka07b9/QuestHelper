using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model
{
    public class OAuthUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Uri ImgUrl { get; set; }
    }
}
