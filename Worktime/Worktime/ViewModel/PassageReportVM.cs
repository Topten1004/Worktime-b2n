namespace Worktime.ViewModel
{
    public class PassageReportVM
    {
        public PassageReportVM() {

            Passages = new List<PassageVM>();
            Summaries = new List<SummaryVM>();
        }

        public List<PassageVM> Passages { get; set; }

        public List<SummaryVM> Summaries { get; set; }
    }

    public class SummaryVM {
        public bool IsAbsense { get; set; } = false;
        public string EmployeeName { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;

        public int Absense { get; set; }
    }
}
