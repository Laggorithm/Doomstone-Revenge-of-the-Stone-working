using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;
    public TMP_Text PHp;
    public TMP_Text BHp;
    public TMP_Text DoubleJump;
    public TMP_Text Timer;
    private int ElapsedTime;
    public GameObject dead;
    public static bool IsDead;
    // Start is called before the first frame update
    private void Awake()
    {
        // Ensure there's only one instance of the Canvas that persists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this Canvas from being destroyed
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate Canvas in other scenes
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
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
        else { BHp.gameObject.SetActive(false);}
        if (Player.isGrounded == true)
        {
            DoubleJump.text = ("1x jump");
        }
        else { DoubleJump.text = ("0x jumps"); }
        Timer.text = ElapsedTime.ToString();
        if (IsDead == true)
        {
            dead.gameObject.SetActive(true);
        }
        else if (IsDead == false)
        {
            dead.gameObject.SetActive(false);
        }
    }
}
