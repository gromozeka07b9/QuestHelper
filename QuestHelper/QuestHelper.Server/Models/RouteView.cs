using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models
{
    /// <summary>
    /// Данные о количестве просмотров маршрутов
    /// </summary>
    public class RouteView
    {
        [Key]
        public int Id { get; set; }
        public string RouteId { get; set; }
        public string UserId { get; set; }
        public DateTime ViewDate { get; set; }

    }
}
