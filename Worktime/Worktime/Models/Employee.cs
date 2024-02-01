using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worktime.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("SSN")]
        [StringLength(15)]
        public string SSN { get; set; } = string.Empty;

        [Column("TAG")]
        [StringLength(15)]
        public string? Tag { get; set; } = string.Empty;

        [Column("FirstName")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Column("LastName")]
        public string LastName { get; set; } = string.Empty;

        [Column("EntryDate")]
        public DateTime EntryDate { get; set; }

        [Column("Info")]
        public string? Info { get; set; } = string.Empty;

        [Column("ReleaseDate")]
        public DateTime? ReleaseDate { get; set; }

        [Column("WebAccess")]
        public bool WebAccess { get; set; } = false;

        [Column("Enable")]
        public bool Enable { get; set; } = true;

        public virtual List<Passage> Passages { get; set; }
    }
}