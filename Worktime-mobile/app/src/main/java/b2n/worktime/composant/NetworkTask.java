package b2n.worktime.composant;

import android.os.AsyncTask;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;

public class NetworkTask extends AsyncTask<String, String, Boolean> {

    public NetworkTask() {
        super();
    }

    protected Boolean doInBackground(String... params) {
        try {
            int port = 443;
            if(!params[0].contains(".worktime.nc"))
                port = 44347;
            SocketAddress sa = new InetSocketAddress(params[0], port);
            Socket sock = new Socket();
            int timeoutMs = 1200; // Reduced timeout to 200ms
            sock.connect(sa, timeoutMs);
            return true;
        } catch (IOException e) {
            // Handle the exception and return false for network not connected
            return false;
        }
    }

    @Override
    protected void onPostExecute(Boolean result) {
        super.onPostExecute(result);
    }
}
