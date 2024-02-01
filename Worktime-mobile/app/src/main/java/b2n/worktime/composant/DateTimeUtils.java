package b2n.worktime.composant;

import java.text.SimpleDateFormat;
import java.util.Date;

public class DateTimeUtils {
    public static String formatDateTime(Date date) {
        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
        return dateFormat.format(date);
    }
}
