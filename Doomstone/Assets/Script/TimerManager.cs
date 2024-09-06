using UnityEngine;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    // The time that has passed since the start of the program
    public static float elapsedTime { get; private set; }

    // Singleton instance
    public static TimerManager Instance { get; private set; }

    // Ensure the TimerManager persists between scenes
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes this object persist between scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if they exist
        }
    }

    // Start the coroutine to count the time
    private void Start()
    {
        StartCoroutine(CountTime());
    }

    // Coroutine to count seconds
    private IEnumerator CountTime()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime; // Time passed since the last frame
            yield return null; // Wait for the next frame
        }
    }
}
