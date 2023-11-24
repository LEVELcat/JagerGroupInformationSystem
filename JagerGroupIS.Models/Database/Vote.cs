using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JagerGroupIS.Models.Enums;

namespace JagerGroupIS.Models.Database
{
    [Table("Vote")]
    public class Vote
    {
        [Key]
        [Column("VoteID")]
        public int ID { get; set; }

        [Column("ElectionID")]
        public int ElectionID { get; set; }

        [Column("UserID")]
        public int UserID { get; set; }

        [Column("VoteType")]
        public VoteType VoteType { get; set; }

        [Column("VoteTimeUTC")]
        public DateTime VoteTime { get; set; }

        public virtual Election? Election { get; set; }

        public virtual User? User { get; set; } 
    }
}
