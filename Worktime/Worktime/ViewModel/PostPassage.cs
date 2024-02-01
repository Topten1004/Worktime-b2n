namespace Worktime.ViewModel
{
    public class PostPassage
    {
        public int PointerId {  get; set; }

        public string Tag { get; set; } = string.Empty;

        public string LogTime { get; set; } = string.Empty;
        public float Longitude { get; set; } = 0;
        public float Latitude { get; set; } = 0;
    }
}
