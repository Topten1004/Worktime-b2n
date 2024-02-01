namespace Worktime.ViewModel
{
    public class PassageFilterDualVM
    {
        public string SSN { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string OddDate { get; set; } = string.Empty;
        public string OddLogTime { get; set; } = string.Empty;
        public string OddPointerName { get; set; } = string.Empty;
        public string EvenDate { get; set; } = string.Empty;
        public string EvenLogTime { get; set; } = string.Empty;
        public string EvenPointerName { get; set; } = string.Empty;

        public string? Info { get; set; } = string.Empty;
    }
}
