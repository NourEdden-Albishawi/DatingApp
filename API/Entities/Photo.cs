﻿using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public bool IsMain { get; set; }
        public string? PublicId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
