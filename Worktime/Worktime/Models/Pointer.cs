using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Worktime.Controllers;

namespace Worktime.Models
{
    [Table("Pointer")]
    public class Pointer
    {

        public Pointer() {
            Passages = new List<Passage>();
        }

        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Code")]
        public string Code { get; set; } = string.Empty;

        public virtual List<Passage> Passages { get; set; }
    }
}
