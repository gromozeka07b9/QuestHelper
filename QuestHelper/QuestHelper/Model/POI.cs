using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace QuestHelper.Model
{
    public class POI
    {
        public Position Position { get; set; }
        public string Address { get; set; }
        public string PathToPicture { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
