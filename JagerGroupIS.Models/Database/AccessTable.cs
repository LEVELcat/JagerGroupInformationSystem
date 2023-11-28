using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.Models.Database
{
    [Table("AccessTable")]
    public class AccessTable
    {
        [Key]
        [Column("AccessID")]
        public int ID { get; set; }

        [Column("GuildID")]
        public long GuildID { get; set; }

        [Column("UserID")]
        public long RoleID { get; set; }
    }
}
