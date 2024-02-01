package b2n.worktime;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.graphics.Point;
import android.os.AsyncTask;
import android.os.Build;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.concurrent.ExecutionException;

import b2n.worktime.composant.HttpsTrustManager;
import b2n.worktime.dto.Pointer;
import b2n.worktime.json.JsonTaskPointer;

public class LoginActivity extends AppCompatActivity {

    TextView labCodeLogin;
    String codeSaisie = "";
    CountDownTimer cdtSaisie = null;
    static boolean bloqueSaisie = false;


    @RequiresApi(api = Build.VERSION_CODES.O)
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        Globals.idConstructeur = Build.SERIAL.toUpperCase();

        HttpsTrustManager.allowAllSSL();
        labCodeLogin = (TextView) findViewById(R.id.labCodeLogin);
        setTitle("Worktime - Login");

        Globals g = (Globals)getApplication();
        String codeAgent = g.lirePref("code");

    }

    public void SaisieCode(View v) {
        if(bloqueSaisie) return;

        if(cdtSaisie != null)cdtSaisie.cancel();
        cdtSaisie = new CountDownTimer(2500, 300) {
            public void onTick(long millisUntilFinished) {}
            public void onFinish() {
                bloqueSaisie = false;
                codeSaisie = "";
                labCodeLogin.setText("");
                this.start();
            }
        }.start();

        Button btn = (Button) findViewById(v.getId());
        codeSaisie = codeSaisie + btn.getText();
        labCodeLogin.append("*");

        if(codeSaisie.length() == 5){

            bloqueSaisie = true;
            String req = Globals.urlAPIWorkTime + "pointer/" + codeSaisie;
            try {
                Globals g = (Globals)getApplication();

                Pointer result = new JsonTaskPointer().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,req).get();
                if (result != null) {
                    finish();
                    Globals.pointer = result;
                    g.ecrirePref("codeAgent", codeSaisie);
                    startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
                } else {
                    Toast.makeText(getApplicationContext(), "Unknown code", Toast.LENGTH_SHORT).show();
                }
            } catch (ExecutionException e) {
                e.printStackTrace();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            codeSaisie = "";
            labCodeLogin.setText("");
            bloqueSaisie = false;
        }
    }
}