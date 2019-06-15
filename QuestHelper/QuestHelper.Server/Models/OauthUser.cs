using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestHelper.Server.Models
{
    public class OauthUser
    {
        public OauthUser()
        {
        }
        [Key]
        public string OauthUserId { get; set; }
        public string Name { get; set; }
        public string AuthenticatorId { get; set; }
        public string ImgUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public string Email { get; set; }
        public string Locale { get; set; }
        /// <summary>
        /// Версия записи
        /// </summary>
        public int Version { get; set; }
        public string UserId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("UserId")]
        public User User { get; set; }
    }
}
