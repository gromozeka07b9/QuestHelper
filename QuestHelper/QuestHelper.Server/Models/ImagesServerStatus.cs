using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class ImagesServerStatus
    {
        public ImagesServerStatus()
        {
            Images = new List<Imagefile>();
        }
        public ICollection<Imagefile> Images { get; set; }

        public class Imagefile
        {
            public string Name;
            public bool OnServer;
        }
    }
}
