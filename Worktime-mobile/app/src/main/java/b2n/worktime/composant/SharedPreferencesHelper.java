package b2n.worktime.composant;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

import b2n.worktime.dto.PostPassage;

public class SharedPreferencesHelper {
    private static final String PREF_NAME = "PostPassagePrefs";
    private static final String KEY_PASSAGES = "passages";

    private SharedPreferences sharedPreferences;

    public SharedPreferencesHelper(Context context) {
        sharedPreferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }

    public List<PostPassage> getPassages() {
        String json = sharedPreferences.getString(KEY_PASSAGES, null);
        Log.d("SharedPreferencesHelper", "Retrieved JSON: " + json); // Add this line
        if (json != null) {
            Type type = new TypeToken<List<PostPassage>>(){}.getType();
            return new Gson().fromJson(json, type);
        }

        return new ArrayList<>();
    }

    public void savePassages(List<PostPassage> passages) {
        String json = new Gson().toJson(passages);
        Log.d("SharedPreferencesHelper", "Saving JSON: " + json); // Add this line
        sharedPreferences.edit().putString(KEY_PASSAGES, json).apply();
    }
}
