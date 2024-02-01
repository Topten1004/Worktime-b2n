using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Writers;

namespace Worktime.ViewModel
{
    public class EmployeeVM
    {
        public string SSN { get; set; } = string.Empty;
        public string? Tag { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? Info { get; set; } = string.Empty;
        public bool WebAccess { get; set; } = false;
        public bool Enable { get; set; } = false;
        public DateTime? EntryDate { get; set; }
        public DateTime? ReleaseDate { get; set; }

    }
}
