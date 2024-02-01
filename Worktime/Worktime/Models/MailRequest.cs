using Worktime.ViewModel;

namespace Worktime.Models
{
    public class MajReportRequest
    {
        public MajReportRequest() {

            Absents = new List<string>();
            Passages = new List<PassageVM>();
        }

        public string ToEmail { get; set; } =  string.Empty;

        public DateTime ScheduleTime { get; set; }

        public List<string> Absents { get; set; }

        public List<PassageVM> Passages { get; set; }
    }
}
