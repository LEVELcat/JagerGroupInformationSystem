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
        public DateTime StartTimeUTC { get; set; }

        [Column("EndTimeUTC")]
        public DateTimeOffset EndTimeUTC { get; set; }

        [Column("GuildID")]
        public long GuildID { get; set; }

        [Column("ChanelID")]
        public long ChanelID { get; set; }

        [Column("MessageID")]
        public long MessageID { get; set; }

        [Column("SettingsMask")]
        public ElectionSettingsBitMask Settings { get; set; }

        public virtual ICollection<Vote>? Votes { get; set; }

        public virtual ICollection<RoleElectionSetup>? RoleSetups { get; set; }


        [NotMapped]
        public DateTimeOffset EndTime
        {
            get { return EndTimeUTC.ToLocalTime(); }
            set { EndTimeUTC = value.UtcDateTime; }
        }

        [NotMapped]
        public DateTimeOffset StartTime
        {
            get { return StartTimeUTC.ToLocalTime(); }
            set { StartTimeUTC = value.UtcDateTime; }
        }
    }
}
