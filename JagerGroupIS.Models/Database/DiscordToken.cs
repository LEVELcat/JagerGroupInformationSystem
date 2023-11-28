using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagerGroupIS.Models.Database
{
    [Table("DiscordToken")]
    public class DiscordToken
    {
        [Key]
        [Column("DiscordTokenID")]
        public int Id { get; set; }

        [Column("Token")]
        public string Token {  get; set; }
    }
}
