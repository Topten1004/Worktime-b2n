namespace Worktime.ViewModel
{
    public class PassageVM
    {
        public int Id { get; set; }
        public DateTime LogTime { get; set; }

        public int Type { get; set; }
        public string PointerName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
    }
}
