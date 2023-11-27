using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JagerGroupIS.Models.Database
{
    [Table("RoleElectionSetup")]
    public class RoleElectionSetup
    {
        [Key]
        [Column("RoleElectionSetupID")]
        public int ID { get; set; }

        [Column("ElectionID")]
        public int ElectionID { get; set; }

        [Column("RoleID")]
        public string DisordRoleID { get; set; }

        [Column("IsTakingPart")]
        public bool IsTackingPart { get; set; }

        public virtual Election? Election { get; set; }
    }
}
