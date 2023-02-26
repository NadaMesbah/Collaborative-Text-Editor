﻿using System.ComponentModel.DataAnnotations;

namespace RealTimeCollaborativeApp.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
