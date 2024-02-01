//cd 'C:\Program Files\Android\platform-tools'
//.\adb tcpip 5555
//.\adb shell ip addr show wlan0
//.\adb connect 192.168.20.18

package b2n.worktime;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.view.menu.MenuBuilder;
import androidx.core.content.ContextCompat;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.media.AudioManager;
import android.media.ToneGenerator;
import android.nfc.NfcAdapter;
import android.nfc.Tag;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.os.Handler;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.TimeZone;
import java.util.concurrent.ExecutionException;

import b2n.worktime.composant.DateTimeUtils;
import b2n.worktime.composant.DialogTexte;
import b2n.worktime.composant.Location;
import b2n.worktime.composant.SharedPreferencesHelper;
import b2n.worktime.dto.PostPassage;
import b2n.worktime.dto.ScanSuccess;
import b2n.worktime.json.JsonTaskEmployees;
import b2n.worktime.json.JsonTaskScanSuccess;
import b2n.worktime.outil.Format;

public class MainActivity extends AppCompatActivity {

    public Location location;
    static boolean scanEnCours = false;
    private Handler delayHandler;
    TextView labPointer = null;
    TextView labCurrentTime = null;
    TextView labUserName = null;
    NfcAdapter mAdapter;
    Button btn = null;
    private Handler handler;
    private Handler showHandler;
    private Runnable updateTimeRunnable;

    @Override
    public void onStart() {
        super.onStart();

        mAdapter = NfcAdapter.getDefaultAdapter(this);

        btn = this.findViewById(R.id.Notification);
        btn.setVisibility(View.GONE);

        labCurrentTime = this.findViewById(R.id.lblCurrentTime);
        labPointer = this.findViewById(R.id.pointerName);
        labUserName = this.findViewById(R.id.userName);

        labUserName.setVisibility(View.GONE);
        labPointer.setText(Globals.pointer.name);

        handler = new Handler();
        showHandler = new Handler();
        delayHandler = new Handler();

        // Create a Runnable to update the time periodically
        updateTimeRunnable = new Runnable() {
            @Override
            public void run() {
                updateTime();
                handler.postDelayed(this, 1000); // Update every second
            }
        };

        if(mAdapter == null){
        } else {
            if (!mAdapter.isEnabled()) {
            } else {
                toogleNfc(true);
            }
        }
    }

    private void updateTime() {
        // Define the desired time zone (GMT+11)
        TimeZone timeZone = TimeZone.getTimeZone("Pacific/Efate");

        // Get the current time in the specified time zone
        Calendar calendar = Calendar.getInstance(timeZone);
        Date currentTime = calendar.getTime();

        // Format the time as desired (in this example, using HH:mm:ss)
        SimpleDateFormat dateFormat = new SimpleDateFormat("HH:mm:ss", Locale.getDefault());
        String formattedTime = dateFormat.format(currentTime);

        // Display the formatted time in the TextView
        labCurrentTime.setText(formattedTime);
    }



    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        location = new Location(getApplicationContext());

