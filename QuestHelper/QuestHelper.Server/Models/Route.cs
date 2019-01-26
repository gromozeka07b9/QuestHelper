﻿using System;
using System.ComponentModel.DataAnnotations;

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
    }
}
