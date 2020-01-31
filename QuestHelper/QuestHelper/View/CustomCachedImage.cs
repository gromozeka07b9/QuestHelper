using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.View
{
    public class CustomCachedImage : CachedImage
    {
        public string RoutePointId { get; set; }
        public string RoutePointMediaId { get; set; }
        public int MediaType { get; internal set; }
    }
}
