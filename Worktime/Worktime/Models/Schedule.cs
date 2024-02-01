using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worktime.Models
{
    [Table("schedule")]
    public class Schedule
    {
        [Key]
        [Column("Id")]
        public int id { get; set; }

        [Column("timelist")]

        public string timelist { get; set; } = string.Empty;

        [Column("addresslist")]

        public string addresslist { get; set; } = string.Empty;
    }
}
