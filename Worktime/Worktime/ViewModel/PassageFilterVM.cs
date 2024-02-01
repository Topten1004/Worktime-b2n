namespace Worktime.ViewModel
{
    public class PassageFilterVM
    {
        public string SSN { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string LogDate { get; set; } = string.Empty;
        public string LogTime { get; set; } = string.Empty;
        public string PointerName { get; set; } = string.Empty;

        public string? Info { get; set; } = string.Empty;
    }
}
