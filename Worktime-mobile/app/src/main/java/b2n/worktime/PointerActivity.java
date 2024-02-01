package b2n.worktime;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.Toast;

import com.google.gson.Gson;

import java.util.concurrent.ExecutionException;

import b2n.worktime.dto.Employee;
import b2n.worktime.dto.PostTag;
import b2n.worktime.json.JsonTaskIntegerPost;

public class PointerActivity extends AppCompatActivity {

    private Spinner employeeSpinner;
    private Button confirm;

    @Override
    public void onStart() {
        super.onStart();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pointer);
        setTitle("Register Tag");

        employeeSpinner = findViewById(R.id.employeeSpinner);
        confirm = findViewById(R.id.Confirm);

        // Create an ArrayAdapter to populate the spinner with employee names
        ArrayAdapter<String> adapter = new ArrayAdapter<>(this,
                android.R.layout.simple_spinner_item, Globals.getEmployeeNames());

        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        employeeSpinner.setAdapter(adapter);
    }

    public void OnConfirm(View v)
    {
        if(Globals.listEmployees.size() > 0) {
            String selectedEmployeeName = employeeSpinner.getSelectedItem().toString();

            int selectedId = 0;

            for (int i = 0; i < Globals.listEmployees.size(); i++) {
                Employee employee = Globals.listEmployees.get(i);
                if (employee.getName().equals(selectedEmployeeName)) {
                    selectedId = employee.getId();
                    break;
                }
            }

            if (selectedId > 0)
            {
                int result = -100;
                PostTag tag = new PostTag(Globals.tag, selectedId);
                try {
                    result = new JsonTaskIntegerPost().executeOnExecutor( AsyncTask.THREAD_POOL_EXECUTOR,
                            Globals.urlAPIWorkTime + "Employee/register",
                            new Gson().toJson(tag)).get();
                } catch (ExecutionException e) {
                    e.printStackTrace();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                if (result == 0) {
                    Toast.makeText(getApplicationContext(), "The tag registered successfully", Toast.LENGTH_SHORT).show();
                    startActivityForResult(new Intent(getApplicationContext(), MainActivity.class), 0);
                    Globals.tag = "";
                } else {
                    Toast.makeText(getApplicationContext(), "Failed to register tag.", Toast.LENGTH_SHORT).show();
                }
            }
            else{
                Toast.makeText(getApplicationContext(), "Please select employee", Toast.LENGTH_SHORT).show();
                return;
            }

        }
        else{
            Toast.makeText(getApplicationContext(), "There is no employee without tag", Toast.LENGTH_SHORT).show();
            return;
        }
    }

    static String hexTagId;
}