using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worktime.Models
{ 
    [Table("User")]
    public class User
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [Column("LastName")]
        public string LastName { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("Login")]
        [StringLength(50)]
        public string Login { get; set; } = null!;

        [Column("MDP")]
        public string Mdp { get; set; } = string.Empty;

        [Column("Role")]
        public string Role { get; set; } = string.Empty;

    }
}
