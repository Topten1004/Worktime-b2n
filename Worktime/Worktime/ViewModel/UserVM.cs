using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Worktime.ViewModel
{
    public class UserVM
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Login { get; set; } = null!;
        public string Mdp { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
