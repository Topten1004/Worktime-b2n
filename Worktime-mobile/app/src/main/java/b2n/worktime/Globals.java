package b2n.worktime;

import android.app.Application;
import android.content.Context;
import android.content.SharedPreferences;
import android.location.Location;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

import b2n.worktime.composant.Connectivity;
import b2n.worktime.composant.NetworkTask;
import b2n.worktime.dto.Employee;
import b2n.worktime.dto.Pointer;
import b2n.worktime.dto.ScanSuccess;

public class Globals extends Application {

    public static boolean type = false;
    public static Pointer pointer = null;
    public  static ScanSuccess ScanSuccessInfo = null;
    public static List<Employee> listEmployees = new ArrayList<>();
    public static String idConstructeur;

    public static String dns = "b2n.worktime.nc";
    public static String url = "https://b2n.worktime.nc";
    public static Boolean isWorking = false;
    public static String tag;

//  public static String dns = "10.0.2.2";
//  public static String url = "https://10.0.2.2:44328";

//  public static String urlAPIGrool = "https://10.0.2.2:44330/api/grool/";
    public static String urlAPIWorkTime = url + "/api/worktime/";
    public static Boolean dispoAPI= false;

    private static Location location;
    public static String msgErreur;

    public Location getLocation() {
        return location;
    }
    public void setLocation(Location _location) {
        location = _location;
    }

    public void ecrirePref(String cle, String valeur){
        SharedPreferences sp = getSharedPreferences("ClinoTag_Preference", MODE_PRIVATE);
        SharedPreferences.Editor esp = sp.edit();
        esp.putString(cle, valeur);
        esp.commit();
    }

    public String lirePref(String cle){
        SharedPreferences sp = getSharedPreferences("ClinoTag_Preference", MODE_PRIVATE);
        return sp.getString(cle, null);
    }


    public boolean isNetworkConnected() {
        if (Connectivity.isConnected(getApplicationContext())) {
            dispoAPI = false;
            try {
                // Execute the NetworkTask
                NetworkTask networkTask = new NetworkTask();
                networkTask.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, dns);

                // Wait for the result, but with a timeout
                Boolean isConnected = networkTask.get(1200, TimeUnit.MILLISECONDS);

                if (isConnected != null && isConnected) {
                    dispoAPI = true;
                } else {
                    // Network is not connected, cancel the task
                    networkTask.cancel(true);
                    return false;
                }
            } catch (InterruptedException | ExecutionException | TimeoutException e) {
                e.printStackTrace();
            }
            return dispoAPI;
        }
        return false;
    }


    public static String getCurrentTime() {
        DateFormat dateFormat = new SimpleDateFormat("HH:mm");
        Date date = new Date();
        return dateFormat.format(date);
    }

    // Get the employee names from Globals.listEmployees
    public static List<String> getEmployeeNames() {
        List<String> employeeNames = new ArrayList<>();
        for (Employee employee : Globals.listEmployees) {
            employeeNames.add(employee.toString());
        }
        return employeeNames;
    }
}