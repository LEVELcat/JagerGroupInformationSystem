﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JagerGroupIS.Models.Database
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int ID { get; set; }

        [Column("DiscordUserID")]
        public long? DiscordUserID { get; set; }

        [Column("Steam64ID")]
        public long? Steam64ID { get; set; }

        public virtual ICollection<Vote>? Votes { get; set; }
    }
}
