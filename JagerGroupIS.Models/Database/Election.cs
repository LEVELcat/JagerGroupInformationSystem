using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagerGroupIS.Models.Enums;

namespace JagerGroupIS.Models.Database
{
    [Table("Election")]
    public class Election
    {
        [Key]
        [Column("ElectionID")]
        public int ID { get; set; }

        [Column("StartTimeUTC")]
        public DateTime StartTime { get; set; }

        [Column("EndTimeUTC")]
        public DateTime EndTime { get; set; }

        [Column("GuildID")]
        public string GuildID { get; set; }

        [Column("ChanelID")]
        public string ChanelID { get; set; }

        [Column("MessageID")]
        public string MessageID { get; set; }

        [Column("SettingsMask")]
        public ElectionSettingsBitMask Settings { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<RoleElectionSetup> RoleSetups { get; set; }
    }
}
