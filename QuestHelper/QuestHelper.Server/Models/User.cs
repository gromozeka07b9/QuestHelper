using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class User
    {
        public User()
        {
        }
        [Key]
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Email { get; set; }
        public ICollection<Route> Routes { get; set; }
    }
}
