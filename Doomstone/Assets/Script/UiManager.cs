using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;
    public TMP_Text PHp;
    public TMP_Text BHp;
    public TMP_Text DoubleJump;
    public TMP_Text Timer;
    public TMP_Text FastestRun; // Fastest run display
    private int ElapsedTime;
    private int FastestTime; // Fastest time storage
    public GameObject dead;
    public static bool IsDead;

    public DatabaseManager dbManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this Canvas from being destroyed
            dbManager = GetComponent<DatabaseManager>(); // Ensure DatabaseManager is on the same GameObject
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate Canvas in other scenes
        }
    }

    void Start()
    {
        FastestTime = dbManager.LoadElapsedTime(); // Load saved fastest elapsed time on start

        // If no saved time (e.g., first run), set a high default value
        if (FastestTime == 0)
        {
            FastestTime = int.MaxValue; // Representing the "least time" as max initially
        }

        // Update the FastestRun text to display the loaded time
        FastestRun.text = FastestTime == int.MaxValue ? "N/A" : FastestTime.ToString();
    }

    void Update()
    {
        ElapsedTime = (int)TimerManager.elapsedTime;
        PHp.text = Player.Hp.ToString();
        BHp.text = BossCommonBehaviour.Hp.ToString();
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "BossRoomTwo" || currentScene.name == "BossRoomOne")
        {
            BHp.gameObject.SetActive(true);
        }
        else
        {
            BHp.gameObject.SetActive(false);
        }

        if (Player.isGrounded)
        {
            DoubleJump.text = "1x jump";
        }
        else
        {
            DoubleJump.text = "0x jumps";
        }

        Timer.text = ElapsedTime.ToString();

        // If the player is dead (or game ends), check and update the fastest time
        if (IsDead)
        {
            dead.gameObject.SetActive(true);

            // Check if the current run is the fastest one
            if (ElapsedTime < FastestTime)
            {
                FastestTime = ElapsedTime; // Update FastestTime with the new time
                dbManager.SaveElapsedTime(FastestTime); // Save the new fastest time to the database

                // Update the FastestRun text to show the new fastest time
                FastestRun.text = FastestTime.ToString();
            }
        }
        else
        {
            dead.gameObject.SetActive(false);
        }
    }
}
