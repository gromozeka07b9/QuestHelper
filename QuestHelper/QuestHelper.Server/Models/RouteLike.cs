using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models
{
    public class RouteLike
    {
        [Key]
        public int Id { get; set; }
        public string RouteId { get; set; }
        public string UserId { get; set; }
        public int IsLike { get; set; }
        public DateTime SetDate { get; set; }

    }
}
