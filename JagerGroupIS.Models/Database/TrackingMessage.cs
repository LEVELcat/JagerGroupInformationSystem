﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagerGroupIS.Models.Enums;

namespace JagerGroupIS.Models.Database
{
    [Table("TrackingMessage")]
    public class TrackingMessage
    {
        [Key]
        [Column("TrackingMessageID")]
        public int ID { get; set; }

        [Column("GuildID")]
        public long GuildID { get; set; }

        [Column("ChanelID")]
        public long ChanelID { get; set; }

        [Column("MessageID")]
        public long MessageID { get; set; }

        [Column("RefreshTime")]
        public TimeSpan RefreshTime {  get; set; }

        [Column("MessageTypeID")]
        public MessageType MessageType { get; set; }
    }
}
