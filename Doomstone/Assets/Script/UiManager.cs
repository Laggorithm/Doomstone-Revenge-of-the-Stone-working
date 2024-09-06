using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;
    public TMP_Text PHp;
    public TMP_Text BHp;
    public TMP_Text DoubleJump;
    public TMP_Text Timer;
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
        PHp.text = Player.Hp.ToString();
        BHp.text = BossCommonBehaviour.Hp.ToString();
        if (Player.isGrounded == true)
        {
            DoubleJump.text = ("1x jump available");
        }
        else { DoubleJump.text = ("0x"); }
        Timer.text = TimerManager.elapsedTime.ToString();

    }
}
