namespace Worktime.ViewModel
{
    public class GetGeolocationVM
    {
        public string SSN { get; set; } = string.Empty;
        public string PointerName { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
