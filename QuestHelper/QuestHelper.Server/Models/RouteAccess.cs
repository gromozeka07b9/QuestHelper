using System;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class RouteAccess
    {
        public RouteAccess()
        {
        }
        [Key]
        public string RouteAccessId { get; set; }
        public string RouteId { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserId { get; set; }
        public bool CanChange { get; set; }
    }
}
