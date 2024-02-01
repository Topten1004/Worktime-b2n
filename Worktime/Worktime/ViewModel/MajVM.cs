namespace Worktime.ViewModel
{
    public class MajVM
    {
        public MajVM() {

            Passages = new List<PassageVM>();
            Summaries = new List<SummaryVM>();
        }

        public List<PassageVM> Passages { get; set; }

        public List<SummaryVM> Summaries { get; set; }
    }
}
