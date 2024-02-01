package b2n.worktime.dto;

import java.sql.Date;

public class ScanSuccess {

    public String pointerName;
    public String logTime;
    public String userName;

    public ScanSuccess(String _pointerName, String _logTime, String _userName) {
        pointerName = _pointerName;
        logTime = _logTime;
        userName = _userName;
    }
}
