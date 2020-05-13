using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Model
{
    public class GalleryImage
    {
        public string ImagePath { get; set; }
        public DateTime CreateDate { get; internal set; }
        public string ImageCreateDateString
        {
            get
            {
                return CreateDate.ToString("dd MMMM yyyy");
            }
        }
    }

}
