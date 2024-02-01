package b2n.worktime.dto;

public class PostPassage {
    public int pointerId;
    public String tag;

    public String logTime;
    public double longitude;
    public double latitude;
    public PostPassage(int _pointerId, String _tag, String _logTime, double _longitude, double _latitude)
    {
        pointerId = _pointerId;
        tag = _tag;
        logTime = _logTime;
        longitude = _longitude;
        latitude = _latitude;
    }
}
