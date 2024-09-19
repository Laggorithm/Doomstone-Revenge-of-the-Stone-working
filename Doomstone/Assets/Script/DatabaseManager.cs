using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    private int currentElapsedTime;

    private const float saveInterval = 1f; // Interval in seconds
    private const string elapsedTimeKey = "ElapsedTime";

    private void Start()
    {
        // Start the coroutine to save elapsed time every second
        StartCoroutine(SaveElapsedTimeCoroutine());
    }
    
    private IEnumerator SaveElapsedTimeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);
            SaveElapsedTime((int)TimerManager.elapsedTime);
        }
    }

    public void SetElapsedTime(int elapsedTime)
    {
        currentElapsedTime = elapsedTime;
    }

    public void SaveElapsedTime(int elapsedTime)
    {
        int savedElapsedTime = LoadElapsedTime();

        if (TimerManager.elapsedTime > savedElapsedTime || savedElapsedTime == 0)
        {
            PlayerPrefs.SetInt(elapsedTimeKey, elapsedTime);
            PlayerPrefs.Save(); // Ensures the data is written immediately
            Debug.Log("ElapsedTime saved using PlayerPrefs.");
        }
        else if (TimerManager.elapsedTime < savedElapsedTime || savedElapsedTime > 0)
        {
            PlayerPrefs.SetInt(elapsedTimeKey, elapsedTime);
            PlayerPrefs.Save();
            Debug.Log("ElapsedTime saved as a new record using PlayerPrefs.");
        }
    }

    public int LoadElapsedTime()
    {
        if (PlayerPrefs.HasKey(elapsedTimeKey))
        {
            int elapsedTime = PlayerPrefs.GetInt(elapsedTimeKey);
            Debug.Log("ElapsedTime loaded using PlayerPrefs.");
            return elapsedTime;
        }

        Debug.Log("No ElapsedTime record found in PlayerPrefs.");
        return 0;
    }
}
