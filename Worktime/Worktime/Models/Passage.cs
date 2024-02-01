using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Worktime.Models
{
    [Table("Passage")]
    public class Passage
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("EmployeeId")]
        public int EmployeeId{ get; set; }

        [Column("PointerId")]
        public int PointerId { get; set; }

        [Column("LogTime")]
        public DateTime LogTime { get; set; }

        [Column("Longitude")]
        public float Longitude { get; set; }

        [Column("Latitude")]
        public float Latitude { get; set; }

        // Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("PointerId")]
        public virtual Pointer Pointer { get; set; }
    }
}
