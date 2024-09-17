using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public TMP_Text FastestRun;
    public int FastestRrun;
    private DatabaseManager dbManager;
    private void Start()
    {
       
        FastestRrun = dbManager.LoadElapsedTime();
        
    }
    private void Update()
    {
        FastestRun.text = FastestRrun.ToString();
    }
    public GameObject Controls;
    // Start is called before the first frame update
    public void LoadScene()
    {
        SceneManager.LoadScene("LevelOne");
    }
    public void LoadControls()
    {
        Controls.gameObject.SetActive(true);
    }
    public void UnloadControls()
    {
        Controls.gameObject.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
