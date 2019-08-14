using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class RouteShare
    {
        public RouteShare()
        {
        }
        [Key]
        public string RouteShareId { get; set; }
        public string RouteId { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserId { get; set; }
        public string ReferenceHash { get; set; }
    }
}
