using System.Collections.Generic;

namespace QuestHelper.Model.WS
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
