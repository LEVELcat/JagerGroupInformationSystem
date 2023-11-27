using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JagerGroupIS.Models.Database
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int ID { get; set; }

        [Column("DiscordMemberID")]
        public string? MemberID { get; set; }

        [Column("Steam64ID")]
        public string? Steam64ID { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}
