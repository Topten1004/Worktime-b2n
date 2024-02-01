using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worktime.Models
{
    [Table("setting")]

    public class Setting
    {

        [Key]
        [Column("id")]

        public int id { get; set; }

        [Column("geolocation")]

        public int geolocation { get; set; }
    }
}