        setTitle(Globals.getCurrentTime() + " - " + Globals.pointer.name);
        new CountDownTimer(5000, 300) {

            public void onTick(long millisUntilFinished) {}

            public void onFinish() {
                setTitle(Globals.getCurrentTime() + " - " + Globals.pointer.name);
                this.start();
            }
        }.start();
    }

    @SuppressLint("RestrictedApi")
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main, menu);
        if(menu instanceof MenuBuilder){
            MenuBuilder m = (MenuBuilder) menu;
            m.setOptionalIconsVisible(true);
        }
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.action_actualise:
                break;
            case R.id.action_deconnexion:
                Globals g = (Globals)getApplication();
                g.ecrirePref("codeAgent", null);
                startActivityForResult(new Intent(getApplicationContext(), LoginActivity.class), 0);
                finish();
                break;
        }
        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onResume() {
        super.onResume();
        location.initLocation();
        handler.post(updateTimeRunnable);
    }

    @Override
    protected void onPause() {
        super.onPause();
        // Stop updating the time when the app is paused
        handler.removeCallbacks(updateTimeRunnable);
    }

    static String hexTagId;

    void toogleNfc(Boolean enable) {
        if(enable){
            final ToneGenerator tg = new ToneGenerator(AudioManager.STREAM_MUSIC, ToneGenerator.MAX_VOLUME);

            mAdapter.enableReaderMode(this, new NfcAdapter.ReaderCallback() {
                @Override
                public void onTagDiscovered(final Tag tag) {
                    runOnUiThread(() -> {
                        if(!scanEnCours){
                            hexTagId = Format.bytesToHexString(tag.getId()).substring(2).toUpperCase();
                            Toast.makeText(getApplicationContext(), hexTagId, Toast.LENGTH_SHORT).show();
                            tg.startTone(ToneGenerator.TONE_CDMA_ALERT_CALL_GUARD,200);
                            ReadingTag(hexTagId);
                        }
                    });

                }
            }, NfcAdapter.FLAG_READER_NFC_A |
                    NfcAdapter.FLAG_READER_NFC_B |
                    NfcAdapter.FLAG_READER_NFC_F |
                    NfcAdapter.FLAG_READER_NFC_V |
                    NfcAdapter.FLAG_READER_NFC_BARCODE |
                    NfcAdapter.FLAG_READER_NO_PLATFORM_SOUNDS, null);
        }else{
            mAdapter.disableReaderMode(this);
        }
    }

    private void handleOnlineMode() {

        SharedPreferencesHelper preferencesHelper = new SharedPreferencesHelper(getApplicationContext());

        try{
            List<PostPassage> savedPassages = preferencesHelper.getPassages();

            for(int i = 0 ; i < savedPassages.size(); i++)
            {
                PostPassage passage = savedPassages.get(i);
                ScanSuccess result = new JsonTaskScanSuccess().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR,
                        Globals.urlAPIWorkTime + "Passage",
                        new Gson().toJson(passage)).get();
            }
        }
        catch (Exception e)
        {

        }

        List<PostPassage> newPassages = new ArrayList<>();

        preferencesHelper.savePassages(newPassages);
    }

    private void handleOfflineMode(PostPassage passage) {
        scanEnCours = false;
        SharedPreferencesHelper preferencesHelper = new SharedPreferencesHelper(getApplicationContext());

        // Save the passage to SharedPreferences for offline processing
        List<PostPassage> savedPassages = preferencesHelper.getPassages();
        savedPassages.add(passage); // Add the current PostPassage
        preferencesHelper.savePassages(savedPassages);
        return;
    }

    private boolean isTagReadingAllowed = true; // Add this variable
    private void ReadingTag(String hexTagId) {
        try {

            if (hexTagId.length() > 0 && isTagReadingAllowed)
            {
                isTagReadingAllowed = false;
                scanEnCours = true;

                Globals g = (Globals)getApplication();

                double longitude = location.longitude;
                double latitude = location.latitude;

                Date dateTimeToSend = new Date(); // Replace with your DateTime value
                String _logTime = DateTimeUtils.formatDateTime(dateTimeToSend);

                PostPassage passage = new PostPassage(Globals.pointer.id, hexTagId, _logTime, longitude, latitude);

                if(!g.isNetworkConnected())
                {
                    handleOfflineMode(passage);
                    Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_LONG).show();
                    return;
                }

                try {
                    if(g.isNetworkConnected())
                    {
                        handleOnlineMode();

                        ScanSuccess result;

                        result = new JsonTaskScanSuccess().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR,
                                Globals.urlAPIWorkTime + "Passage",
                                new Gson().toJson(passage)).get();

                        if (result.userName.length() == 0) {
                            Globals.tag = hexTagId;
                            String req = Globals.urlAPIWorkTime + "employee/tag";
                            Globals.listEmployees = new JsonTaskEmployees().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();

                            Globals.ScanSuccessInfo = result;
                            Toast.makeText(getApplicationContext(), "You'll register the unknown tag.", Toast.LENGTH_SHORT).show();
                            startActivityForResult(new Intent(getApplicationContext(), PointerActivity.class), 0);
                        }
                        else {
                            Globals.ScanSuccessInfo = result;
                            Toast.makeText(getApplicationContext(), "The Passage is successfully registered.", Toast.LENGTH_SHORT).show();

                            labUserName.setVisibility(View.VISIBLE);
                            labUserName.setText(Globals.ScanSuccessInfo.userName);

                            Drawable successDrawable = ContextCompat.getDrawable(this, R.drawable.checksuccess);

                            btn.setVisibility(View.VISIBLE);
                            btn.setForeground(successDrawable);
                            btn.setBackground(null);

                            showHandler.postDelayed(new Runnable() {
                                @Override
                                public void run() {
                                    // Revert the context back to the original state
                                    labUserName.setVisibility(View.GONE);
                                    btn.setBackground(ContextCompat.getDrawable(MainActivity.this, R.drawable.check));
                                    btn.setForeground(ContextCompat.getDrawable(MainActivity.this, R.drawable.check));
                                    btn.setVisibility(View.GONE);
                                }
                            }, 2000); // Delay for 1 second (2000 milliseconds)
                        }
                    }
                    else{
                        handleOfflineMode(passage);
                        return;
                    }
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }else {
                Toast.makeText(getApplicationContext(), R.string.noconnexion, Toast.LENGTH_LONG).show();
            }
            scanEnCours = false;

            delayHandler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    isTagReadingAllowed = true; // Allow the function to run again after the delay
                }
            }, 10000);


        } catch (Exception e) {
            Toast.makeText(getApplicationContext(), "Error reading tag.", Toast.LENGTH_SHORT).show();
        }
    }
}