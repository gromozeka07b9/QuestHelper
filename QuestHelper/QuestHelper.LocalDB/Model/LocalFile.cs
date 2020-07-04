using Realms;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace QuestHelper.LocalDB.Model
{
    public class LocalFile : RealmObject
    {
        [PrimaryKey]
        public string LocalFileId { get; set; } = Guid.NewGuid().ToString();
        [Indexed]
        public string SourceFileName { get; set; }
        public string SourcePath { get; set; }
        public string ImagePreviewFileName { get; set; }
        [Indexed]
        public DateTimeOffset FileNameDate { get; set; } = DateTime.MinValue;
        public DateTimeOffset CreateDate { get; set; } = DateTime.Now;
        public string Address { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
