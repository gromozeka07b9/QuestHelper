using System;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace QuestHelper.Server.Models
{
    public class VoiceObject
    {
        public VoiceObject()
        {
        }
        [Key]
        public string VoiceObjectId { get; set; }
        public string RoutePointMediaObjectId { get; set; }
        public string TextVoiceObject { get; set; }
    }
}
