using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuestHelper.Server.Models
{
    public class Route
    {
        public Route()
        {
        }
        [Key]
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public string CreatorId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShared { get; set; }
        public bool IsPublished { get; set; }
        public string ImgFilename { get; set; }
        public string Description { get; set; }
        public string VersionsHash { get; set; }
        public string VersionsList { get; set; }
        
        [NotMapped]
        public string CoverImgBase64 { get; set; }
    }
}
